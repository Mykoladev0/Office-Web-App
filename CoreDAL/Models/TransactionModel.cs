using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;

namespace CoreDAL.Models
{
    public class TransactionModel : BaseDBModel
    {
        public enum TransactionTypeEnum
        {
            Stripe = 0,
            Cash = 1,
            Paypal = 2
        }
        public TransactionModel()
        {
            RegistrationCharges = new List<PaymentItemDTO>();
        }
        public string StripeChargeId { get; set; }
        public UserModel ChargedTo { get; set; }
        public double Amount { get; set; }
        public string RegistrationsAsJSON { get; set; }

        public TransactionTypeEnum TransactionType { get; set; }

        private ICollection<PaymentItemDTO> _registrations;

        [NotMapped]
        public ICollection<PaymentItemDTO> RegistrationCharges
        {
            get { return _registrations ?? new List<PaymentItemDTO>(); }
            set
            {
                _registrations = value ?? new List<PaymentItemDTO>();
                RegistrationsAsJSON = Newtonsoft.Json.JsonConvert.SerializeObject(_registrations);
            }
        }

    }
}