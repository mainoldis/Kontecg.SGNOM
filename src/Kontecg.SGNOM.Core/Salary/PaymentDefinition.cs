using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Accounting;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Taxes;

namespace Kontecg.Salary
{
    [Table("payment_definitions", Schema = "sal")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PaymentDefinition : AuditedEntity, IMustHaveCompany, IPassivable
    {
        public const int MaxNameLength = 5;

        public const int MaxDescriptionLength = 250;

        public const int MaxSymbolLength = 3;

        public const int MaxScriptLength = 2000;

        public const int MaxObservationLength = 2000;

        /// <summary>
        ///     Max length of the <see cref="Reference" /> property.
        /// </summary>
        public const int MaxReferenceLength = AccountingFunctionDefinition.MaxReferenceLength;

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [StringLength(MaxSymbolLength)]
        public virtual string Symbol { get; set; }

        [Required]
        public virtual MathType MathType { get; set; }

        [Required]
        public virtual decimal Factor { get; set; }

        [Required]
        public virtual int AverageMonths { get; set; }

        [StringLength(MaxScriptLength)]
        public virtual string Formula  { get; set; }

        [Required]
        public virtual EmployeeSalaryForm SalaryForm { get; set; }

        [Required]
        public virtual PaymentSystem PaymentSystem { get; set; }

        [Required]
        public virtual WageAdjuster WageAdjuster { get; set; }

        [Required]
        public virtual AccountOperation Operation { get; set; }

        [StringLength(MaxReferenceLength)]
        public virtual string Reference { get; set; }

        [StringLength(MaxObservationLength)]
        public virtual string Observation { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        public virtual bool SumHoursForHolidayHistogram { get; set; } //Acumula Tiempo de Vacaciones

        public virtual bool SumAmountForHolidayHistogram { get; set; } //Acumula Importe de Vacaciones

        public virtual bool SumHoursForPaymentHistogram { get; set; } //Acumula Tiempo en el SC-4-08

        public virtual bool SumAmountForPaymentHistogram { get; set; } //Acumula Importe en el SC-4-08

        public virtual bool SumHoursForSocialSecurity { get; set; } //Acumula Tiempo de Subsidio en el SC-4-08

        public virtual bool SumAmountForSocialSecurity { get; set; } //Acumula Importe de Subsidio en el SC-4-08

        public virtual bool IsWageGuarantee { get; set; } //Garantía Salarial

        public virtual TaxAmountToContribute ContributeForCompanySocialSecurityTaxes { get; set; } //Aporta a la Seguridad Social

        public virtual TaxAmountToContribute ContributeForIncomeTaxes { get; set; } //Aporta Impuestos sobre Ingresos Personales

        public virtual TaxAmountToContribute ContributeForCompanyWorkforceTaxes { get; set; } //Aporta Impuestos a la Seguridad Social (12.5%)

        public virtual bool Absence { get; set; } // Ausencia

        public virtual bool LongTerm { get; set; } // Ausencia a largo plazo

        public virtual bool IsIncident { get; set; } // Incidencia

        [Required]
        public virtual bool IsActive { get; set; }

        public PaymentDefinition()
        {
            MathType = MathType.Percent;
            Factor = 100;
            WageAdjuster = WageAdjuster.None;
            SalaryForm = EmployeeSalaryForm.Royal;
            PaymentSystem = PaymentSystem.ByTime;
            
            ContributeForCompanySocialSecurityTaxes = TaxAmountToContribute.ContributeIncomePlusHoliday;
            ContributeForCompanyWorkforceTaxes = TaxAmountToContribute.ContributeIncomePlusHoliday;
            ContributeForIncomeTaxes = TaxAmountToContribute.ContributeIncome;

            IsActive = true;
        }

        public PaymentDefinition(int companyId, string name, string description)
            :this()
        {
            Name = name;
            Description = description;
            SalaryForm = EmployeeSalaryForm.Royal;
            PaymentSystem = PaymentSystem.ByTime;
            CompanyId = companyId;

            SetNameAndDescriptionNormalized();
        }

        public PaymentDefinition(int companyId, string name, string description, MathType mathType, decimal factor, string reference, int averageMonths = 0, WageAdjuster wageAjuster = WageAdjuster.Salary, EmployeeSalaryForm salaryForm = EmployeeSalaryForm.Royal, PaymentSystem paymentSystem = PaymentSystem.ByTime)
        {
            Name = name;
            Description = description;
            MathType = mathType;
            Factor = factor;
            AverageMonths = averageMonths;
            Reference = reference;
            WageAdjuster = wageAjuster;
            SalaryForm = salaryForm;
            PaymentSystem = paymentSystem;
            CompanyId = companyId;

            ContributeForCompanySocialSecurityTaxes = TaxAmountToContribute.ContributeIncomePlusHoliday;
            ContributeForCompanyWorkforceTaxes = TaxAmountToContribute.ContributeIncomePlusHoliday;
            ContributeForIncomeTaxes = TaxAmountToContribute.ContributeIncome;

            IsActive = true;

            SetNameAndDescriptionNormalized();
        }

        public PaymentDefinition(int companyId, string name, string description, string formula)
        {
            Name = name;
            Description = description;
            MathType = MathType.Formula;
            Factor = 1;
            Formula = formula;
            WageAdjuster = WageAdjuster.None;
            SalaryForm = EmployeeSalaryForm.Royal;
            PaymentSystem = PaymentSystem.ByTime;
            CompanyId = companyId;

            ContributeForCompanySocialSecurityTaxes = TaxAmountToContribute.ContributeIncomePlusHoliday;
            ContributeForCompanyWorkforceTaxes = TaxAmountToContribute.ContributeIncomePlusHoliday;
            ContributeForIncomeTaxes = TaxAmountToContribute.ContributeIncome;

            IsActive = true;

            SetNameAndDescriptionNormalized();
        }

        protected virtual void SetNameAndDescriptionNormalized()
        {
            Name = Name?.ToUpperInvariant();
            Description = Description?.ToUpperInvariant();
        }
    }
}
