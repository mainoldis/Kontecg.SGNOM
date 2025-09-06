using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Workflows;

namespace Kontecg.Salary
{
    [Table("time_distribution_documents", Schema = "sal")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class TimeDistributionDocument : AuditedEntity, IMustHaveCompany, IMustHaveReview
    {
        public const int MaxDisplayNameLength = 100;

        public const int MaxCodeLength = 5;

        [Required]
        public virtual int DocumentDefinitionId { get; set; }

        [Required]
        public virtual DateTime MadeOn { get; set; }

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual long WorkPlacePaymentId { get; set; }

        [Required]
        public virtual int PeriodId { get; set; }

        [Required]
        public virtual DateTime Since { get; set; }

        [Required]
        public virtual DateTime Until { get; set; }

        [Required]
        public virtual ReviewStatus Review { get; set; }

        public TimeDistributionDocument()
        {
            Review = ReviewStatus.ForReview;
            SetDescriptionNormalized();
        }

        public TimeDistributionDocument(int documentDefinitionId, long workPlacePaymentId, int periodId, string description, DateTime since, DateTime until)
            : this()
        {
            DocumentDefinitionId = documentDefinitionId;
            WorkPlacePaymentId = workPlacePaymentId;
            PeriodId = periodId;
            DisplayName = description;
            Since = since;
            Until = until;
        }

        protected virtual void SetDescriptionNormalized()
        {
            DisplayName = DisplayName?.ToUpperInvariant();
        }
    }
}
