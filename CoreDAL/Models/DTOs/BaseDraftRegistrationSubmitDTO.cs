namespace CoreDAL.Models.DTOs
{
    public class BaseDraftRegistrationSubmitDTO
    {
        public int Id { get; set; }

        public bool? OvernightRequested { get; set; }
        public bool? RushRequested { get; set; }
        public bool IsInternational { get; set; }
    }
}