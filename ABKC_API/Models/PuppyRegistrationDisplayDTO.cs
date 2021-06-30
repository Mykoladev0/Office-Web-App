using System;
using System.Collections.Generic;
using ABKCCommon.Models.DTOs;
using CoreDAL.Interfaces;
using CoreDAL.Models.v2.Registrations;

namespace CoreApp.Models
{
    /// <summary>
    /// For retrieval of puppy information to display to the clients
    /// Also is used for Transfer Requests
    /// </summary>
    public class PuppyRegistrationDisplayDTO
    {
        public PuppyRegistrationDisplayDTO()
        {
            DocumentTypesProvided = new List<SupportingDocumentTypeEnum>();
            RegistrationType = RegistrationTypeEnum.Puppy;
        }

        public int Id { get; set; }
        public string PuppyABKCNumber { get; set; }
        public DogInfoDTO DogInfo { get; set; }

        public OwnerDTO NewOwner { get; set; }
        public OwnerDTO NewCoOwner { get; set; }
        public DateTime? SellDate { get; set; }
        public string RegistrationStatus { get; set; }
        public ABKCUserDTO SubmittedBy { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public RegistrationTypeEnum RegistrationType { get; private set; }

        /// <summary>
        /// Puppy registrations can be used for transfers
        /// only difference is the name won't be changed
        /// </summary>
        /// <value></value>
        public bool IsTransferRequest { get; set; }

        public bool OvernightRequested { get; set; }
        public bool RushRequested { get; set; }
        public ICollection<SupportingDocumentTypeEnum> DocumentTypesProvided { get; set; }

    }
}