using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Kontecg.Accounting;
using Kontecg.Adjustments;
using NMoneys;

namespace Kontecg.Retentions
{
    [Table("retention_definitions", Schema = "ret")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class RetentionDefinition : AuditedEntity, IMustHaveCompany, IPassivable
    {
        public const int MaxDescriptionLength = 250;

        public const int MaxTypeLength = 50;

        /// <summary>
        ///     Max length of the <see cref="Reference" /> property.
        /// </summary>
        public const int MaxReferenceLength = AccountingFunctionDefinition.MaxReferenceLength;

        [Required]
        public virtual int Code { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [StringLength(MaxReferenceLength)]
        public virtual string Reference { get; set; }

        [Required]
        [StringLength(MaxTypeLength)]
        public virtual string Type { get; set; }

        [Required]
        public virtual int Priority { get; set; }

        public virtual RetentionMathType? MathType { get; set; }

        public virtual decimal? TaxAmortization { get; set; }

        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        [Required]
        public virtual bool Partial { get; set; }

        [Required]
        public virtual bool Auto { get; set; }

        [Required]
        public virtual bool Credit { get; set; }

        [Required]
        public virtual bool Total { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        public virtual int? RefundDefinitionId { get; set; }

        [ForeignKey("RefundDefinitionId")]
        public virtual AdjustmentDefinition RefundDefinition { get; set; }

        [Required]
        public virtual bool IsActive { get; set; }

        public RetentionDefinition()
        {
            IsActive = true;
        }

        public RetentionDefinition(int companyId, int code, string description, string reference, string type, bool partial, bool auto, bool credit, bool total, int priority = 0, int? refundId = null, CurrencyIsoCode currency = CurrencyIsoCode.CUP)
            : this()
        {
            Code = code;
            Description = description;
            Type = type;
            Reference = reference;
            CompanyId = companyId;
            RefundDefinitionId = refundId;
            Partial = partial;
            Auto = auto;
            Credit = credit;
            Total = total;
            Priority = priority;
            Currency = currency;

            SetNameAndDescriptionNormalized();
        }

        protected virtual void SetNameAndDescriptionNormalized()
        {
            Description = Description?.ToUpperInvariant();
        }
    }
}
