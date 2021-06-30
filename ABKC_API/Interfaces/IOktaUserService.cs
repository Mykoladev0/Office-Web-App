using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDAL.Models.v2;
using Okta.Sdk;

namespace CoreApp.Interfaces
{
    public interface IOktaUserService
    {
        Task<string> GetUserToken(string userName, string password);
        Task<Okta.Sdk.IUser> GetUserFromOkta(string id);
        Task<bool> ActivateUser(IUser user);
        Task<ICollection<IUser>> GetByRole(SystemRoleEnum role);
        Task<bool> AddUserToRole(string oktaId, SystemRoleEnum role);
        Task<bool> RemoveUserFromRole(string oktaId, SystemRoleEnum role);
        Task<ICollection<IUser>> GetUnActivatedUsers();
        Task<IUser> GetUserFromLogin(string emailAddress);
        Task<IUser> RegisterUser(Models.UserRegistrationModel registration);
        Task<bool> RemoveUserAccount(IUser user);
        Task<bool> SuspendUserAccount(IUser user);
        Task<bool> UnSuspendUserAccount(IUser user);

        Task<bool> ResendActivationEmail(IUser user);
        Task<ICollection<IUser>> GetActiveUsers();
        Task<ICollection<IUser>> GetSuspendedUsers();
        /// <summary>
        ///  send email to user to reset
        /// </summary>
        /// <param name="user">User to reset password</param>
        /// <returns></returns>
        Task<bool> ResetPassword(IUser user);
    }
}