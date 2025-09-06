using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.Accounting
{
    [Table("center_cost_definitions", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class CenterCostDefinition : FullAuditedEntity, IPassivable, IMustHaveCompany
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

        public virtual int AccountDefinitionId { get; set; }

        [Required]
        [ForeignKey("AccountDefinitionId")]
        public virtual AccountDefinition AccountDefinition { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int CompanyId { get; set; }

        public CenterCostDefinition()
        {
            IsActive = true;
        }

        public CenterCostDefinition(AccountDefinition account, string description, int code, string reference = null)
            : this()
        {
            Check.NotNull(account, nameof(account));

            AccountDefinition = account;
            Description = description;
            Code = code;
            Reference = reference ?? $"CC_{account.Account}_{code}";
            
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
