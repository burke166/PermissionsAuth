using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PermissionsAuth.Data;
using NLog;

namespace PermissionsAuth.Services
{
    public class UserService : IUserService
    {
        private readonly Db _context;
        private readonly Logger _logger;

        public UserService(Db context)
        {
            _context = context;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<ServiceActionResult<bool>> CreateUserIfNotExistsAsync(string auth0UserId, ClaimsPrincipal httpContextUser)
        {
            var result = new ServiceActionResult<bool>();
            result.Value = true;
            UserAccount user = null;
            UserRole basicRole = null, superAdminRole = null;
            bool firstUser = false;

            try
            {
                firstUser = !_context.UserAccounts.Any();
                user = _context.UserAccounts.Where(ua => ua.Auth0UserId == auth0UserId).FirstOrDefault();
                basicRole = _context.UserRoles.Where(ur => ur.Name == Constants.Roles.Basic.ToString()).FirstOrDefault();
                superAdminRole = _context.UserRoles.Where(ur => ur.Name == Constants.Roles.SuperAdmin.ToString()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to load user account database.");
                result.Errors.Add("Unable to connect to user database.");
                result.Value = false;
                result.HttpStatusCode = HttpStatusCode.InternalServerError;
            }

            if (!result.Success) return result;

            if (user == null)
            {
                user = new UserAccount
                {
                    Auth0UserId = auth0UserId,
                    Email = httpContextUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty,
                    Mobile = httpContextUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value ?? string.Empty,
                    Name = httpContextUser.Identity.Name,
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

                try
                {
                    await _context.UserAccounts.AddAsync(user);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unable to load user account database.");
                    result.Errors.Add("Unable to connect to user database.");
                    result.Value = false;
                    result.HttpStatusCode = HttpStatusCode.InternalServerError;
                    return result;
                }

            }

            return result;
        }

        public async Task<ServiceActionResult<List<string>>> GetUserPermissionsAsync(string auth0UserId)
        {
            var result = new ServiceActionResult<List<string>>();
            result.Value = new List<string>();

            try
            {
                var roles = await _context.UserAccounts.AsNoTracking()
                    .Where(ua => ua.Auth0UserId == auth0UserId)
                    .Include(ua => ua.UserRoles)
                    .SelectMany(ua => ua.UserRoles)
                    .Include(uar => uar.UserRole)
                    .Select(ur => ur.UserRole)
                    .ToListAsync();

                result.Value = roles.SelectMany(r => r.Permissions).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to load user account information.");
                result.Errors.Add("Unable to load user information.");
                result.HttpStatusCode = HttpStatusCode.InternalServerError;
            }

            return result;
        }
    }
}
