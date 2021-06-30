using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Controllers.Api;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;

namespace CoreApp.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionModel> FinalizeTransaction(RegistrationPaymentRequest paymentRequest, UserModel chargedTo);
        Task<RefundModel> IssueRefund(TransactionModel originalTransaction, ICollection<PaymentItemDTO> registrationsToRefund, UserModel issuedBy, string reason);
        Task<TransactionModel> GetTransaction(int transactionId);
        Task<RefundModel> GetRefund(int refundId);
        Task<RefundModel> IssueRefundForRegistration(IRegistration registration, UserModel curUser, string reason);
    }
}