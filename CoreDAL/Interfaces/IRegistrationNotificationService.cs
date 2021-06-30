using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2.Registrations;

namespace CoreDAL.Interfaces
{
    public interface IRegistrationNotificationService
    {
        Task RegistrationApproved(IRegistration registration);
        Task RegistrationsApproved(ICollection<int> registrationsIds, int submittedBy);
        Task RegistrationDenied(IRegistration registration);
        Task RegistrationInformationRequested(IRegistration registration);

        Task NewRegistrationSubmitted(IRegistration registration, bool isOvernight, bool isRush);
    }
}