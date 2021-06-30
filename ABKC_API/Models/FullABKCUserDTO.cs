using System.Collections.Generic;
using ABKCCommon.Models.DTOs;

using CoreDAL.Models.v2;

namespace CoreApp.Models
{
    public class FullABKCUserDTO : ABKCUserDTO
    {
        public Okta.Sdk.IUserProfile Profile { get; set; }
        public ICollection<string> ABKCRolesUserBelongsTo { get; set; }
    }

    public class RepresentativeDTO : FullABKCUserDTO
    {
        public int RegistrationCount { get; set; }
        public int PendingRegistrationCount { get; set; }

        /// <summary>
        /// per dog registration fee (set by ABKC office)
        /// </summary>
        /// <value></value>        
        public double PedigreeRegistrationFee { get; set; }
        public double LitterRegistrationFee { get; set; }
        public double PuppyRegistrationFee { get; set; }
        public double BullyIdRequestFee { get; set; }
        public double JrHandlerRegistrationFee { get; set; }
        public double TransferFee { get; set; }

        public int RepresentativeId { get; set; }
    }

}