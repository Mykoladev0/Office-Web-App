using System.Collections.Generic;

namespace CoreDAL.Models.v2
{

    public enum SystemRoleEnum
    {
        Owner = 1,
        Representative = 2,
        Judge = 3,
        ABKCOffice = 4,
        Administrator = 5
    }

    /// <summary>
    /// Base model for all system users. Ties to an OKTA account
    /// </summary>
    public class UserModel : BaseDBModel
    {
        public UserModel()
        {
            Roles = new List<RoleType>();
        }

        public string OktaId { get; set; }
        public string LoginName { get; set; }
        public string StripeCustomerId { get; set; }
        public ICollection<RoleType> Roles { get; set; }
        public bool IsSuspended { get; set; }
    }
    public class RoleType
    {
        public int Id { get; set; }
        public SystemRoleEnum Type { get; set; }
    }

    // public class UserRoleJoinModel{

    //     public int UserId { get; set; }
    //     public virtual UserModel User { get; set; }
    //     public int ColorId { get; set; }
    //     public virtual Colors Colors { get; set; }
    // }
}