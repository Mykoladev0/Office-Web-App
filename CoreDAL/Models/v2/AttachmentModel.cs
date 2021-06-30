namespace CoreDAL.Models.v2
{
    public class AttachmentModel : BaseDBModel
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }

    }
}