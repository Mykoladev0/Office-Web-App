using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreDAL.Interfaces;
using CoreDAL.Models.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApp.Controllers.Api
{
    [Authorize]
    public abstract class BaseAuthorizedAPIController : ControllerBase
    {

    }

    public abstract class BaseAuthorizedAPIControllerWithUser : BaseAuthorizedAPIController
    {
        protected readonly IABKCUserService _userService;

        public BaseAuthorizedAPIControllerWithUser(IABKCUserService userService) => _userService = userService;
        protected string GetLoggedInUserId()
        {
            // var userId = HttpContext.User.Claims.SingleOrDefault(u=>u.Type == "uid")?.Value;
            // var idClaim = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);
            var idClaim = HttpContext.User.FindFirst(x => x.Type == "uid");
            return idClaim.Value;
        }

        protected string GetLoggedInUserName()
        {
            var usernameClaim = HttpContext.User.FindFirst(x => x.Type == "username");
            return usernameClaim.Value;
        }
        protected async Task<UserModel> GetLoggedInUser()
        {
            string oktaId = GetLoggedInUserId();
            UserModel curUser = await _userService.GetUserFromOktaId(oktaId);
            if (curUser == null)
            {
                curUser = await _userService.AddUser(oktaId, GetLoggedInUserName());
            }
            return curUser;
        }
        // protected Okta.Sdk.User GetLoggedInUser()
        // {
        //     var userClaim = HttpContext.User.FindFirst(x => x.Type == "user").Value;
        //     if (string.IsNullOrEmpty(userClaim))
        //     {
        //         return null;
        //     }
        //     return Newtonsoft.Json.JsonConvert.DeserializeObject<Okta.Sdk.User>(userClaim);
        // }
    }
}