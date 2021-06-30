using System.Collections.Generic;

namespace CoreDAL.Models.DTOs
{
    public class PaymentQuoteDTO
    {
        public PaymentQuoteDTO()
        {
            Registrations = new List<PaymentItemDTO>();
            InvalidRegistrations = new List<InvalidItemDTO>();
        }

        public double SubTotal { get; set; }
        public double TransactionFee { get; set; }

        public ICollection<PaymentItemDTO> Registrations { get; set; }

        public ICollection<InvalidItemDTO> InvalidRegistrations { get; set; }

    }

    public class PaymentItemDTO : RegistrationSubmitDTO
    {
        public double Amount { get; set; }
    }
    public class InvalidItemDTO : RegistrationSubmitDTO
    {
        public double Amount { get; set; }
        public string Reason { get; set; }
    }
}