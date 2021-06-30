using System;
using ABKCCommon.Models.DTOs;

namespace CoreDAL.Models.DTOs
{
    /// <summary>
    /// used to PUT changes of a puppy registration to the server
    /// NOTE Dog and existing owner information cannot be changed with the exception of:
    /// Microchip and color information
    /// </summary>
    public class PuppyRegistrationDraftDTO : BaseDraftRegistrationSubmitDTO
    {
        public string DogName { get; set; }
        public int ColorId { get; set; }
        public string MicrochipNumber { get; set; }
        public string MicrochipType { get; set; }

        /// <summary>
        /// either a new owner entry or a reference to an existing owner (owner id >0)
        /// </summary>
        /// <value></value>
        public OwnerDTO NewOwner { get; set; }
        /// <summary>
        /// either a new owner entry or a reference to an existing owner (owner id >0)
        /// </summary>
        /// <value></value>

        public OwnerDTO NewCoOwner { get; set; }

        public DateTime? SellDate { get; set; }
    }
}