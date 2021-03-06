using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog;
using PermissionsAuth.Authorization;
using PermissionsAuth.Data;
using PermissionsAuth.Services;

namespace PermissionsAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serverVersion = new MariaDbServerVersion(new Version(10, 3, 29));
            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            services.Configure<Configuration.Auth0Options>(Configuration.GetSection("Auth0"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Login";
                options.AccessDeniedPath = "/Error";
            })
            .AddOpenIdConnect("Auth0", options =>
            {
                options.Authority = $"https://{Configuration["Auth0:Domain"]}";
                options.ClientId = Configuration["Auth0:ClientId"];
                options.ClientSecret = Configuration["Auth0:ClientSecret"];
                options.ResponseType = "code";

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                options.CallbackPath = new PathString("/signin-auth0");
                options.ClaimsIssuer = "Auth0";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = async (context) =>
                    {
                        var auth0UserId = context.Principal.Claims.FirstOrDefault(c => c.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                        var optionsBuilder = new DbContextOptionsBuilder<Db>();
                        optionsBuilder.UseMySql(connectionString, serverVersion);

                        using (var _db = new Db(optionsBuilder.Options))
                        {
                            var service = new UserService(_db);
                            await service.CreateUserIfNotExistsAsync(auth0UserId, context.Principal);
                        }
                    },
                    OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                        var logoutUri = $"https://{Configuration["Auth0:Domain"]}/v2/logout?client_id={Configuration["Auth0:ClientId"]}";

                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                                // transform to absolute
                                var request = context.Request;
                                if (postLogoutUri.Contains(request.PathBase))
                                    postLogoutUri = request.Scheme + "://" + request.Host + postLogoutUri;
                                else
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                            }
                            logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                };
            });


            services.AddDbContext<Data.Db>(options => options.UseMySql(connectionString, serverVersion));

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddScoped<IUserService, UserService>();

            services.AddRazorPages();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.Permissions.Any, policy => policy.Requirements.Add(new PermissionRequirement(Constants.Permissions.Any)));

                foreach(string permission in Constants.Permissions.GeneratePermissionsForModule(Constants.Modules.Users))
                    options.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages().AllowAnonymous();
            });
        }
    }
}
