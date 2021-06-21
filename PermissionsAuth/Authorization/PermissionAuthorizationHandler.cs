using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PermissionsAuth.Services;

namespace PermissionsAuth.Authorization
{
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUserService _userService;
        public PermissionAuthorizationHandler(IUserService userService)
        {
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return;
            }

            if (requirement.Permission == Constants.Permissions.Any)
            {
                context.Succeed(requirement);
            }

            var auth0UserId = context.User.Claims.FirstOrDefault(c => c.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            var permissions = (await _userService.GetUserPermissionsAsync(auth0UserId)).Value.Where(p => p == requirement.Permission || p == Constants.Permissions.All);

            if (permissions.Any())
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
