using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class Shows
    {
        public Shows()
        {
            ShowJudges = new List<Judges>();
        }
        [Key]
        public int ShowId { get; set; }
        [StringLength(100)]
        public string ShowName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ShowDate { get; set; }
        [StringLength(35)]
        public string Address { get; set; }
        [StringLength(25)]
        public string City { get; set; }
        [StringLength(3)]
        public string State { get; set; }
        [StringLength(10)]
        public string Zip { get; set; }
        public bool? International { get; set; }
        [StringLength(50)]
        public string InsurancePolicy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InsuranceExpires { get; set; }
        public int? EntriesAllowed { get; set; }
        public int? ClubId { get; set; }
        public bool? FirstShow { get; set; }
        public bool? AppRecvd { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateApproved { get; set; }
        [Column(TypeName = "decimal(6, 3)")]
        public decimal? FeePaid { get; set; }
        [Column("ABKCHosted")]
        public bool? Abkchosted { get; set; }
        [StringLength(50)]
        public string Judge1 { get; set; }
        [StringLength(50)]
        public string Judge2 { get; set; }
        [StringLength(50)]
        public string RingSteward { get; set; }
        [StringLength(50)]
        public string Paperwork1 { get; set; }
        [StringLength(50)]
        public string Paperwork2 { get; set; }
        [Column("ABKCRep")]
        [StringLength(50)]
        public string Abkcrep { get; set; }
        public bool? AuthLetterRecvd { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateLetterRecvd { get; set; }
        [StringLength(150)]
        public string BreedsShown { get; set; }
        [StringLength(50)]
        public string StylesShown { get; set; }
        public bool? ClassesClosed { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifyDate { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        [StringLength(250)]
        public string Comments { get; set; }
        public int? JudgeId { get; set; }

        public DateTime? FinalizedDate { get; set; }
        

        [NotMapped]
        public ICollection<Judges> ShowJudges { get; set; }
    }
}
