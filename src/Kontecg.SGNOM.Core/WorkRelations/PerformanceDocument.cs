using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Workflows;

namespace Kontecg.WorkRelations
{
    [Table("performance_documents", Schema = "rel")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PerformanceDocument : AuditedEntity, IMustHaveCompany, IMustHaveReview
    {
        public const int MaxCodeLength = 5;

        public const int MaxDisplayNameLength = 150;

        [Required]
        public virtual int DocumentDefinitionId { get; set; }

        /// <inheritdoc />
        [Required]
        public DateTime MadeOn { get; set; }

        [NotMapped]
        public virtual DocumentDefinition DocumentDefinition { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public string Code { get; set; }

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        [Required]
        public virtual int PeriodId { get; set; }

        [Required]
        public virtual DateTime Since { get; set; }

        [Required]
        public virtual DateTime Until { get; set; }

        [Required]
        public virtual ReviewStatus Review { get; set; }

        public virtual int CompanyId { get; set; }

        public PerformanceDocument()
        {
            Review = ReviewStatus.ForReview;
        }

        public PerformanceDocument(string description, DateTime since, DateTime until)
            :this()
        {
            DisplayName = description;
            Since = since;
            Until = until;

            SetDescriptionAndDatesNormalized();
        }

        protected virtual void SetDescriptionAndDatesNormalized()
        {
            DisplayName = DisplayName?.ToUpperInvariant();
        }
    }
}
