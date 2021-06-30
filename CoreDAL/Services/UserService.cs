using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDAL.Interfaces;
using CoreDAL.Models.v2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoreDAL.Services
{
    public class UserService : IABKCUserService
    {
        private readonly ABKCOnlineContext _context;
        private readonly IConfiguration _appSettings;

        public UserService(ABKCOnlineContext context, IConfiguration appSettings)
        {
            _context = context;
            _appSettings = appSettings;
        }
        public async Task<UserModel> GetUserFromOktaId(string oktaId) => await _context.ABKCUsers.Include(u => u.Roles).FirstOrDefaultAsync(u => u.OktaId == oktaId);
        public async Task<UserModel> GetUserFromOktaLogin(string login) => await _context.ABKCUsers.Include(u => u.Roles).FirstOrDefaultAsync(u => u.LoginName == login);

        public async Task<bool> RemoveUserAccount(UserModel abkcUser)
        {
            try
            {
                IQueryable<UserModel> found = _context.ABKCUsers.Where(u => u.OktaId == abkcUser.OktaId || u.LoginName == abkcUser.LoginName);
                if (!found.Any())
                {
                    throw new InvalidOperationException("No User already exists in system, cannot remove");
                }
                UserModel user = await found.Include(u => u.Roles).FirstAsync();
                _context.RemoveRange(user.Roles);
                _context.ABKCUsers.Remove(user);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                //log exception
                return false;
            }
            return true;
        }

        public async Task<bool> SuspendAccount(UserModel abkcUser)
        {
            try
            {
                IQueryable<UserModel> found = _context.ABKCUsers.Where(u => u.OktaId == abkcUser.OktaId || u.LoginName == abkcUser.LoginName);
                if (!found.Any())
                {
                    throw new InvalidOperationException("No User already exists in system, cannot suspend");
                }
                UserModel user = await found.Include(u => u.Roles).FirstAsync();
                user.IsSuspended = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                //log exception
                return false;
            }
            return true;
        }
        public async Task<bool> UnSuspendAccount(UserModel abkcUser)
        {
            try
            {
                IQueryable<UserModel> found = _context.ABKCUsers.Where(u => u.OktaId == abkcUser.OktaId || u.LoginName == abkcUser.LoginName);
                if (!found.Any())
                {
                    throw new InvalidOperationException("No User already exists in system, cannot un-suspend");
                }
                UserModel user = await found.Include(u => u.Roles).FirstAsync();
                user.IsSuspended = false;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                //log exception
                return false;
            }
            return true;
        }

        public async Task<UserModel> AddUser(string oktaId, string login)
        {
            if (string.IsNullOrEmpty(oktaId))
            {
                throw new InvalidOperationException("Okta Id must be provided");
            }
            if (string.IsNullOrEmpty(login))
            {
                throw new InvalidOperationException("Okta Login must be provided");
            }
            //todo: removing this temporarily to seed registrations
            IQueryable<UserModel> found = _context.ABKCUsers.Where(u => u.OktaId == oktaId || u.LoginName == login);
            if (found.Any())
            {
                throw new InvalidOperationException("User already exists in system, cannot add");

            }
            UserModel user = new UserModel
            {
                OktaId = oktaId,
                LoginName = login
            };
            await _context.ABKCUsers.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public Task<UserModel> GetUserFromABKCId(int id) => _context.ABKCUsers.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);

        public async Task<ICollection<UserModel>> GetByRole(SystemRoleEnum role)
        {
            ICollection<UserModel> found = await _context.ABKCUsers.Include(u => u.Roles).Where(u => u.Roles.Any(r => r.Type == role)).ToListAsync();
            return found;

        }

        public async Task<bool> AddUserToRole(UserModel user, SystemRoleEnum role)
        {
            bool rtnVal = false;
            _context.Attach(user);
            if (!user.Roles.Any(r => r.Type == role))
            {
                user.Roles.Add(new RoleType
                {
                    Type = role
                });
                await _context.SaveChangesAsync();
                rtnVal = true;
            }
            //check to see if they are a rep, if so, verify account exists
            if (role != SystemRoleEnum.Representative)
            {
                return rtnVal;
            }
            RepresentativeModel rep = await GetRepresentativeFromABKCId(user.Id);
            if (rep == null)
            {
                var fees = _appSettings.GetSection("RepresentativeFees");
                var childs = fees.GetChildren();
                await AddRepresentativeFromABKCId(user,
                    fees.GetValue<double>("Pedigree"), fees.GetValue<double>("Litter"),
                    fees.GetValue<double>("Puppy"), fees.GetValue<double>("BullyID"),
                    fees.GetValue<double>("JrHandler"), fees.GetValue<double>("Transfer")
                );
            }
            //already has role, return false?
            return rtnVal;
        }

        public async Task<bool> RemoveUserFromRole(UserModel user, SystemRoleEnum role)
        {
            _context.Attach(user);
            if (user.Roles.Any(r => r.Type == role))
            {
                var foundRole = user.Roles.FirstOrDefault(r => r.Type == role);
                if (foundRole != null)
                {
                    if (user.Roles.Remove(foundRole))
                    {
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new InvalidOperationException($"There was a problem removing user {user.Id} from role {role}.");
                    }
                }


            }
            //check to see if they are a rep, if so, remove
            if (role != SystemRoleEnum.Representative)
            {
                return true;
            }
            RepresentativeModel rep = await GetRepresentativeFromABKCId(user.Id);
            if (rep != null)
            {
                _context.Representatives.Remove(rep);
                await _context.SaveChangesAsync();

            }
            //already has role, return false?
            return true;
        }

        /// <summary>
        /// tries to find corresponding representative record (for fees) from a user
        /// </summary>
        /// <param name="id">ABKC User Id</param>
        /// <returns>null if not found, otherwise a representative mode</returns>
        public async Task<RepresentativeModel> GetRepresentativeFromABKCId(int id)
        {
            RepresentativeModel found = await _context.Representatives.FirstOrDefaultAsync(r => r.UserRecord != null && r.UserRecord.Id == id);
            return found;
        }

        /// <summary>
        /// Adds a representative specific user to the system with the rates charged for each registration
        /// </summary>
        /// <param name="abkcUser">ABKC User to associate</param>
        /// <param name="pedigreeRegistrationFee"></param>
        /// <param name="litterRegistrationFee"></param>
        /// <param name="puppyRegistrationFee"></param>
        /// <param name="bullyIdRequestFee"></param>
        /// <param name="jrHandlerRegistrationFee"></param>
        /// <param name="transferFee"></param>
        /// <returns></returns>
        public async Task<RepresentativeModel> AddRepresentativeFromABKCId(UserModel abkcUser, double pedigreeRegistrationFee,
            double litterRegistrationFee, double puppyRegistrationFee,
            double bullyIdRequestFee, double jrHandlerRegistrationFee, double transferFee)
        {
            RepresentativeModel found = await GetRepresentativeFromABKCId(abkcUser.Id);
            if (found != null)
            {
                return found;
            }
            found = new RepresentativeModel
            {
                UserRecord = abkcUser,
                PedigreeRegistrationFee = pedigreeRegistrationFee,
                PuppyRegistrationFee = puppyRegistrationFee,
                BullyIdRequestFee = bullyIdRequestFee,
                TransferFee = transferFee,
                JrHandlerRegistrationFee = jrHandlerRegistrationFee,
                LitterRegistrationFee = litterRegistrationFee
            };
            _context.Representatives.Add(found);
            await _context.SaveChangesAsync();
            return found;
        }

        /// <summary>
        /// Updates an existing representative's rates in the system.
        /// If rep is not found, null is returned
        /// </summary>
        /// <param name="userId">ABKC Id</param>
        /// <param name="pedigreeRegistrationFee"></param>
        /// <param name="litterRegistrationFee"></param>
        /// <param name="puppyRegistrationFee"></param>
        /// <param name="bullyIdRequestFee"></param>
        /// <param name="jrHandlerRegistrationFee"></param>
        /// <param name="transferFee"></param>
        /// <returns></returns>
        public async Task<RepresentativeModel> UpdateRepresentativeFees(int userId, double pedigreeRegistrationFee,
            double litterRegistrationFee, double puppyRegistrationFee,
            double bullyIdRequestFee, double jrHandlerRegistrationFee, double transferFee)
        {
            RepresentativeModel found = await GetRepresentativeFromABKCId(userId);
            if (found == null)
            {
                return null;
            }
            found.PedigreeRegistrationFee = pedigreeRegistrationFee;
            found.PuppyRegistrationFee = puppyRegistrationFee;
            found.LitterRegistrationFee = litterRegistrationFee;
            found.JrHandlerRegistrationFee = jrHandlerRegistrationFee;
            found.BullyIdRequestFee = bullyIdRequestFee;
            found.TransferFee = transferFee;
            await _context.SaveChangesAsync();
            return found;
        }

        public async Task AddStripeCustomerId(int userId, string stripeCustomerId)
        {
            UserModel found = await _context.ABKCUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (found == null)
            {
                return;
            }
            found.StripeCustomerId = stripeCustomerId;
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<ICollection<UserModel>> GetAllUsers() => await _context.ABKCUsers.Include(u => u.Roles).ToListAsync();
        public async Task<ICollection<UserModel>> SuspendedUsers() => await _context.ABKCUsers.Include(u => u.Roles).Where(u => u.IsSuspended).ToListAsync();
    }
}