using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreDAL.Models
{
    public class Styles
    {
        [Required]
        public Int32 Id { get; set; }

        [MaxLength(20)]
        public String StyleName { get; set; }

    }
}
