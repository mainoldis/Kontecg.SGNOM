using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Accounting;
using Kontecg.Domain;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.WorkRelations;
using NMoneys;

namespace Kontecg.Salary
{
    [Table("time_distributions", Schema = "sal")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class TimeDistribution : AuditedEntity<long>, IMustHaveCompany
    {
        public virtual int DocumentId { get; set; }

        [Required]
        [ForeignKey("DocumentId")]
        public virtual TimeDistributionDocument Document { get; set; }

        [Required]
        public virtual long PersonId { get; set; }

        public virtual long EmploymentId { get; set; }

        [Required]
        [ForeignKey("EmploymentId")]
        public virtual EmploymentDocument Employment { get; set; }

        [Required]
        public virtual Guid GroupId { get; set; }

        [Required]
        public virtual int CenterCost { get; set; }

        [Required]
        public virtual GenerationSystemData Kind { get; set; }

        public virtual int PaymentDefinitionId { get; set; }

        [Required]
        [ForeignKey("PaymentDefinitionId")]
        public virtual PaymentDefinition PaymentDefinition { get; set; }

        [Required]
        public virtual decimal Hours { get; set; }

        public virtual Money? Amount { get; set; }

        /// <summary>
        ///     Currency.
        ///     Default: CUP
        /// </summary>
        public virtual CurrencyIsoCode? Currency { get; set; }

        public virtual decimal? ReservedForHoliday { get; set; }

        public virtual Money? AmountReservedForHoliday { get; set; }

        public virtual decimal? RatePerHour { get; set; }

        [Required]
        public virtual AccountingNoteStatus Status { get; set; }

        [Required] 
        public virtual int CompanyId { get; set; }

        public virtual List<TimeDistributionPlus> Plus { get; set; }

        public TimeDistribution()
        {
            Status = AccountingNoteStatus.ToAnalyze;
            Plus = new List<TimeDistributionPlus>();
        }

        public TimeDistribution(int documentId, long personId, long employmentId, GenerationSystemData kind, int paymentDefinitionId)
            : this()
        {
            DocumentId = documentId;
            PersonId = personId;
            EmploymentId = employmentId;
            Kind = kind;
            PaymentDefinitionId = paymentDefinitionId;
        }

        public virtual void SetRatePerHour()
        {
            RatePerHour = Employment.RatePerHour;
        }
    }
}
