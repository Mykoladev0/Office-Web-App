using System;
using System.Collections.Generic;
using ABKCCommon.Models.DTOs;
using CoreDAL.Interfaces;
using CoreDAL.Models.v2.Registrations;

namespace CoreApp.Models
{
    /// <summary>
    /// For retrieval of bully Id requests to display to the clients
    /// </summary>
    public class BullyIdRequestDisplayDTO
    {
        public BullyIdRequestDisplayDTO()
        {
            DocumentTypesProvided = new List<SupportingDocumentTypeEnum>();
            RegistrationType = RegistrationTypeEnum.BullyId;
        }

        public int Id { get; set; }
        public DogInfoDTO DogInfo { get; set; }
        public RegistrationTypeEnum RegistrationType { get; private set; }

        public bool OvernightRequested { get; set; }
        public bool RushRequested { get; set; }
        public ICollection<SupportingDocumentTypeEnum> DocumentTypesProvided { get; set; }

    }
}