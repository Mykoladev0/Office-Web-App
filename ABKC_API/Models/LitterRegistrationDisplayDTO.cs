using System;
using System.Collections.Generic;
using ABKCCommon.Models.DTOs;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using CoreDAL.Models.v2.Registrations;

namespace CoreApp.Models
{
    /// <summary>
    /// For retrieval of litter information to display to the clients
    /// </summary>
    public class LitterRegistrationDisplayDTO
    {
        public LitterRegistrationDisplayDTO()
        {
            DocumentTypesProvided = new List<SupportingDocumentTypeEnum>();
            RegistrationType = RegistrationTypeEnum.Litter;
        }

        public int Id { get; set; }
        public DogInfoDTO SireInfo { get; set; }
        public DogInfoDTO DamInfo { get; set; }
        public DateTime? DateOfBreeding { get; set; }
        public DateTime? DateOfLitterBirth { get; set; }
        public Breeds Breed { get; set; }
        public int NumberOfMalesBeingRegistered { get; set; }
        public int NumberOfFemalesBeingRegistered { get; set; }
        public bool FrozenSemenUsed { get; set; }
        public DateTime? DateSemenCollected { get; set; }


        public string RegistrationStatus { get; set; }
        public ABKCUserDTO SubmittedBy { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public RegistrationTypeEnum RegistrationType { get; private set; }

        public bool IsInternationalRegistration { get; set; }
        public bool OvernightRequested { get; set; }
        public bool RushRequested { get; set; }
        public ICollection<SupportingDocumentTypeEnum> DocumentTypesProvided { get; set; }

    }
}