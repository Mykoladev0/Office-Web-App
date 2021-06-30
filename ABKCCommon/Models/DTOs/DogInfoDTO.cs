using System;
using System.Collections.Generic;

namespace ABKCCommon.Models.DTOs
{
    public class DogInfoDTO
    {
        public int Id { get; set; }
        public string DogName { get; set; }
        public string Breed { get; set; }
        public string Color { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string MicrochipNumber { get; set; }
        public string MicrochipType { get; set; }
        public OwnerDTO Owner { get; set; }
        public OwnerDTO CoOwner { get; set; }
        public DogInfoDTO Sire { get; set; }
        public DogInfoDTO Dam { get; set; }

        public string ABKCNumber { get; set; }

    }
}