using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullsBluffCore.Models
{
    public class SimpleShowDTO
    {
        public int ShowId { get; set; }
        public string ShowName { get; set; }
        public DateTime ShowDate { get; set; }
        [JsonIgnore]
        public string BreedList { get; set; }
        public int NumberBreeds { get
            {
                var breedCount = BreedList.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries).Count();
                return breedCount;
            }
        }
        public string Address { get; set; }
    }
}
