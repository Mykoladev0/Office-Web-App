using System;
using System.Collections.Generic;
using ABKCCommon.Models.DTOs;
using CoreDAL.Interfaces;
using CoreDAL.Models.v2.Registrations;

namespace CoreDAL.Models.DTOs
{
    public class PedigreeRegistrationDraftDTO : BaseDraftRegistrationSubmitDTO
    {
        public PedigreeRegistrationDraftDTO()
        {
        }
        public BaseDogDTO DogInfo { get; set; }
        public OwnerDTO Owner { get; set; }
        public OwnerDTO CoOwner { get; set; }

    }
}