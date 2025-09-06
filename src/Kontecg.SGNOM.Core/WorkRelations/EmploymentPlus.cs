using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Salary;
using NMoneys;

namespace Kontecg.WorkRelations
{
    [Table("employment_pluses", Schema = "rel")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class EmploymentPlus : CreationAuditedEntity<long>, IMustHaveCompany
    {
        public virtual long EmploymentId { get; set; }

        [Required]
        [ForeignKey("EmploymentId")]
        public virtual EmploymentDocument Employment { get; set; }

        public virtual int PlusDefinitionId { get; set; }

        [Required]
        [ForeignKey("PlusDefinitionId")]
        public virtual PlusDefinition PlusDefinition { get; set; }

        [Required]
        public virtual Money Amount { get; set; }

        [Required]
        public virtual decimal RatePerHour { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        public EmploymentPlus()
        {
        }

        public EmploymentPlus(int companyId, long employmentId, int plusDefinitionId, decimal amount)
        {
            CompanyId = companyId;
            EmploymentId = employmentId;
            PlusDefinitionId = plusDefinitionId;

            SetAmount(amount);
            SetRatePerHour();
        }

        public virtual void SetAmount(decimal amount)
        {
            if(amount <= 0)
                throw new KontecgException("Money can't be less than 0");
            Amount = new Money(amount, KontecgCoreConsts.DefaultCurrency);
        }

        public virtual void SetAmount(Money amount)
        {
            if (amount <= Money.Zero(amount.CurrencyCode))
                throw new KontecgException("Money can't be less than 0");

            Amount = amount;
        }

        public virtual void SetRatePerHour(decimal? averageWorkingHoursPerPeriod = null)
        {
            if(Amount == Money.Zero(KontecgCoreConsts.DefaultCurrency))
                throw new KontecgException("Money must be different from 0");

            RatePerHour = decimal.Divide(Amount.Amount,
                averageWorkingHoursPerPeriod > 0 && averageWorkingHoursPerPeriod  != SGNOMConsts.DefaultAverageWorkingHoursPerPeriod
                    ? averageWorkingHoursPerPeriod.Value
                    : SGNOMConsts.DefaultAverageWorkingHoursPerPeriod);
        }
    }
}
