using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreApp.Interfaces;
using CoreDAL.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Okta.Sdk;

namespace CoreApp.Authentication
{
    public class OktaClaimsTransformation : IClaimsTransformation
    {
        private readonly IOktaUserService _oktaService;
        private readonly IABKCUserService _userService;

        public OktaClaimsTransformation(IOktaUserService oktaService)
        {
            _oktaService = oktaService;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var idClaim = principal.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);
            if (idClaim != null)
            {
                var user = await _oktaService.GetUserFromOkta(idClaim.Value);//_oktaClient.Users.GetUserAsync(idClaim.Value);
                if (user != null)
                {
                    //profile, roles, id
                    ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("username", user.Profile.Login));
                    ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("status", user.Status.Value));
                    var groups = user.Groups.ToEnumerable();
                    foreach (var group in groups)
                    {
                        ((ClaimsIdentity)principal.Identity).AddClaim(new Claim(ClaimTypes.Role, group.Profile.Name));

                    }
                }
            }
            return principal;
        }
    }
}