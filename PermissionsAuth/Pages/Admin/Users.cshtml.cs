using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PermissionsAuth.Pages.Admin
{
    [Authorize(Policy = Constants.Permissions.Users.View)]
    public class UsersModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}