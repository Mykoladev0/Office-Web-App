namespace CoreDAL.Models.DTOs
{
    public class ABKCDogDTO : BaseDogDTO
    {
        public int OriginalTableId { get; set; }
        public string ABKCNumber { get; set; }
        public int LitterId { get; set; }
    }
}