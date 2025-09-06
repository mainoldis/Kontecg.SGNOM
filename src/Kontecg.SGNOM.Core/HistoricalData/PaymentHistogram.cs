using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using Kontecg.Accounting;
using Kontecg.Salary;
using NMoneys;
using Itenso.TimePeriod;

namespace Kontecg.HistoricalData
{
    [Table("payment_histogram", Schema = "sal")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PaymentHistogram : CreationAuditedEntity<long>, IMustHaveCompany
    {
        [Required]
        public virtual int DocumentDefinitionId { get; set; }

        [Required]
        public virtual long PersonId { get; set; }

        [Required]
        public virtual Guid GroupId { get; set; }

        [Required]
        public virtual int Year { get; set; }

        [Required]
        public virtual YearMonth Month { get; set; }

        [Required]
        public virtual DateTime Since { get; set; }

        [Required]
        public virtual DateTime Until { get; set; }

        [Required]
        public virtual int WorkedDays { get; set; }

        [Required]
        public virtual Money SalaryPaymentReceived { get; set; }

        [Required]
        public virtual int SickLeaveDays { get; set; }

        [Required]
        public virtual Money SickLeavePaymentReceived { get; set; }

        [Required]
        public virtual int Holidays { get; set; }

        [Required]
        public virtual Money HolidaysPaymentReceived { get; set; }

        [Required]
        public virtual Money SalaryPlusPaymentReceived { get; set; }

        [Required]
        public virtual Money BonusPaymentReceived { get; set; }

        [Required]
        public virtual int CertifiedDays { get; set; }

        /// <summary>
        ///     Currency.
        ///     Default: CUP
        /// </summary>
        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        [StringLength(PaymentDefinition.MaxObservationLength)]
        public virtual string Observation { get; set; }

        [Required]
        public virtual AccountingNoteStatus Status { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        public PaymentHistogram()
        {
            Status = AccountingNoteStatus.ToAnalyze;
            Currency = CurrencyIsoCode.CUP;
        }
    }
}
