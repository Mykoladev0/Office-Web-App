using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class Litters
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("Litter_Id")]
        public int LitterId { get; set; }
        [Column("Owner_id")]
        public int OwnerId { get; set; }
        [Column("Dam_No")]
        public int DamNo { get; set; }
        [Column("Sire_No")]
        public int SireNo { get; set; }
        public int? Males { get; set; }
        public int? Females { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Birthdate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? BreedingDate { get; set; }
        [StringLength(50)]
        public string Breed { get; set; }
        [StringLength(250)]
        public string Comments { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifyDate { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        [Column("CoOwner_Id")]
        public int? CoOwnerId { get; set; }
        public int? SireOwnerId { get; set; }
        public int? SireCoOwnerId { get; set; }
    }
}
