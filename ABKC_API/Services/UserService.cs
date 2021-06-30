using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoreApp.Interfaces;
using CoreApp.Models;
using CoreDAL.Models.v2;
using Microsoft.Extensions.Configuration;
using Okta.Sdk;

namespace CoreApp.Services
{
    public class UserService : IOktaUserService
    {
        private readonly IOktaClient _client;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public const string DUMMYOKTAID = "DUMMY";

        public UserService(HttpClient httpClient, IOktaClient client, IConfiguration config)
        {
            _client = client;
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<bool> ActivateUser(IUser user)
        {
            try
            {
                await _client.Users.ActivateUserAsync(user.Id, true);
                return true;
            }
            catch (Exception e)
            {
                //TODO:log error!
                throw (e);
            }
            return false;
        }

        public async Task<bool> ResendActivationEmail(IUser user)
        {
            if (user.Status == "PROVISIONED")
            {
                await _client.PostAsync(new Okta.Sdk.HttpRequest
                {
                    Uri = $"/api/v1/users/{user.Id}/lifecycle/reactivate",
                    PathParameters = new Dictionary<string, object>()
                    {
                        ["userId"] = user.Id,
                    },
                    QueryParameters = new Dictionary<string, object>()
                    {
                        ["sendEmail"] = true,
                    }
                });
                return true;
            }
            else
            {
                throw new InvalidOperationException("User {user.login} is not provisioned, so no activation email can be sent");
            }
        }

        public async Task<bool> RemoveUserAccount(IUser user)
        {
            try
            {
                //do it twice to delete
                await _client.Users.DeactivateOrDeleteUserAsync(user.Id, false);
                await _client.Users.DeactivateOrDeleteUserAsync(user.Id, false);
                return true;
            }
            catch (Exception e)
            {
                //TODO:log error!
                throw (e);
            }
            return false;
        }

        public async Task<bool> SuspendUserAccount(IUser user)
        {
            try
            {
                await _client.Users.SuspendUserAsync(user.Id);
                return true;
            }
            catch (Exception e)
            {
                //TODO:log error!
                throw (e);
            }
            return false;
        }
        public async Task<bool> UnSuspendUserAccount(IUser user)
        {
            try
            {
                await _client.Users.UnsuspendUserAsync(user.Id);
                return true;
            }
            catch (Exception e)
            {
                //TODO:log error!
                throw (e);
            }
            return false;
        }
        public async Task<bool> AddUserToRole(string oktaId, SystemRoleEnum role)
        {
            if (oktaId == DUMMYOKTAID)
            {
                return true;
            }
            IGroup group = await GetGroup(role);
            var user = await _client.Users.FirstOrDefault(u => u.Id == oktaId);
            if (group != null && user != null)
            {
                await _client.Groups.AddUserToGroupAsync(group.Id, user.Id);
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveUserFromRole(string oktaId, SystemRoleEnum role)
        {
            if (oktaId == DUMMYOKTAID)
            {
                return true;
            }
            IGroup group = await GetGroup(role);
            var user = await _client.Users.FirstOrDefault(u => u.Id == oktaId);
            if (group != null && user != null)
            {
                await _client.Groups.RemoveGroupUserAsync(group.Id, user.Id);
                return true;
            }
            return false;
        }

        private async Task<IGroup> GetGroup(SystemRoleEnum role)
        {

            string searchStr = "";
            switch (role)
            {
                case SystemRoleEnum.Representative:
                    searchStr = "Representatives";
                    break;
                case SystemRoleEnum.ABKCOffice:
                    searchStr = "ABKCOffice";
                    break;
                case SystemRoleEnum.Owner:
                    searchStr = "Owners";
                    break;
                case SystemRoleEnum.Judge:
                    searchStr = "Judges";
                    break;
            }
            if (searchStr == null)
            {
                throw new InvalidOperationException($"No matching group could be found or created for role {role.ToString()}");
            }
            var found = await _client.Groups.ListGroups(searchStr).FirstOrDefault();
            if (found == null)
            {
                //create it!
                found = await _client.Groups.CreateGroupAsync(new CreateGroupOptions
                {
                    Name = searchStr,
                    Description = "Added by ABKC system software",

                });
            }
            return found;
        }
        public async Task<ICollection<IUser>> GetByRole(SystemRoleEnum role)
        {
            IGroup group = await GetGroup(role);
            if (group != null)
            {
                var tmp = await group.Users.ToList();
                return tmp;
            }
            return new List<IUser>();
        }

        public async Task<IUser> GetUserFromOkta(string id)
        {
            if (id == DUMMYOKTAID)
            {
                return new Okta.Sdk.User
                {
                    Profile = new UserProfile
                    {
                        Email = "dummy@dummy.com",
                        Login = "dummy@dummy.com",
                        FirstName = "dummy@dummy.com",
                        LastName = "dummy@dummy.com",
                        MobilePhone = "1-555-555-5555",
                        SecondEmail = "dummy2@dummy.com"
                    }
                };
            }
            if (!string.IsNullOrEmpty(id))
            {
                IUser user = await _client.Users.GetUserAsync(id);
                return user;
            }
            return null;
        }
        public async Task<IUser> RegisterUser(UserRegistrationModel registration)
        {
            IUser created = await _client.Users.CreateUserAsync(new CreateUserWithoutCredentialsOptions
            {
                Activate = false,
                Profile = new UserProfile
                {
                    Email = registration.EmailAddress,
                    Login = registration.EmailAddress,
                    FirstName = registration.FirstName,
                    LastName = registration.LastName,
                },
            });
            //user will receive an email when their account is activated
            if (created != null)
            {
                //add to correct group
                await AddUserToRole(created.Id, registration.RoleRequested);
            }
            return created;
        }

        public async Task<string> GetUserToken(string userName, string password)
        {

            // Populate the form variable
            var formVariables = new List<KeyValuePair<string, string>>();
            /*$client_id={client_id}&client_secret={client_secret}&grant_type=password&scope=openid&username={userName}&password={password}
             */
            formVariables.Add(new KeyValuePair<string, string>("client_id", _config["Okta:ClientId"]));
            formVariables.Add(new KeyValuePair<string, string>("client_secret", _config["Okta:ClientSecret"]));
            formVariables.Add(new KeyValuePair<string, string>("scope", "openid"));
            formVariables.Add(new KeyValuePair<string, string>("grant_type", "password"));
            formVariables.Add(new KeyValuePair<string, string>("username", userName));
            formVariables.Add(new KeyValuePair<string, string>("password", password));
            var formContent = new FormUrlEncodedContent(formVariables);

            _httpClient.DefaultRequestHeaders.Add("Content_Type", "application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync($"{_config["Okta:OktaDomain"]}/oauth2/default/v1/token", formContent);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error retrieving Token, {response.ReasonPhrase}");
            }
            var tokenObj = new
            {
                token_type = "",
                access_token = "",
                id_token = ""
            };
            var responseToken = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), tokenObj);
            return responseToken.access_token;
            // return responseToken.id_token;
        }

        public async Task<ICollection<IUser>> GetUnActivatedUsers()
        {
            return await _client.Users.ListUsers(filter: $"status eq \"STAGED\"").ToList();
        }
        public async Task<ICollection<IUser>> GetActiveUsers()
        {
            var found = await _client.Users.ListUsers(filter: $"status eq \"ACTIVE\"").ToList();
            return found ?? new List<IUser>();
        }
        public async Task<ICollection<IUser>> GetSuspendedUsers()
        {
            var found = await _client.Users.ListUsers(filter: $"status eq \"SUSPENDED\"").ToList();
            return found ?? new List<IUser>();
        }

        public async Task<IUser> GetUserFromLogin(string emailAddress)
        {
            try
            {
                var found = await _client.Users.GetUserAsync(emailAddress);
                return found;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //do something with it?
            }
            return null;
        }

        public async Task<bool> ResetPassword(IUser user)
        {
            var result = await _client.Users.ResetPasswordAsync(user.Id, true);
            return result != null;

        }
    }
}