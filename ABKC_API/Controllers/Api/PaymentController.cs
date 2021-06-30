using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Interfaces;
using CoreApp.Models;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace CoreApp.Controllers.Api
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
    public class PaymentController : BaseAuthorizedAPIControllerWithUser
    {
        private readonly ITransactionService _transactionService;
        private readonly IGeneralRegistrationService _generalRegService;

        public PaymentController(IABKCUserService userService,
        ITransactionService transactionService, IGeneralRegistrationService generalRegService) : base(userService)
        {
            _transactionService = transactionService;
            _generalRegService = generalRegService;
        }

        /// <summary>
        /// after using the stripe client-side api to authorize a transaction, use this endpoint to finalize the charge
        /// and then submit all registrations contained within the transaction
        /// </summary>
        /// <param name="paymentRequest">Includes the amount to charge, the token for authorization from stripe server 
        /// and the registrations to include in the transaction</param>
        /// <returns></returns>
        [HttpPost("finalizeTransaction")]
        public async Task<ActionResult<TransactionModel>> FinalizeTransaction([FromBody]RegistrationPaymentRequest paymentRequest)
        {

            if (paymentRequest == null)
            {
                return BadRequest("No payment request provided");
            }
            if (paymentRequest.tokenId == null)
            {
                return BadRequest("A Stripe Token Id for the transaction is needed before it can be completed");
            }
            UserModel curUser = await base.GetLoggedInUser();
            try
            {
                TransactionModel transaction = await _transactionService.FinalizeTransaction(paymentRequest, curUser);
                return Ok(transaction);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Using the id of the original transaction, issue a refund. NOTE, transaction must be included for a refund
        /// </summary>
        /// <param name="originalTransactionId">the id of the original transaction where the refund will come from</param>
        /// <param name="registrationsToRefund">the registrations within the transaction to refund. If none provided, the entire transaction is refunded</param>
        /// <param name="reason">reason for refund</param>
        /// <returns></returns>
        [HttpPost("issueRefund")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<RefundModel>> IssueRefund(int originalTransactionId, [FromBody]ICollection<PaymentItemDTO> registrationsToRefund, string reason)
        {

            UserModel curUser = await base.GetLoggedInUser();
            TransactionModel origTransaction = await _transactionService.GetTransaction(originalTransactionId);

            if (origTransaction == null)
            {
                return BadRequest("No original transaction found");
            }
            try
            {
                RefundModel refund = await _transactionService.IssueRefund(origTransaction, registrationsToRefund, curUser, reason);
                return Ok(refund);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Issue a refund for a submitted registration
        /// </summary>
        /// <param name="registrationId">Id of registration</param>
        /// <param name="registrationType">Type of registration</param>
        /// <param name="reason">reason for refund</param>
        /// <returns></returns>
        [HttpPost("issueRefundForRegistration")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<RefundModel>> IssueRefundForRegistration(int registrationId, RegistrationTypeEnum registrationType, string reason)
        {

            UserModel curUser = await base.GetLoggedInUser();
            try
            {
                IRegistration found = await _generalRegService.GetRegistration(registrationId, registrationType);
                if (found == null)
                {
                    return NotFound($"Registration for id {registrationId} was not found");
                }
                if (found.AssociatedTransaction == null)
                {
                    return BadRequest("No associated transaction for the registration could be found");
                }
                RefundModel refund = await _transactionService.IssueRefundForRegistration(found, curUser, reason);
                return Ok(refund);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }

    public class RegistrationPaymentRequest
    {
        public string tokenId { get; set; }
        public ICollection<PaymentItemDTO> registrations { get; set; }
        public int amount { get; set; }
    }
}