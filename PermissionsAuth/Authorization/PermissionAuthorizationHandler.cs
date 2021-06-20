using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PermissionsAuth.Data;

namespace PermissionsAuth.Authorization
{
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly Db _db;
        public PermissionAuthorizationHandler(Db db)
        {
            _db = db;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return;
            }

            var auth0UserId = context.User.Claims.FirstOrDefault(c => c.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(auth0UserId)) return;

            bool firstUser = !_db.UserAccounts.Any();
            var user = _db.UserAccounts.Where(ua => ua.Auth0UserId == auth0UserId).FirstOrDefault();

            var basicRole = _db.UserRoles.Where(ur => ur.Name == Constants.Roles.Basic.ToString()).FirstOrDefault();
            var superAdminRole = _db.UserRoles.Where(ur => ur.Name == Constants.Roles.SuperAdmin.ToString()).FirstOrDefault();

            if (user == null)
            {
                user = new UserAccount
                {
                    Auth0UserId = auth0UserId,
                    Email = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty,
                    Mobile = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value ?? string.Empty,
                    Name = context.User.Identity.Name,
                    Status = UserAccountStatus.Enabled,
                    UserRoles = new List<UserAccountUserRole>
                    {
                        new UserAccountUserRole
                        {
                            UserAccount = user,
                            UserRole = firstUser ? superAdminRole : basicRole
                        }
                    }
                };

                await _db.UserAccounts.AddAsync(user);
                await _db.SaveChangesAsync();
            }

            if (requirement.Permission == Constants.Permissions.Any)
            {
                context.Succeed(requirement);
            }

            var permissions = user.UserRoles.Select(ur => ur.UserRole).SelectMany(ur => ur.Permissions).Where(p => p == requirement.Permission || p == Constants.Permissions.All);

            if (permissions.Any())
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
