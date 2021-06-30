using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreDAL.Models
{
    public class Judges
    {
        [Required]
        public Int32 Id { get; set; }

        [MaxLength(50)]
        public String FirstName { get; set; }

        [MaxLength(50)]
        public String LastName { get; set; }

        public Boolean IsActive { get; set; }

        public DateTime? InActiveDate { get; set; }

    }

}
