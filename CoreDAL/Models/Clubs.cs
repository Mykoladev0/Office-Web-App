using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class Clubs
    {
        public int Id { get; set; }
        [Key]
        public int ClubId { get; set; }
        [StringLength(50)]
        public string ClubName { get; set; }
        [StringLength(35)]
        public string Address1 { get; set; }
        [StringLength(35)]
        public string Address2 { get; set; }
        [StringLength(35)]
        public string Address3 { get; set; }
        [StringLength(25)]
        public string City { get; set; }
        [StringLength(3)]
        public string State { get; set; }
        [StringLength(10)]
        public string Zip { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        public bool? International { get; set; }
        [StringLength(50)]
        public string President { get; set; }
        [StringLength(50)]
        public string VicePresident { get; set; }
        [StringLength(15)]
        public string PresContact { get; set; }
        [Column("VPContact")]
        [StringLength(15)]
        public string Vpcontact { get; set; }
        [StringLength(50)]
        public string WebAddress { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [Column(TypeName = "decimal(6, 2)")]
        public decimal? DuesPaid { get; set; }
        public int? MemberCount { get; set; }
        public bool? GoodStanding { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifyDate { get; set; }
        [StringLength(250)]
        public string Comments { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
