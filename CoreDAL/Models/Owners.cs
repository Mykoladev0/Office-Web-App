using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoreDAL.Models
{
    public partial class Owners
    {
        [Column("id")]
        public int Id { get; set; }
        [Key]
        [Column("Owner_Id")]
        public int OwnerId { get; set; }
        [Column("Last_Name")]
        [StringLength(30)]
        public string LastName { get; set; }
        [StringLength(12)]
        public string MiddleInitial { get; set; }
        [Column("First_Name")]
        [StringLength(20)]
        public string FirstName { get; set; }

        //[StringLength(50)]
        [NotMapped]
        public string FullName { get
            {
                List<string> names = new List<string> { FirstName??"", MiddleInitial ?? "", LastName??"" };
                string fullName = String.Join(" ", names.Where(n => !String.IsNullOrEmpty(n)));
                return fullName;
            }
        }
        
        [StringLength(50)]
        public string Address1 { get; set; }
        [StringLength(50)]
        public string Address2 { get; set; }
        [StringLength(50)]
        public string Address3 { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        //[StringLength(3)]
        public string State { get; set; }
        [StringLength(10)]
        public string Zip { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        public bool? International { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [StringLength(30)]
        public string Phone { get; set; }
        [StringLength(30)]
        public string Cell { get; set; }
        [Column("Dual_Signature")]
        public bool? DualSignature { get; set; }
        [StringLength(250)]
        public string Comments { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifyDate { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        [StringLength(20)]
        public string WebPassword { get; set; }
    }
}
