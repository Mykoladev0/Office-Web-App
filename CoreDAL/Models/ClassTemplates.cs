using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoreDAL.Models
{
    public class ClassTemplates
    {
        [Required]
        [Key]
        [Column("ClassId")]
        public Int32 ClassId { get; set; }

        [Required]
        public Int32 StyleId { get; set; }

        [MaxLength(50)]
        public String Name { get; set; }

        public Int32 Points { get; set; }

        public Boolean ChampWin { get; set; }

        public Int32 ChampPoints { get; set; }

        public Int32 SortOrder { get; set; }

        public string Gender { get; set; }

        [NotMapped]
        public virtual Styles Style { get; set;}
        [NotMapped]
        public bool IsBestOf { get
            {
                if (Name.ToLower().Contains("best"))
                {
                    return true;
                }
                return false;
            }
        }

    }
}
