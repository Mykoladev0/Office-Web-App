using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullsBluffCore.Models
{
    public class EventResultDTO
    {
        public int ShowId { get; set; }
        /// <summary>
        /// if null, will assume it is a new result
        /// </summary>
        public int? ResultId { get; set; }
        public int ClassId { get; set; }
        public int? StyleId { get; set; }
        public int ArmbandNumber { get; set; }
        public int DogId { get; set; }
        public bool? NoComp { get; set; }
        public int Points { get; set; }

    }
}
