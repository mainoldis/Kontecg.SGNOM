using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Accounting;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Taxes;
using NMoneys;

namespace Kontecg.Adjustments
{
    [Table("adjustment_definitions", Schema = "aju")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class AdjustmentDefinition : AuditedEntity, IMustHaveCompany, IPassivable
    {
        public const int MaxDescriptionLength = 250;

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
        public virtual int ProcessOn { get; set; }

        /// <summary>
        ///     Currency.
        ///     Default: CUP
        /// </summary>
        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        [Required]
        public virtual bool SumHoursForHolidayHistogram { get; set; } //Acumula Tiempo de Vacaciones

        [Required]
        public virtual bool SumAmountForHolidayHistogram { get; set; } //Acumula Importe de Vacaciones

        [Required]
        public virtual bool SumHoursForPaymentHistogram { get; set; } //Acumula Tiempo en el SC-4-08

        [Required]
        public virtual bool SumAmountForPaymentHistogram { get; set; } //Acumula Importe en el SC-4-08

        [Required]
        public virtual bool SumHoursForSocialSecurity { get; set; } //Acumula Tiempo de Subsidio en el SC-4-08

        [Required]
        public virtual bool SumAmountForSocialSecurity { get; set; } //Acumula Importe de Subsidio en el SC-4-08

        [Required]
        public virtual TaxAmountToContribute ContributeForCompanySocialSecurityTaxes { get; set; } //Aporta a la Seguridad Social

        [Required]
        public virtual TaxAmountToContribute ContributeForIncomeTaxes { get; set; } //Aporta Impuestos sobre Ingresos Personales

        [Required]
        public virtual TaxAmountToContribute ContributeForCompanyWorkforceTaxes { get; set; } //Aporta Impuestos a la Seguridad Social (12.5%)

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual bool IsActive { get; set; }

        public AdjustmentDefinition()
        {
            IsActive = true;
            
        }

        /// <inheritdoc />
        public AdjustmentDefinition(int companyId, int code, string description, int processOn, string reference, bool sumHoursForHolidayHistogram = true, bool sumAmountForHolidayHistogram = true, bool sumHoursForPaymentHistogram = true, bool sumAmountForPaymentHistogram = true, bool sumHoursForSocialSecurity = true, bool sumAmountForSocialSecurity = true, TaxAmountToContribute contributeForCompanySocialSecurityTaxes = TaxAmountToContribute.ContributeIncomePlusHoliday, TaxAmountToContribute contributeForIncomeTaxes = TaxAmountToContribute.ContributeIncome, TaxAmountToContribute contributeForCompanyWorkforceTaxes = TaxAmountToContribute.ContributeIncomePlusHoliday, CurrencyIsoCode currency = CurrencyIsoCode.CUP)
            :this()
        {
            CompanyId = companyId;
            Code = code;
            Description = description;
            ProcessOn = processOn;
            Reference = reference;
            Currency = currency;
            SumHoursForHolidayHistogram = sumHoursForHolidayHistogram;
            SumAmountForHolidayHistogram = sumAmountForHolidayHistogram;
            SumHoursForPaymentHistogram = sumHoursForPaymentHistogram;
            SumAmountForPaymentHistogram = sumAmountForPaymentHistogram;
            SumHoursForSocialSecurity = sumHoursForSocialSecurity;
            SumAmountForSocialSecurity = sumAmountForSocialSecurity;
            ContributeForCompanySocialSecurityTaxes = contributeForCompanySocialSecurityTaxes;
            ContributeForIncomeTaxes = contributeForIncomeTaxes;
            ContributeForCompanyWorkforceTaxes = contributeForCompanyWorkforceTaxes;

            SetNameAndDescriptionNormalized();
        }

        protected virtual void SetNameAndDescriptionNormalized()
        {
            Description = Description?.ToUpperInvariant();
        }
    }
}
