using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreApp.Interfaces;
using CoreApp.Models;
using CoreDAL.Interfaces;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using EmailValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Okta.Sdk;

namespace CoreApp.Controllers.Api
{

    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
    public class UsersController : BaseAuthorizedAPIControllerWithUser
    {
        private readonly IOktaUserService _oktaService;
        private readonly IMapper _autoMapper;
        private readonly IGeneralRegistrationService _registrationService;

        public UsersController(IABKCUserService userService,
        IOktaUserService oktaService,
        IGeneralRegistrationService registrationService,
        IMapper autoMapper) : base(userService)
        {
            _oktaService = oktaService;
            _autoMapper = autoMapper;
            _registrationService = registrationService;
        }

        /// <summary>
        /// retrieve a user either by ABKC User Id 
        /// </summary>
        /// <param name="id">either by ABKC User Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<FullABKCUserDTO>> GetUserProfile(int id)
        {
            UserModel abkcUser = await _userService.GetUserFromABKCId(id);
            if (abkcUser == null)
            {
                return BadRequest($"Okta Id could not be retrieved for ABKC Id {id}");
            }
            var user = await _oktaService.GetUserFromOkta(abkcUser.OktaId);
            FullABKCUserDTO rtn = _autoMapper.Map<FullABKCUserDTO>(abkcUser);
            rtn.Profile = user.Profile;
            // FullABKCUserDTO rtn = new FullABKCUserDTO
            // {
            //     Id = abkcUser.Id,
            //     LoginName = abkcUser.LoginName,
            //     Profile = user.Profile
            // };
            return Ok(rtn);
        }

        /// <summary>
        /// activates a pending user account
        /// this action should be followed up with setting their role!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("activate/{id}")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<bool>> ActivateUser(int id)
        {
            UserModel abkcUser = await _userService.GetUserFromABKCId(id);
            if (abkcUser == null)
            {
                return BadRequest($"ABKC User could not be retrieved for ABKC Id {id}");
            }
            var user = await _oktaService.GetUserFromOkta(abkcUser.OktaId);
            if (user == null)
            {
                return BadRequest($"Okta user with id {abkcUser.OktaId} is not in the remote system.");
            }
            try
            {
                bool success = await _oktaService.ActivateUser(user);
                return Ok(success);
            }
            catch (Exception e)
            {
                //probably already activated
                return BadRequest($"trouble activating user {id} because {e.Message}");
                // throw e;
            }
        }


        /// <summary>
        /// Suspends an account for a given ABKC User Id that is active
        /// </summary>
        /// <param name="id">ABKC Id</param>
        /// <returns></returns>
        [HttpPost("suspend/{id}")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<bool>> SuspendAccount(int id)
        {
            UserModel abkcUser = await _userService.GetUserFromABKCId(id);
            if (abkcUser == null)
            {
                return BadRequest($"ABKC User could not be retrieved for ABKC Id {id}");
            }
            var user = await _oktaService.GetUserFromOkta(abkcUser.OktaId);
            if (user == null)
            {
                return BadRequest($"Okta user with id {abkcUser.OktaId} is not in the remote system.");
            }
            try
            {
                bool success = await _oktaService.SuspendUserAccount(user);
                if (success)
                {
                    //mark it as suspended in ABKC
                    success = await _userService.SuspendAccount(abkcUser);
                }
                return Ok(success);
            }
            catch (Exception e)
            {
                //probably already not activated
                return BadRequest($"trouble suspending user {id} because {e.Message}");
                // throw e;
            }
        }

        /// <summary>
        /// Un Suspends an account for a given ABKC User Id
        /// </summary>
        /// <param name="id">ABKC Id</param>
        /// <returns></returns>
        [HttpPost("unSuspend/{id}")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<bool>> UnSuspendAccount(int id)
        {
            UserModel abkcUser = await _userService.GetUserFromABKCId(id);
            if (abkcUser == null)
            {
                return BadRequest($"ABKC User could not be retrieved for ABKC Id {id}");
            }
            var user = await _oktaService.GetUserFromOkta(abkcUser.OktaId);
            if (user == null)
            {
                return BadRequest($"Okta user with id {abkcUser.OktaId} is not in the remote system.");
            }
            try
            {
                bool success = await _oktaService.UnSuspendUserAccount(user);
                if (success)
                {
                    //mark it as not suspended in ABKC
                    success = await _userService.UnSuspendAccount(abkcUser);
                }
                return Ok(success);
            }
            catch (Exception e)
            {
                //probably already not activated
                return BadRequest($"trouble un-suspending user {id} because {e.Message}");
                // throw e;
            }
        }

        /// <summary>
        /// removes an account from the ABKC and Okta Systems
        /// </summary>
        /// <param name="id">ABKC Id</param>
        /// <returns></returns>
        [HttpPost("remove/{id}")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<bool>> RemoveAccount(int id)
        {
            UserModel abkcUser = await _userService.GetUserFromABKCId(id);
            if (abkcUser == null)
            {
                return BadRequest($"ABKC User could not be retrieved for ABKC Id {id}");
            }
            var user = await _oktaService.GetUserFromOkta(abkcUser.OktaId);
            if (user == null)
            {
                return BadRequest($"Okta user with id {abkcUser.OktaId} is not in the remote system.");
            }
            try
            {
                bool success = await _oktaService.RemoveUserAccount(user);
                if (success)
                {
                    //remove from ABKC
                    success = await _userService.RemoveUserAccount(abkcUser);
                }
                return Ok(success);
            }
            catch (Exception e)
            {
                //probably already activated
                return BadRequest($"trouble activating user {id} because {e.Message}");
                // throw e;
            }
        }
        /// <summary>
        /// Adds an ABKC user to a specific role
        /// Used to put them in Owner/Rep/Office/Judge roles
        /// </summary>
        /// <param name="userId">Either the ABKC or Okta Id</param>
        /// <param name="role">Either the role Enum Id or the string</param>
        /// <returns></returns>
        [HttpPost("addToRole/{userId}")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<bool>> AddUserToRole(string userId, SystemRoleEnum role)
        {
            // if (!Enum.TryParse<SystemRoleEnum>(role, true, out SystemRoleEnum parsedRole))
            // {
            //     return BadRequest($"Role could not be determined from input {role}");
            // }
            UserModel found = null;
            if (int.TryParse(userId, out int id))
            {
                found = await _userService.GetUserFromABKCId(id);
            }
            else
            {
                found = await _userService.GetUserFromOktaId(userId);
            }
            if (found == null)
            {
                return BadRequest($"No ABKC User with Id {userId} could be found");
            }
            bool result = await _userService.AddUserToRole(found, role);
            //check okta!
            result = await _oktaService.AddUserToRole(found.OktaId, role) || result;
            return Ok(result);
        }
        /// <summary>
        /// Removes a user from a role if they belong to it
        /// </summary>
        /// <param name="userId">Either the ABKC or Okta Id</param>
        /// <param name="role">Either the role Enum Id or the string</param>
        /// <returns></returns>
        [HttpPost("removeFromRole/{userId}")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<bool>> RemoveUserFromRole(string userId, SystemRoleEnum role)
        {
            UserModel found = null;
            if (int.TryParse(userId, out int id))
            {
                found = await _userService.GetUserFromABKCId(id);
            }
            else
            {
                found = await _userService.GetUserFromOktaId(userId);
            }
            if (found == null)
            {
                return BadRequest($"No ABKC User with Id {userId} could be found");
            }
            bool result = await _userService.RemoveUserFromRole(found, role);
            //check okta!
            result = await _oktaService.RemoveUserFromRole(found.OktaId, role) || result;
            return Ok(result);
        }

        /// <summary>
        /// Get Representatives in the ABKC System
        /// </summary>
        /// <returns></returns>
        [HttpGet("representatives")]
        public async Task<ICollection<RepresentativeDTO>> GetRepresentatives()
        {
            ICollection<UserModel> repUsers = await _userService.GetByRole(SystemRoleEnum.Representative);
            ICollection<Okta.Sdk.IUser> otkaRepUsers = await _oktaService.GetByRole(SystemRoleEnum.Representative);
            ICollection<RepresentativeDTO> mapped = _autoMapper.Map<ICollection<RepresentativeDTO>>(repUsers);
            foreach (var rep in mapped)
            {
                var found = otkaRepUsers.FirstOrDefault(r => r.Id == rep.OktaId);
                rep.Profile = found?.Profile;
                ICollection<IRegistration> regs = await _registrationService.GetRegistrationsByRepresentative(rep.Id);
                rep.RegistrationCount = regs.Count();
                try
                {
                    rep.PendingRegistrationCount = regs.Where(reg => reg.CurStatus == RegistrationStatusEnum.Pending).Count();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                RepresentativeModel foundRep = await _userService.GetRepresentativeFromABKCId(rep.Id);
                if (foundRep != null)
                {
                    rep.RepresentativeId = foundRep.Id;
                    rep.JrHandlerRegistrationFee = foundRep.JrHandlerRegistrationFee;
                    rep.LitterRegistrationFee = foundRep.LitterRegistrationFee;
                    rep.PedigreeRegistrationFee = foundRep.PedigreeRegistrationFee;
                    rep.PuppyRegistrationFee = foundRep.PuppyRegistrationFee;
                    rep.TransferFee = foundRep.TransferFee;
                    rep.BullyIdRequestFee = foundRep.TransferFee;
                }
            }
            return mapped;
        }

        /// <summary>
        /// Get Owners with Login Accounts in the ABKC System
        /// </summary>
        /// <returns></returns>
        [HttpGet("owners")]
        public async Task<ICollection<FullABKCUserDTO>> GetOwners()
        {
            ICollection<UserModel> ownerUsers = await _userService.GetByRole(SystemRoleEnum.Owner);
            ICollection<Okta.Sdk.IUser> otkaOwnerUsers = await _oktaService.GetByRole(SystemRoleEnum.Owner);
            otkaOwnerUsers = otkaOwnerUsers.Where(u => u.Status.Value != "STAGED" && u.Status.Value != "DEPROVISIONED").ToList();
            ICollection<FullABKCUserDTO> mapped = _autoMapper.Map<ICollection<FullABKCUserDTO>>(ownerUsers);
            foreach (var rep in mapped)
            {
                var found = otkaOwnerUsers.FirstOrDefault(r => r.Id == rep.OktaId);
                rep.Profile = found?.Profile;
                //todo: will need to tie into existing ABKC system to pull owner information
            }
            return mapped;
        }
        /// <summary>
        /// returns all active ABKC users in the system, available to admins and Office workers
        /// </summary>
        /// <returns></returns>
        [HttpGet("active")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<ICollection<FullABKCUserDTO>>> GetActiveABKCUsers()
        {
            ICollection<FullABKCUserDTO> activeUsers = new List<FullABKCUserDTO>();
            ICollection<UserModel> allUsers = await _userService.GetAllUsers();
            ICollection<Okta.Sdk.IUser> otkaActiveUsers = await _oktaService.GetActiveUsers();

            ICollection<FullABKCUserDTO> mapped = _autoMapper.Map<ICollection<FullABKCUserDTO>>(allUsers);
            foreach (var rep in mapped)
            {
                var found = otkaActiveUsers.FirstOrDefault(r => r.Id == rep.OktaId);
                if (found != null)
                {
                    rep.Profile = found?.Profile;
                    activeUsers.Add(rep);
                }

                //todo: will need to tie into existing ABKC system to pull owner information
            }
            activeUsers = activeUsers.OrderBy(u => u.LoginName).ToList();
            return Ok(activeUsers);
        }

        /// <summary>
        /// Returns all user requests that have been submitted but not yet activated
        /// </summary>
        /// <returns>A list of potential users with the role requested and whatever profile info we have</returns>
        [HttpGet("pending")]
        public async Task<ICollection<FullABKCUserDTO>> GetPendingUserRegistrations()
        {
            ICollection<Okta.Sdk.IUser> users = await _oktaService.GetUnActivatedUsers();
            ICollection<FullABKCUserDTO> rtn = new List<FullABKCUserDTO>();
            if (users == null)
            {
                return rtn;
            }
            foreach (IUser u in users)
            {
                //find abkc user
                UserModel found = await _userService.GetUserFromOktaId(u.Id);
                if (found != null)
                {
                    FullABKCUserDTO dto = _autoMapper.Map<FullABKCUserDTO>(found);
                    dto.Profile = u?.Profile;
                    rtn.Add(dto);
                }
            }
            return rtn;
        }

        /// <summary>
        /// returns all suspended users in the system
        /// </summary>
        /// <returns></returns>
        [HttpGet("suspended")]
        public async Task<ICollection<FullABKCUserDTO>> SuspendedUsers()
        {
            ICollection<UserModel> suspended = await _userService.SuspendedUsers();
            ICollection<FullABKCUserDTO> mapped = _autoMapper.Map<ICollection<FullABKCUserDTO>>(suspended);
            ICollection<Okta.Sdk.IUser> oktaUsers = await _oktaService.GetSuspendedUsers();
            foreach (var rep in mapped)
            {
                var found = oktaUsers.FirstOrDefault(r => r.Id == rep.OktaId);
                rep.Profile = found?.Profile;
                //todo: will need to tie into existing ABKC system to pull owner information
            }
            return mapped;
        }

        /// <summary>
        /// will allow an office worker to associate a user account to an ABKC Owner database entry (pre-existing)
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="owner">owner information to update</param>
        /// <returns></returns>
        [HttpPut("owner/{id}/update")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<OwnerUserAccountDTO>> UpdateOwnerAccount(int id, OwnerUserAccountDTO owner)
        {
            return BadRequest("Owner Account Update Not Implemented");
        }

        /// <summary>
        /// Updates the Registration fees for a representative
        /// </summary>
        /// <param name="id">ABKC Id</param>
        /// <param name="updatedFees"></param>
        /// <returns></returns>
        [HttpPut("representatives/{id}/updateFees")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<bool>> UpdateRepFees(int id, [FromBody]RepFeesDTO updatedFees)
        {
            try
            {
                RepresentativeModel updated = await _userService.UpdateRepresentativeFees(id, updatedFees.PedigreeRegistrationFee,
                    updatedFees.LitterRegistrationFee, updatedFees.PuppyRegistrationFee, updatedFees.BullyIdRequestFee,
                     updatedFees.JrHandlerRegistrationFee, updatedFees.TransferFee);
                if (updated == null)
                {
                    return BadRequest($"Representative for user id {id} could not be updated");
                }
                // RepresentativeDTO mapped = _autoMapper.Map<RepresentativeDTO>(updated);
                return Ok(true);
            }
            catch (Exception x)
            {
                //log
                return BadRequest(x.Message);
            }
        }

        /// <summary>
        /// a direct way to get the representative fees for a rep by id
        /// NOTE: this data is included in a representative fetch as well
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("representatives/{id}/fees")]
        public async Task<ActionResult<RepFeesDTO>> GetRepFees(int id)
        {
            RepresentativeModel rep = await _userService.GetRepresentativeFromABKCId(id);
            if (rep == null)
            {
                return NotFound($"No representative with id {id} could be found");
            }
            return Ok(new RepFeesDTO
            {
                BullyIdRequestFee = rep.BullyIdRequestFee,
                JrHandlerRegistrationFee = rep.JrHandlerRegistrationFee,
                LitterRegistrationFee = rep.LitterRegistrationFee,
                PedigreeRegistrationFee = rep.PedigreeRegistrationFee,
                PuppyRegistrationFee = rep.PuppyRegistrationFee,
                TransferFee = rep.TransferFee
            });
        }

        [HttpPost("{id}/resetpassword")]
        public async Task<ActionResult<bool>> ResetUserPassword(int id)
        {
            UserModel abkcUser = await _userService.GetUserFromABKCId(id);
            if (abkcUser == null)
            {
                return BadRequest($"ABKC User could not be retrieved for ABKC Id {id}");
            }
            var user = await _oktaService.GetUserFromOkta(abkcUser.OktaId);
            if (user == null)
            {
                return BadRequest($"Okta user with id {abkcUser.OktaId} is not in the remote system.");
            }
            try
            {
                bool success = await _oktaService.ResetPassword(user);
                return Ok(success);
            }
            catch (Exception e)
            {
                //probably already activated
                return BadRequest($"trouble activating user {id} because {e.Message}");
                // throw e;
            }
        }



        [HttpGet("resendActivationEmail")]
        public async Task<ActionResult<bool>> ResendActivationEmailForLogin(string emailAddress)
        {

            UserModel abkcUser = await _userService.GetUserFromOktaLogin(emailAddress);
            if (abkcUser == null)
            {
                return BadRequest($"ABKC User could not be retrieved for ABKC Login {emailAddress}");
            }
            var user = await _oktaService.GetUserFromOkta(abkcUser.OktaId);
            if (user == null)
            {
                return BadRequest($"Okta user with id {abkcUser.OktaId} is not in the remote system.");
            }
            try
            {
                bool success = await _oktaService.ResendActivationEmail(user);
                return Ok(success);
            }
            catch (Exception e)
            {
                //probably already activated
                return BadRequest($"trouble sending activation email for user {emailAddress} because {e.Message}");
                // throw e;
            }
        }
        [HttpGet("{id}/resendActivationEmail")]
        public async Task<ActionResult<bool>> ResendActivationEmailForId(int id)
        {

            UserModel abkcUser = await _userService.GetUserFromABKCId(id);
            if (abkcUser == null)
            {
                return BadRequest($"ABKC User could not be retrieved for Id {id}");
            }
            var user = await _oktaService.GetUserFromOkta(abkcUser.OktaId);
            if (user == null)
            {
                return BadRequest($"Okta user with id {abkcUser.OktaId} is not in the remote system.");
            }
            try
            {
                bool success = await _oktaService.ResendActivationEmail(user);
                return Ok(success);
            }
            catch (Exception e)
            {
                //probably already activated
                return BadRequest($"trouble sending activation email for user id {id} because {e.Message}");
                // throw e;
            }
        }
    }
    public class RepFeesDTO
    {
        public double PedigreeRegistrationFee { get; set; }
        public double LitterRegistrationFee { get; set; }
        public double PuppyRegistrationFee { get; set; }
        public double BullyIdRequestFee { get; set; }
        public double JrHandlerRegistrationFee { get; set; }
        public double TransferFee { get; set; }
    }
}