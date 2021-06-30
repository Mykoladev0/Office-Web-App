using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models.v2
{
    public class BaseDBModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DateModified { get; set; }
    }
}