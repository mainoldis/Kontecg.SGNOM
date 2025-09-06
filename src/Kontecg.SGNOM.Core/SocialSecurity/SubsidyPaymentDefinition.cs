using System.ComponentModel.DataAnnotations;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.Salary;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Accounting;

namespace Kontecg.SocialSecurity
{
    [Table("subsidy_payment_definitions", Schema = "sub")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class SubsidyPaymentDefinition : AuditedEntity, IMustHaveCompany, IPassivable
    {
        public const int MaxDisplayNameLength = 250;

        public const int MaxSymbolLength = 3;

        /// <summary>
        ///     Max length of the <see cref="Reference" /> property.
        /// </summary>
        public const int MaxReferenceLength = AccountingFunctionDefinition.MaxReferenceLength;

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [StringLength(MaxSymbolLength)]
        public virtual string Symbol { get; set; }

        [Required]
        public virtual decimal BasePercent { get; set; }

        public virtual int PaymentDefinitionId { get; set; }

        [Required]
        [ForeignKey("PaymentDefinitionId")]
        public virtual PaymentDefinition PaymentDefinition { get; set; }

        [StringLength(MaxReferenceLength)]
        public virtual string Reference { get; set; }

        [Required]
        public virtual int ProcessOn { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual bool IsActive { get; set; }

        public SubsidyPaymentDefinition()
        {
            IsActive = true;
        }

        public SubsidyPaymentDefinition(string displayName, int paymentDefinitionId, string reference, decimal basePercent, int processOn, int companyId)
            :this()
        {
            DisplayName = displayName;
            CompanyId = companyId;
            PaymentDefinitionId = paymentDefinitionId;
            BasePercent = basePercent;
            Reference = reference;
            ProcessOn = processOn;
            SetDisplayNameNormalized();
        }

        protected virtual void SetDisplayNameNormalized()
        {
            DisplayName = DisplayName?.ToUpperInvariant();
        }
    }
}