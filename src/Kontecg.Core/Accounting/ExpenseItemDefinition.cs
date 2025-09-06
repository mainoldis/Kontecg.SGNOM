using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.Accounting
{
    [Table("expense_item_definitions", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class ExpenseItemDefinition : FullAuditedEntity, IPassivable, IMustHaveCompany
    {
        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int MaxDescriptionLength = 150;

        /// <summary>
        ///     Max length of the <see cref="Reference" /> property.
        /// </summary>
        public const int MaxReferenceLength = 50;

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [Required]
        public virtual int Code { get; set; }

        [StringLength(MaxReferenceLength)]
        public virtual string Reference { get; set; }

        public virtual int? CenterCostDefinitionId { get; set; }

        [ForeignKey("CenterCostDefinitionId")]
        public virtual CenterCostDefinition CenterCostDefinition { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int CompanyId { get; set; }

        public ExpenseItemDefinition()
        {
            IsActive = true;
        }

        public ExpenseItemDefinition(string description, int code, string reference = null)
            :this()
        {
            Description = description;
            Code = code;
            Reference = reference ?? $"EG_{code}";

            SetNormalizedDescription();
            SetNormalizedReference();
        }

        public virtual void SetNormalizedDescription()
        {
            Description = Description?.ToUpperInvariant();
        }

        public virtual void SetNormalizedReference()
        {
            Reference = Reference?.ToUpperInvariant();
        }

        public override string ToString()
        {
            return $"{Code} - {Description}";
        }
    }
}
