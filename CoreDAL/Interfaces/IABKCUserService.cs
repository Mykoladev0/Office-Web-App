using CoreDAL.Models;
using CoreDAL.Models.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreDAL.Interfaces
{
    public interface IABKCUserService
    {
        Task<UserModel> GetUserFromOktaId(string oktaId);
        Task<UserModel> GetUserFromOktaLogin(string login);
        Task<UserModel> AddUser(string oktaId, string login);
        Task<UserModel> GetUserFromABKCId(int id);
        Task<ICollection<UserModel>> GetByRole(SystemRoleEnum role);
        Task<bool> AddUserToRole(UserModel user, SystemRoleEnum role);
        Task<bool> RemoveUserFromRole(UserModel user, SystemRoleEnum role);
        Task<bool> RemoveUserAccount(UserModel abkcUser);

        /// <summary>
        /// tries to find corresponding representative record (for fees) from a user
        /// </summary>
        /// <param name="id">ABKC User Id</param>
        /// <returns>null if not found, otherwise a representative mode</returns>
        Task<RepresentativeModel> GetRepresentativeFromABKCId(int id);

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
        Task<RepresentativeModel> AddRepresentativeFromABKCId(UserModel abkcUser, double pedigreeRegistrationFee,
            double litterRegistrationFee, double puppyRegistrationFee,
            double bullyIdRequestFee, double jrHandlerRegistrationFee, double transferFee);

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
        Task<RepresentativeModel> UpdateRepresentativeFees(int userId, double pedigreeRegistrationFee,
            double litterRegistrationFee, double puppyRegistrationFee,
            double bullyIdRequestFee, double jrHandlerRegistrationFee, double transferFee);
        Task AddStripeCustomerId(int userId, string stripeCustomerId);

        Task<ICollection<UserModel>> GetAllUsers();
        /// <summary>
        /// will mark user as suspended. The registrations associated can still be viewed by office, but user cannot log in
        /// </summary>
        /// <param name="abkcUser"></param>
        /// <returns></returns>
        Task<bool> SuspendAccount(UserModel abkcUser);
        Task<bool> UnSuspendAccount(UserModel abkcUser);
        Task<ICollection<UserModel>> SuspendedUsers();
    }
}