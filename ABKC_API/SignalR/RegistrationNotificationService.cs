using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CoreApp.Models;
using CoreDAL.Interfaces;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2.Registrations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CoreApp.SignalR
{
    public class RegistrationNotificationService : IRegistrationNotificationService
    {
        private IHubContext<ConsumerRegistrationHub> _regConsumerHub;
        private IHubContext<OfficeHub> _officeHub;
        private IMapper _autoMapper;
        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _appConfig;

        public RegistrationNotificationService(IHubContext<OfficeHub> officeHub, IHubContext<ConsumerRegistrationHub> regConsumerHub,
        IMapper autoMapper, ISendGridClient sendGridClient, IConfiguration appConfig)
        {
            _regConsumerHub = regConsumerHub;
            _officeHub = officeHub;
            _autoMapper = autoMapper;
            _sendGridClient = sendGridClient;
            _appConfig = appConfig;
        }

        public Task NewRegistrationSubmitted(IRegistration registration, bool isOvernight, bool isRush)
        {
            // RegistrationResultDTO reg = _autoMapper.Map<RegistrationResultDTO>(registration);
            return _officeHub.Clients.All.SendAsync(nameof(NewRegistrationSubmitted), new { registration, isOvernight, isRush });
        }

        public async Task RegistrationApproved(IRegistration registration)
        {
            // RegistrationResultDTO reg = _autoMapper.Map<RegistrationResultDTO>(registration);
            string dateSubmitted = registration.DateSubmitted.HasValue ? registration.DateSubmitted.Value.ToShortDateString() : "UNKNOWN";
            string messageBody = $"Congratulations, the {registration.RegistrationType} submitted on {dateSubmitted} has been approved. You should receive your documents shortly.  Thank you.";
            await SendCustomerEmail(registration, "ABKC Registration Approved", messageBody);
            await _regConsumerHub.Clients.All.SendAsync(nameof(RegistrationApproved), registration);
            return;
        }

        public async Task RegistrationDenied(IRegistration registration)
        {
            string dateSubmitted = registration.DateSubmitted.HasValue ? registration.DateSubmitted.Value.ToShortDateString() : "UNKNOWN";
            string messageBody = $"Unfortunately, the {registration.RegistrationType} submitted on {dateSubmitted} has been denied.  Please contact the ABKC office for more information";
            await SendCustomerEmail(registration, "ABKC Registration Denied", messageBody);

            await _regConsumerHub.Clients.All.SendAsync(nameof(RegistrationDenied), registration);
            return;
        }

        public async Task RegistrationInformationRequested(IRegistration registration)
        {
            string dateSubmitted = registration.DateSubmitted.HasValue ? registration.DateSubmitted.Value.ToShortDateString() : "UNKNOWN";
            string messageBody = $"The {registration.RegistrationType} submitted on {dateSubmitted} has needs further details. Please log into your ABKC account and correct any problems with the registration";
            await SendCustomerEmail(registration, "ABKC Registration Needs Information", messageBody);
            await _regConsumerHub.Clients.All.SendAsync(nameof(RegistrationInformationRequested), registration);
            return;
        }

        public Task RegistrationsApproved(ICollection<int> registrationIds, int submittedBy)
        {
            return _regConsumerHub.Clients.All.SendAsync(nameof(RegistrationsApproved), new { registrationIds, submittedBy });
        }

        private async Task<bool> SendCustomerEmail(IRegistration registration, string subject, string messageBody)
        {
            if (Boolean.Parse(_appConfig["EnableEmailSending"]) == false)
            {
                return false;
            }
            string fromEmail = _appConfig["SendGrid:FromEmail"];
            EmailAddress from = new EmailAddress(fromEmail, "ABKC Office");
            EmailAddress to = new EmailAddress(registration.SubmittedBy.LoginName);

            SendGridMessage message = MailHelper.CreateSingleEmail(from, to, subject, messageBody, messageBody);
            try
            {
                await _sendGridClient.SendEmailAsync(message);
                return true;
            }
            catch (Exception e)
            {
                //TODO: record exception
                return false;
            }
        }

    }
}