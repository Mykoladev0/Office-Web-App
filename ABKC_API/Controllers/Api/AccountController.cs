using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CoreApp.Interfaces;
using CoreApp.Models;
using CoreDAL.Interfaces;
using CoreDAL.Models.v2;
using EmailValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreApp.Controllers.Api
{
    [Route("api/v1/Account")]
    public class AccountController : BaseAuthorizedAPIControllerWithUser
    {
        private readonly IOktaUserService _oktaService;
        private readonly IABKCUserService _abkcUserService;
        private readonly IMapper _autoMapper;

        public AccountController(IOktaUserService oktaService, IABKCUserService abkcUserService, IMapper autoMapper) : base(abkcUserService)
        {
            _oktaService = oktaService;
            _abkcUserService = abkcUserService;
            _autoMapper = autoMapper;
        }

        [HttpGet("GetCurrentUserId")]
        [Authorize(Roles = "Everyone")]
        public ActionResult GetUserId()
        {
            var access = base.GetLoggedInUserId();
            return Ok(access);
        }
        [HttpGet("GetCurrentUserInformation")]
        [Authorize(Roles = "Everyone")]
        public async Task<ActionResult> GetCurrentUserInformation()
        {
            string id = base.GetLoggedInUserId();
            Okta.Sdk.IUser user = await _oktaService.GetUserFromOkta(id);
            UserModel abkcUser = await _abkcUserService.GetUserFromOktaId(id);
            if (user != null)
            {
                FullABKCUserDTO rtn = _autoMapper.Map<FullABKCUserDTO>(abkcUser);
                rtn.Profile = user.Profile;
                //add roles
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var roles = claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
                // rtn.Roles = roles;

                rtn.ABKCRolesUserBelongsTo = roles.ToList();
                return Ok(rtn);
            }
            return NotFound();
        }
        [AllowAnonymous]
        [HttpPost("GetToken")]
        public async Task<ActionResult<string>> GetToken([FromBody]UserLoginModel user)
        {
            // var user = _userService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            string token = await _oktaService.GetUserToken(user.UserName, user.Password);

            return Ok(token);
        }

        /// <summary>
        /// allows for a new user request to be sent with an account type of Owner, Judge, or Representative roles
        /// Any Admin or ABKC Office accounts will have be created by an existing user. An error will occur if email is already registered
        /// Once the account is approved by the ABKC Office, an email will be sent out for the user to set up their password.
        /// </summary>
        /// <param name="user">email, first and last name required. Role will default to owner</param>
        /// <returns></returns>
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> RegisterUser([FromBody]UserRegistrationModel user)
        {
            //todo:validate email address!

            if (!EmailValidator.Validate(user.EmailAddress))
            {
                return BadRequest($"email address {user.EmailAddress} is not valid.");
            }
            if (user.RoleRequested == SystemRoleEnum.ABKCOffice)
            {
                return BadRequest("ABKC Office users cannot be requested");
            }
            if (user.RoleRequested == SystemRoleEnum.Administrator)
            {
                return BadRequest("ABKC Administrator users cannot be requested");
            }

            Okta.Sdk.IUser found = await _oktaService.GetUserFromLogin(user.EmailAddress);

            if (found != null)
            {
                return BadRequest($"Account for Email Address {user.EmailAddress} already exists with the remote login store");
            }
            //check locally too!
            CoreDAL.Models.v2.UserModel abkcUser = await _userService.GetUserFromOktaLogin(user.EmailAddress);
            if (abkcUser != null)
            {
                return BadRequest($"Account for Email Address {user.EmailAddress} already exists in the ABKC system");
            }
            try
            {
                found = await _oktaService.RegisterUser(user);
                if (found != null)
                {
                    //register locally!
                    abkcUser = await _userService.AddUser(found.Id, found.Profile.Login);
                    if (abkcUser != null)
                    {
                        await _userService.AddUserToRole(abkcUser, user.RoleRequested);
                        return true;
                    }

                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return false;
        }
    }
}