using System;

namespace CoreDAL.Models.DTOs
{
    /// <summary>
    /// used to save DRAFT information about a litter once it has been started
    /// </summary>
    public class LitterDraftDTO : BaseDraftRegistrationSubmitDTO
    {
        public DateTime? DateOfBreeding { get; set; }
        public DateTime? DateOfLitterBirth { get; set; }
        public Breeds Breed { get; set; }
        public int NumberOfMalesBeingRegistered { get; set; }
        public int NumberOfFemalesBeingRegistered { get; set; }
        public bool FrozenSemenUsed { get; set; }
        public DateTime? DateSemenCollected { get; set; }
    }
}