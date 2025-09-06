using System;
using System.ComponentModel.DataAnnotations;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.Workflows;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.WorkRelations;
using System.Collections.Generic;
using Itenso.TimePeriod;
using NMoneys;
using Kontecg.Timing;

namespace Kontecg.SocialSecurity
{
    [Table("subsidy_documents", Schema = "sub")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class SubsidyDocument : CreationAuditedEntity<long>, IMustHaveCompany, IMustHaveReview
    {
        public const int MaxCodeLength = 20;

        [Required]
        public virtual int DocumentDefinitionId { get; set; }

        [Required]
        public virtual DateTime MadeOn { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        [Required]
        public virtual long PersonId { get; set; }

        public virtual int SubsidyPaymentDefinitionId { get; set; }

        [Required]
        [ForeignKey("SubsidyPaymentDefinitionId")]
        public virtual SubsidyPaymentDefinition SubsidyPaymentDefinition { get; set; }

        [Required]
        public virtual DateTime Since { get; set; }

        [Required]
        public virtual DateTime Until { get; set; }

        public virtual long EmploymentId { get; set; }

        [Required]
        [ForeignKey("EmploymentId")]
        public virtual EmploymentDocument EmploymentDocument { get; set; }

        [Required]
        public virtual Guid GroupId { get; set; }

        public virtual long? PreviousId { get; set; }

        [ForeignKey("PreviousId")]
        public virtual SubsidyDocument Previous { get; set; }

        [Required]
        public virtual int SubsidizedDays { get; set; }

        [Required]
        public virtual int DeseaseDays { get; set; }

        [Required]
        public virtual decimal Percent { get; set; }

        [Required]
        public virtual Money AverageAmount { get; set; }

        [Required]
        public virtual Money Amount { get; set; }

        /// <summary>
        ///     Currency.
        ///     Default: CUP
        /// </summary>
        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual ReviewStatus Review { get; set; }

        public virtual List<SubsidyNote> Notes { get; set; }

        public SubsidyDocument()
        {
            Code = "AUTO";
            Currency = CurrencyIsoCode.CUP;
            Review = ReviewStatus.ForReview;
            Notes = [];
        }

        public SubsidyDocument(int documentDefinitionId, long personId, long employmentId, Guid groupId, int subsidizedDays, decimal percent, decimal averageAmount, CurrencyIsoCode currency, long? previousId = null)
            :this()
        {
            DocumentDefinitionId = documentDefinitionId;
            PersonId = personId;
            EmploymentId = employmentId;
            GroupId = groupId;
            SubsidizedDays = subsidizedDays;
            Percent = percent;
            AverageAmount = new Money(averageAmount, currency);
            Amount = Money.Zero(currency);
            PreviousId = previousId;

            Since = Clock.Now;
            Until = Since;
            DeseaseDays = 1;
        }

        public SubsidyDocument(int documentDefinitionId, long personId, long employmentId, Guid groupId, int subsidizedDays, decimal percent, decimal averageAmount, CurrencyIsoCode currency, DateTime since, DateTime until, long? previousId = null)
            : this()
        {
            DocumentDefinitionId = documentDefinitionId;
            PersonId = personId;
            EmploymentId = employmentId;
            GroupId = groupId;
            SubsidizedDays = subsidizedDays;
            Percent = percent;
            AverageAmount = new Money(averageAmount, currency);
            Amount = Money.Zero(currency);
            Since = since;
            Until = until;

            DeseaseDays = new TimeRange(Since, Until, true).Duration.Days + 1;

            PreviousId = previousId;
        }
    }
}
