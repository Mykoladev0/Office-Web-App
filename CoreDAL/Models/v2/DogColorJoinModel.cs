namespace CoreDAL.Models.v2
{
    public class DogColorJoinModel
    {
        public int DogId { get; set; }
        public virtual BaseDogModel Dog { get; set; }
        public int ColorId { get; set; }
        public virtual Colors Colors { get; set; }

    }
}