using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoreDAL.Models
{
    public class ShowResults
    {
        [Required]
        public Int32 Id { get; set; }

        [Required]
        public Int32 ShowId { get; set; }

        [Required]
        [MaxLength(30)]
        public String Breed { get; set; }

        [JsonIgnore]
        [MaxLength(20)]
        public String Style { get; set; }

        [JsonProperty("Style")]
        public String CleanStyle
        {
            get
            {
                return StyleRef != null ? StyleRef.StyleName : Style;
            }
        }

        [JsonIgnore]
        [MaxLength(50)]
        public String Class { get; set; }
        [JsonProperty("Class")]
        public String CleanClass
        {
            get
            {
                return ClassTemplate != null ? ClassTemplate.Name : Class.Replace("Male", "").Replace("Female", "").Replace("()", "").Trim();
            }
        }

        [MaxLength(12)]
        public String Winning_ABKC { get; set; }

        public Int32 Points { get; set; }

        public Boolean ChampWin { get; set; }

        public Int32 ChampPoints { get; set; }

        public Boolean? NoComp { get; set; }

        [MaxLength(250)]
        public String Comments { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreateDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ModifyDate { get; set; }

        [MaxLength(50)]
        public String ModifiedBy { get; set; }

        public int? DogId { get; set; }
        public int? ArmbandNumber { get; set; }

        [JsonIgnore]
        public virtual Shows Show { get; set; }
        [JsonIgnore]
        public virtual ClassTemplates ClassTemplate { get; set; }
        [NotMapped]
        public int? ClassTemplateId { get { return ClassTemplate?.ClassId; } }
        [JsonIgnore]
        public virtual Styles StyleRef { get; set; }
        [NotMapped]
        public int? StyleId { get { return StyleRef?.Id; } }

    }

}
