using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullsBluffCore.Models
{
    public class ShowParticipantDTO
    {
        public int? Id { get; set; }
        public int ShowId { get; set; }
        public int DogId { get; set; }
        public int? ArmbandNumber { get; set; }
        public DateTime DateRegistered { get; set; }
    }
}
