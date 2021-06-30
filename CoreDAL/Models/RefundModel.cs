using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;

namespace CoreDAL.Models
{
    public class RefundModel : BaseDBModel
    {
        public RefundModel()
        {
            this.RegistrationsRefunded = new List<PaymentItemDTO>();

        }
        public double RefundAmount { get; set; }
        public UserModel IssuedBy { get; set; }
        public UserModel RefundedTo { get; set; }
        public string StripeID { get; set; }
        /// <summary>
        /// used for refunds, to track what the original transaction was
        /// </summary>
        /// <value></value>
        public virtual TransactionModel OriginalTransaction { get; set; }

        public string RegistrationsRefundedAsJSON { get; set; }

        private ICollection<PaymentItemDTO> _registrations;

        [NotMapped]
        public ICollection<PaymentItemDTO> RegistrationsRefunded
        {
            get { return _registrations ?? new List<PaymentItemDTO>(); }
            set
            {
                _registrations = value ?? new List<PaymentItemDTO>();
                RegistrationsRefundedAsJSON = Newtonsoft.Json.JsonConvert.SerializeObject(_registrations);
            }
        }
    }
}