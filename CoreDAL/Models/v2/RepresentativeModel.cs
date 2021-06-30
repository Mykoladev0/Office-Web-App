namespace CoreDAL.Models.v2
{

    /// <summary>
    /// holds representative specific information tied to an ABKC User account
    /// </summary>
    public class RepresentativeModel : BaseDBModel
    {
        public virtual UserModel UserRecord { get; set; }
        public double PedigreeRegistrationFee { get; set; }
        public double LitterRegistrationFee { get; set; }
        public double PuppyRegistrationFee { get; set; }
        public double BullyIdRequestFee { get; set; }
        public double JrHandlerRegistrationFee { get; set; }
        public double TransferFee { get; set; }

    }
}