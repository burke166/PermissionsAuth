using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using PermissionsAuth.Data;

namespace PermissionsAuth.Services
{
    public interface IUserService
    {
        public Task<ServiceActionResult<bool>> CreateUserIfNotExistsAsync(string auth0UserId, ClaimsPrincipal httpContextUser);
        public Task<ServiceActionResult<List<string>>> GetUserPermissionsAsync(string auth0UserId);
    }
}
