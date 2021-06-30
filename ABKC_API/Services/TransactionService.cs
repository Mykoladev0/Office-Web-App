using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.Controllers.Api;
using CoreApp.Interfaces;
using CoreDAL;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace CoreApp.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ABKCOnlineContext _context;
        private readonly IGeneralRegistrationService _genRegService;
        private readonly IABKCUserService _abkcUserService;

        public TransactionService(ABKCOnlineContext context, IGeneralRegistrationService genRegService, IABKCUserService abkcUserService)
        {
            _context = context;
            _genRegService = genRegService;
            _abkcUserService = abkcUserService;
        }

        /// <summary>
        /// Saves a new transaction to the datastore
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <param name="chargedTo"></param>
        /// <returns></returns>
        public async Task<TransactionModel> FinalizeTransaction(RegistrationPaymentRequest paymentRequest, UserModel chargedTo)
        {
            TransactionModel transaction = new TransactionModel
            {
                Amount = paymentRequest.amount,
                TransactionType = TransactionModel.TransactionTypeEnum.Stripe,
                ChargedTo = chargedTo,
                // StripeChargeId = paymentRequest.tokenId,
                RegistrationCharges = paymentRequest.registrations
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            Charge charge = await SendStripeCharge(transaction, paymentRequest.tokenId);
            if (charge != null)
            {
                //TODO: if successful, iterate through all registrations, mark them as submitted and associate transaction to them
                foreach (var reg in paymentRequest.registrations)
                {
                    await _genRegService.SubmitRegistration(reg.RegistrationId, reg.RegistrationType, chargedTo, transaction);
                }
            }
            return transaction;
        }

        public async Task<RefundModel> IssueRefundForRegistration(IRegistration reg, UserModel issuedBy, string reason)
        {
            //get full original transaction
            TransactionModel found = await GetTransaction(reg.AssociatedTransaction.Id);
            //calculate refund amount
            PaymentItemDTO payment = found.RegistrationCharges.FirstOrDefault(r => r.RegistrationId == reg.Id && r.RegistrationType == r.RegistrationType);
            if (payment == null)
            {
                throw new InvalidOperationException($"No matching registration was found in the transaction");
            }
            double refundAmount = payment.Amount;
            //create refund transaction
            RefundModel refund = new RefundModel
            {
                OriginalTransaction = found,
                IssuedBy = issuedBy,
                RegistrationsRefunded = new List<PaymentItemDTO> { payment },
                RefundAmount = refundAmount,
                RefundedTo = found.ChargedTo,
            };
            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();
            //send refund to stripe
            var refundService = new RefundService();
            var refundOptions = new RefundCreateOptions
            {
                ChargeId = found.StripeChargeId,
                Amount = (long)(refundAmount * 100),
                Metadata = new Dictionary<string, string>() { { "RefundId", refund.Id.ToString() }, { "Reason", !string.IsNullOrEmpty(reason) ? reason : "ABKC Office issued a refund, no reason provided" } },
                // Reason = ""//duplicate, fraudulent, requested_by_customer
            };
            try
            {
                Refund stripeRefund = refundService.Create(refundOptions);
                if (!string.IsNullOrEmpty(stripeRefund.FailureReason))
                {
                    //failure, backout and let the consumer know
                    //write problem to exceptions
                    _context.Refunds.Remove(refund);
                    //not a valid transaction, back out!
                    await _context.SaveChangesAsync();
                    throw new Exception($"Refund failed to go through because {stripeRefund.FailureReason}. A manual refund may be required.");
                }

                //update refund with response Id
                refund.StripeID = stripeRefund.StripeResponse.RequestId;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _context.Refunds.Remove(refund);
                await _context.SaveChangesAsync();
                throw new Exception("Could not issue refund", e);
            }

            //TODO:what do we do about the original registrations that have been refunded?
            return refund;
        }

        public async Task<RefundModel> IssueRefund(TransactionModel originalTransaction, ICollection<PaymentItemDTO> registrationsToRefund, UserModel issuedBy, string reason)
        {
            //calculate refund amount

            double refundAmount = 0;
            if (registrationsToRefund == null || !registrationsToRefund.Any())
            {
                //whole transaction is refunded
                refundAmount = originalTransaction.Amount;
            }
            else
            {
                registrationsToRefund.Select(r => r.Amount).Sum();
            }
            //create refund transaction
            RefundModel refund = new RefundModel
            {
                OriginalTransaction = originalTransaction,
                IssuedBy = issuedBy,
                RegistrationsRefunded = registrationsToRefund,
                RefundAmount = refundAmount,
                RefundedTo = originalTransaction.ChargedTo,
            };
            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();
            //send refund to stripe
            var refundService = new RefundService();
            var refundOptions = new RefundCreateOptions
            {
                ChargeId = originalTransaction.StripeChargeId,
                Amount = (long)(refundAmount * 100),
                Metadata = new Dictionary<string, string>() { { "RefundId", refund.Id.ToString() }, { "Reason", !string.IsNullOrEmpty(reason) ? reason : "ABKC Office issued a refund, no reason provided" } },
                // Reason = ""//duplicate, fraudulent, requested_by_customer
            };
            try
            {
                Refund stripeRefund = refundService.Create(refundOptions);
                if (!string.IsNullOrEmpty(stripeRefund.FailureReason))
                {
                    //failure, backout and let the consumer know
                    //write problem to exceptions
                    _context.Refunds.Remove(refund);
                    //not a valid transaction, back out!
                    await _context.SaveChangesAsync();
                    throw new Exception($"Refund failed to go through because {stripeRefund.FailureReason}. A manual refund may be required.");
                }

                //update refund with response Id
                refund.StripeID = stripeRefund.StripeResponse.RequestId;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _context.Refunds.Remove(refund);
                await _context.SaveChangesAsync();
                throw new Exception("Could not issue refund", e);
            }

            //TODO:what do we do about the original registrations that have been refunded?
            return refund;
        }

        public async Task<TransactionModel> GetTransaction(int transactionId)
        {
            return await _context.Transactions
            .Include(t => t.ChargedTo)
            .Where(t => t.Id == transactionId).FirstOrDefaultAsync();
        }
        public async Task<RefundModel> GetRefund(int refundId)
        {
            return await _context.Refunds
            .Include(t => t.IssuedBy)
            .Include(t => t.RefundedTo)
            .Include(t => t.OriginalTransaction)
            .Where(t => t.Id == refundId).FirstOrDefaultAsync();
        }


        private async Task<Charge> SendStripeCharge(TransactionModel transaction, string tokenId)
        {
            CustomerService customerService = new CustomerService();
            Customer customer = null;
            UserModel chargedTo = transaction.ChargedTo;
            if (chargedTo == null)
            {
                return null;
            }
            string customerId = transaction.ChargedTo?.StripeCustomerId;
            if (string.IsNullOrEmpty(customerId))
            {
                //TODO: switch to source instead of tokens to attach to a customer.  Allows multiple cards
                //https://stripe.com/docs/api/customers/create
                //https://stripe.com/docs/checkout#integration-custom
                //create customer in stripe
                customer = await customerService.CreateAsync(new CustomerCreateOptions
                {
                    Email = chargedTo.LoginName,
                    SourceToken = tokenId,
                });
                chargedTo.StripeCustomerId = customer.Id;
                await _abkcUserService.AddStripeCustomerId(chargedTo.Id, customer.Id);
            }
            else
            {
                customer = await customerService.GetAsync(customerId);
                //TODO: update customer if source is different than what is stored (multiple payment options)
            }

            ChargeCreateOptions myCharge = new ChargeCreateOptions
            {
                Description = $"ABKC Payment Charge for {transaction.RegistrationCharges.Count} registrations",
                CustomerId = customer.Id,
                Amount = (long)(transaction.Amount * 100),//expects amounts in cents
                Currency = "usd",
                StatementDescriptor = $"ABKC* {transaction.RegistrationCharges.Count} regs",
                Metadata = new Dictionary<string, string>() { { "TransactionId", transaction.Id.ToString() } },
                Capture = true,
                ReceiptEmail = chargedTo.LoginName
            };

            ChargeService chargeService = new ChargeService();
            Charge stripeCharge = chargeService.Create(myCharge);
            StripeResponse response = stripeCharge.StripeResponse;
            if (stripeCharge.Paid == false || !string.IsNullOrEmpty(stripeCharge.FailureCode))
            {
                //write problem to exceptions
                _context.Transactions.Remove(transaction);
                //not a valid transaction, back out!
                await _context.SaveChangesAsync();
                throw new Exception(stripeCharge.Outcome.ToJson());
            }
            transaction.StripeChargeId = stripeCharge.Id;
            await _context.SaveChangesAsync();

            return stripeCharge;
        }
    }
}