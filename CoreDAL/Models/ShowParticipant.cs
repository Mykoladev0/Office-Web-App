using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoreDAL.Models
{
    public class ShowParticipant
    {
        public int Id { get; set; }
        public int? ArmbandNumber { get; set; }
        [JsonIgnore]
        public virtual Shows Show { get; set; }
        [NotMapped]
        public int? ShowId
        {
            get
            {
                return Show?.ShowId;
            }
        }
        [JsonIgnore]
        public virtual Dogs Dog{ get; set; }
        [NotMapped]
        public int? DogId
        {
            get
            {
                return Dog?.Id;
            }
        }
        public DateTime DateRegistered { get; set; }

        //TODO:Add Who Registered!
    }
}
