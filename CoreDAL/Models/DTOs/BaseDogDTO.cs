using System;
using System.Collections.Generic;

namespace CoreDAL.Models.DTOs
{
    public class BaseDogDTO
    {
        public int Id { get; set; }
        public string DogName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string MicrochipNumber { get; set; }
        public int BreedId { get; set; }
        public int ColorId { get; set; }
        public int OwnerId { get; set; }
        public int? CoOwnerId { get; set; }
        public int? SireId { get; set; }
        public int? DamId { get; set; }
    }
}