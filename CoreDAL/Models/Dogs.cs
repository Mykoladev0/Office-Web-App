using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class Dogs
    {
        public int Id { get; set; }
        [Column("Bully_Id")]
        public int BullyId { get; set; }
        [Column("ABKC_No")]
        [StringLength(12)]
        public string AbkcNo { get; set; }
        [Column("UKC_No")]
        [StringLength(12)]
        public string UkcNo { get; set; }
        [Column("AKC_No")]
        [StringLength(12)]
        public string AkcNo { get; set; }
        [Column("ADBA_No")]
        [StringLength(12)]
        public string AdbaNo { get; set; }
        [Column("Other_No")]
        [StringLength(12)]
        public string OtherNo { get; set; }
        [StringLength(20)]
        public string RegNo { get; set; }
        [StringLength(20)]
        public string RegType { get; set; }
        [StringLength(50)]
        public string DogName { get; set; }
        [StringLength(25)]
        public string Title { get; set; }
        [StringLength(25)]
        public string Suffix { get; set; }
        [StringLength(30)]
        public string Breed { get; set; }
        [StringLength(10)]//changing to 10 to support Unknown
        public string Gender { get; set; }
        [StringLength(25)]
        public string Color { get; set; }
        [Column("Sire_No")]
        public int? SireNo { get; set; }
        [Column("Dam_No")]
        public int? DamNo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Birthdate { get; set; }
        [Column("Litter_No")]
        public int? LitterNo { get; set; }
        [Column("Owner_Id")]
        public int OwnerId { get; set; }
        [Column("CoOwner_Id")]
        public int? CoOwnerId { get; set; }
        [Column("Tatoo_No")]
        [StringLength(20)]
        public string TatooNo { get; set; }
        [Column("Chip_No")]
        [StringLength(30)]
        public string ChipNo { get; set; }
        public int? Points { get; set; }
        public int? ChampPoints { get; set; }
        public int? ChampWins { get; set; }
        [Column("First_Generation")]
        public bool FirstGeneration { get; set; }
        public bool Registered { get; set; }
        public bool Verified { get; set; }
        public bool Printed { get; set; }
        [StringLength(250)]
        public string Comments { get; set; }
        [Column("Date_Created", TypeName = "datetime")]
        public DateTime? DateCreated { get; set; }
        [Column("Date_Registered", TypeName = "datetime")]
        public DateTime? DateRegistered { get; set; }
        [Column("Reg_Amt", TypeName = "money")]
        public decimal? RegAmt { get; set; }
        [Column("Reg_User")]
        [StringLength(25)]
        public string RegUser { get; set; }
        public int? Pups { get; set; }
        [StringLength(20)]
        public string Degrees { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        [Column("BDNA")]
        [StringLength(20)]
        public string Bdna { get; set; }
        [Column("NW")]
        public bool? Nw { get; set; }
        [Column("CHCertPrinted")]
        public bool? ChcertPrinted { get; set; }
        public int? BreedingDamOwnerId { get; set; }
        public int? BreedingDamCoOwnerId { get; set; }
        public int? BreedingSireOwnerId { get; set; }
        public int? BreedingSireCoOwnerId { get; set; }
        [Column("WP_Points")]
        public int? WpPoints { get; set; }
        public bool? SaveBully { get; set; }
        [Column("SAB_Date", TypeName = "datetime")]
        public DateTime? SabDate { get; set; }
        [Column("NeedsNewCHCert")]
        public bool? NeedsNewChcert { get; set; }
        [Column("WP_ChampWins")]
        public int? WpChampWins { get; set; }
        [Column("NeedsNewWPCHCert")]
        public bool? NeedsNewWpchcert { get; set; }
        [Column("WP_Title")]
        [StringLength(25)]
        public string WpTitle { get; set; }
        public bool? N { get; set; }
        [Column("BB")]
        public bool? Bb { get; set; }
        public int? Majors { get; set; }
        public int? Judges { get; set; }
        [StringLength(25)]
        public string OrigTitle { get; set; }
        [Column("INW")]
        public bool? Inw { get; set; }
    }
}
