using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoreDAL.Models.v2.Registrations
{
    public enum RegistrationTypeEnum
    {
        Pedigree = 1,
        Litter = 2,
        JuniorHandler = 3,
        Transfer = 4,
        Puppy = 5,
        BullyId = 6
    }

    public abstract class BaseRegistrationModel<R> : BaseDBModel where R : IRegistrationStatus, new()
    {
        public BaseRegistrationModel()
        {
            StatusHistory = StatusHistory ?? new List<R>();
        }
        public virtual UserModel SubmittedBy { get; set; }

        public virtual ICollection<R> StatusHistory { get; set; }

        [NotMapped]
        public R CurrentStatus
        {
            get
            {
                R cur = default(R);
                if (StatusHistory != null && StatusHistory.Any())
                    cur = StatusHistory.OrderByDescending(s => s.DateModified).First();
                return cur;
            }
        }

        //swipe CC info and amount charged

        //store tracking number if approved

        public string SubmissionNotes { get; set; }

        //special processing
        public bool IsInternationalRegistration { get; set; }
        public bool RushRequested { get; set; }

        //can only be true if !IsInternationalRegistration
        public bool OvernightRequested { get; set; }

        public TransactionModel AssociatedTransaction { get; set; }
        [NotMapped]
        public abstract RegistrationTypeEnum RegistrationType { get; }

        [NotMapped]
        public RegistrationStatusEnum CurStatus
        {
            get
            {
                if (CurrentStatus != null) return CurrentStatus.Status;
                return RegistrationStatusEnum.Unknown;
            }
        }
        [NotMapped]
        public DateTime? DateSubmitted
        {
            get
            {
                if (StatusHistory == null || !StatusHistory.Any())
                {
                    return null;
                }
                R submitted = StatusHistory.OrderByDescending(s => s.DateCreated).FirstOrDefault(s => s.Status == RegistrationStatusEnum.Pending);
                //   if (CurrentStatus != null && CurrentStatus.Status == RegistrationStatusEnum.Pending) return CurrentStatus.DateCreated;
                if (submitted != null) { return submitted.DateCreated; }
                return null;
            }
        }

        public abstract void SetStatus(RegistrationStatusEnum newStatus, UserModel setBy, string comments = "");

    }
}