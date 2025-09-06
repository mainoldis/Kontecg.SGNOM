using System;
using System.ComponentModel.DataAnnotations;
using NMoneys;

namespace Kontecg.SocialSecurity
{
    public class TemporalDisability : SubsidyDocument
    {
        public const int MinimumWaitingPeriodDays = 0;

        public const int MaximumWaitingPeriodDays = 3;

        [Required]
        public virtual MedicalExpertiseStatus MedicalExpertise { get; set; }

        [Required]
        public virtual TemporalDisabilityStatus Detail { get; set; }

        [Required]
        [Range(MinimumWaitingPeriodDays, MaximumWaitingPeriodDays)]
        public virtual int WaitingPeriodDays { get; set; }

        [Required]
        public virtual bool Hospitalized { get; set; }

        /// <inheritdoc />
        public TemporalDisability()
        {
            MedicalExpertise = MedicalExpertiseStatus.None;
            Detail = TemporalDisabilityStatus.OnSetOfDisease;
            WaitingPeriodDays = 3;
            Hospitalized = false;
        }

        /// <inheritdoc />
        public TemporalDisability(int documentDefinitionId, long personId, long employmentId, Guid groupId, int days, decimal percent,
            decimal averageAmount, CurrencyIsoCode currency, DateTime since, DateTime until,
            long? previousId = null) : base(documentDefinitionId, personId, employmentId, groupId, days, percent,
            averageAmount, currency, since, until, previousId)
        {
            MedicalExpertise = MedicalExpertiseStatus.None;
            Detail = TemporalDisabilityStatus.OnSetOfDisease;
            WaitingPeriodDays = 3;
            Hospitalized = false;
        }

        /// <inheritdoc />
        public TemporalDisability(int documentDefinitionId, long personId, long employmentId, Guid groupId, int days, decimal percent,
            decimal averageAmount, CurrencyIsoCode currency, DateTime since, DateTime until,
            MedicalExpertiseStatus medicalExpertise, TemporalDisabilityStatus detail, int waitingPeriodDays,
            bool hospitalized, long? previousId = null)
            : base(documentDefinitionId, personId, employmentId, groupId, days, percent, averageAmount, currency, since, until, previousId)
        {
            MedicalExpertise = medicalExpertise;
            Detail = detail;
            WaitingPeriodDays = waitingPeriodDays;
            Hospitalized = hospitalized;
        }
    }
}
