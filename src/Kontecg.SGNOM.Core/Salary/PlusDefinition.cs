using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Accounting;
using Kontecg.Domain;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Taxes;

namespace Kontecg.Salary
{
    [Table("plus_definitions", Schema = "sal")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PlusDefinition : AuditedEntity, IMustHaveCompany, IPassivable
    {
        /// <summary>
        ///     Max length of the <see cref="Reference" /> property.
        /// </summary>
        public const int MaxReferenceLength = AccountingFunctionDefinition.MaxReferenceLength;

        [Required]
        [StringLength(PaymentDefinition.MaxNameLength)]
        public virtual string Name { get; set; }

        [Required]
        public virtual ScopeData Scope { get; set; }

        [Required]
        [StringLength(PaymentDefinition.MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [StringLength(MaxReferenceLength)]
        public virtual string Reference { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        public virtual bool SumHoursForHolidayHistogram { get; set; } //Acumula Tiempo de Vacaciones

        public virtual bool SumAmountForHolidayHistogram { get; set; } //Acumula Importe de Vacaciones

        public virtual bool SumHoursForPaymentHistogram { get; set; } //Acumula Tiempo en el SC-4-08

        public virtual bool SumAmountForPaymentHistogram { get; set; } //Acumula Importe en el SC-4-08

        public virtual bool SumHoursForSocialSecurity { get; set; } //Acumula Tiempo de Subsidio en el SC-4-08

        public virtual bool SumAmountForSocialSecurity { get; set; } //Acumula Importe de Subsidio en el SC-4-08

        public virtual TaxAmountToContribute ContributeForCompanySocialSecurityTaxes { get; set; } //Aporta a la Seguridad Social

        public virtual TaxAmountToContribute ContributeForIncomeTaxes { get; set; } //Aporta Impuestos sobre Ingresos Personales

        public virtual TaxAmountToContribute ContributeForCompanyWorkforceTaxes { get; set; } //Aporta Impuestos a la Seguridad Social (12.5%)

        public virtual bool IsActive { get; set; }

        public PlusDefinition()
        {
            ContributeForCompanySocialSecurityTaxes = TaxAmountToContribute.ContributeIncomePlusHoliday;
            ContributeForCompanyWorkforceTaxes = TaxAmountToContribute.ContributeIncomePlusHoliday;
            ContributeForIncomeTaxes = TaxAmountToContribute.ContributeIncome;

            IsActive = true;
        }

        public PlusDefinition(string name, ScopeData scope, string description, int companyId, string reference)
            :this()
        {
            Name = name;
            Scope = scope;
            Description = description;
            Reference = reference;
            CompanyId = companyId;

            SetNameAndDescriptionNormalized();
        }

        protected virtual void SetNameAndDescriptionNormalized()
        {
            Name = Name?.ToUpperInvariant();
            Description = Description?.ToUpperInvariant();
        }
    }
}
