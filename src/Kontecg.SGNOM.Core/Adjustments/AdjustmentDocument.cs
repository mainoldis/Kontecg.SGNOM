using Kontecg.Workflows;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NMoneys;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using Kontecg.WorkRelations;
using System;
using Kontecg.Timing;
using Kontecg.Accounting;

namespace Kontecg.Adjustments
{
    [Table("adjustment_documents", Schema = "aju")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class AdjustmentDocument : CreationAuditedEntity<long>, IMustHaveCompany, IMustHaveReview
    {
        public const int MaxCodeLength = 20;

        public const int MaxDescriptionLength = 500;

        [Required]
        public virtual int DocumentDefinitionId { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        [Required]
        public virtual long PersonId { get; set; }

        public virtual int AdjustmentDefinitionId { get; set; }

        [Required]
        [ForeignKey("AdjustmentDefinitionId")]
        public virtual AdjustmentDefinition AdjustmentDefinition { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [Required]
        public virtual Money Amount { get; set; } //Si voy a ajustar solo vacaciones dejo este campo en cero.

        public virtual decimal? HoursReservedForHoliday { get; set; }

        public virtual Money? AmountReservedForHoliday { get; set; }

        /// <summary>
        ///     Currency.
        ///     Default: CUP
        /// </summary>
        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        [Required]
        public virtual DateTime MadeOn { get; set; }

        public virtual long EmploymentId { get; set; }

        [Required]
        [ForeignKey("EmploymentId")]
        public virtual EmploymentDocument EmploymentDocument { get; set; }

        [Required]
        public virtual Guid GroupId { get; set; }

        [Required]
        public virtual int CenterCost { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual AccountingNoteStatus Status { get; set; }

        [Required]
        public virtual ReviewStatus Review { get; set; }

        public AdjustmentDocument()
        {
            MadeOn = Clock.Now;
            Currency = CurrencyIsoCode.CUP;
            Status = AccountingNoteStatus.ToAnalyze;
            Review = ReviewStatus.ForReview;
        }

        public AdjustmentDocument(int documentDefinitionId, long personId, int adjustmentDefinitionId, string description, long employmentId, Guid groupId, int centerCost, Money amount)
            :this()
        {
            DocumentDefinitionId = documentDefinitionId;
            PersonId = personId;
            AdjustmentDefinitionId = adjustmentDefinitionId;
            Description = description;
            EmploymentId = employmentId;
            GroupId = groupId;
            CenterCost = centerCost;
            Amount = amount;
        }

        public AdjustmentDocument(int documentDefinitionId, long personId, AdjustmentDefinition adjustmentDefinition, string description, Money amount, EmploymentDocument employmentDocument)
            : this(documentDefinitionId, personId, adjustmentDefinition.Id, description, employmentDocument.Id, employmentDocument.GroupId, employmentDocument.CenterCost, amount)
        {
            AdjustmentDefinition = adjustmentDefinition;
            EmploymentDocument = employmentDocument;
        }
    }
}
