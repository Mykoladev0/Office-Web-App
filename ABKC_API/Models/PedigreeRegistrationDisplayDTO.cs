using System;
using System.Collections.Generic;
using ABKCCommon.Models.DTOs;
using CoreDAL.Interfaces;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2.Registrations;

namespace CoreApp.Models
{
    public class PedigreeRegistrationDisplayDTO
    {
        public PedigreeRegistrationDisplayDTO()
        {
            DocumentTypesProvided = new List<SupportingDocumentTypeEnum>();
            RegistrationType = RegistrationTypeEnum.Pedigree;
        }

        public int Id { get; set; }
        public BaseDogDTO DogInfo { get; set; }
        public OwnerDTO Owner { get; set; }
        public OwnerDTO CoOwner { get; set; }
        public string RegistrationStatus { get; set; }
        public ABKCUserDTO SubmittedBy { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public RegistrationTypeEnum RegistrationType { get; private set; }

        public bool? OvernightRequested { get; set; }
        public bool? RushRequested { get; set; }
        public bool IsInternational { get; set; }

        public ICollection<SupportingDocumentTypeEnum> DocumentTypesProvided { get; set; }

    }
}