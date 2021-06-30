using System.ComponentModel.DataAnnotations;
using CoreDAL.Models.v2;

namespace CoreApp.Models
{
    public class UserRegistrationModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        public SystemRoleEnum RoleRequested { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}