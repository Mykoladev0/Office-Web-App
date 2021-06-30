using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BullsBluffCore.Controllers
{
    public class AccountController:Controller
    {
        public async Task Login(string returnUrl="/"){
            await HttpContext.ChallengeAsync("Auth0", 
                new AuthenticationProperties(){
                    RedirectUri=returnUrl
                }
            );
        }

        [Authorize]
        public async Task Logout(){
            await HttpContext.SignOutAsync("Auth0",
                new AuthenticationProperties(){
                    //indicate to Auth0 where to redirect after successful logout
                    //NOTE: returning URI must be whitelisted with Auth0
                    //This is found in Auth0 dashboard at *Allowed Logout Urls**
                    RedirectUri = Url.Action("Index", "Home")
                }
            );
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}