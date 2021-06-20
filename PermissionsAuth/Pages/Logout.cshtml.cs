using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PermissionsAuth.Pages
{
    [Authorize]
    public class LogoutModel : PageModel
    { 
        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
                {
                    RedirectUri = Url.Page("/Index")
                });
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            else
            {
                RedirectToPage("/Index");
            }
        }
    }
}
