using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class JrHandlers
    {
        [Column("id")]
        public int Id { get; set; }
        [Key]
        public int JrHandlerId { get; set; }
        [Required]
        [StringLength(30)]
        public string ChildFirstName { get; set; }
        [Required]
        [StringLength(30)]
        public string ChildLastName { get; set; }
        [StringLength(50)]
        public string ChildName { get; set; }
        [Column("DOB", TypeName = "datetime")]
        public DateTime? Dob { get; set; }
        [StringLength(30)]
        public string ParentLastName { get; set; }
        [StringLength(30)]
        public string ParentFirstName { get; set; }
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
        [StringLength(10)]
        public string Country { get; set; }
        public bool? International { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [StringLength(15)]
        public string Phone { get; set; }
        [StringLength(15)]
        public string Cell { get; set; }
        [StringLength(250)]
        public string Comments { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifyDate { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        public int? Points { get; set; }
    }
}
