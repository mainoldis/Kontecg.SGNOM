using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Accounting;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.WorkRelations;
using NMoneys;

namespace Kontecg.Salary
{
    [Table("time_distribution_pluses", Schema = "sal")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class TimeDistributionPlus : AuditedEntity<long>, IMustHaveCompany
    {
        public virtual long TimeDistributionId { get; set; }

        [Required]
        [ForeignKey("TimeDistributionId")]
        public virtual TimeDistribution TimeDistribution { get; set; }

        public virtual long EmploymentPlusId { get; set; }

        [Required]
        [ForeignKey("EmploymentPlusId")]
        public virtual EmploymentPlus EmploymentPlus { get; set; }

        public virtual Money? Amount { get; set; }

        /// <summary>
        ///     Currency.
        ///     Default: CUP
        /// </summary>
        public virtual CurrencyIsoCode? Currency { get; set; }

        public virtual decimal? ReservedForHoliday { get; set; }

        public virtual Money? AmountReservedForHoliday { get; set; }

        public virtual decimal? RatePerHour { get; set; }

        [Required]
        public virtual AccountingNoteStatus Status { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        public TimeDistributionPlus()
        {
            Status = AccountingNoteStatus.ToAnalyze;
        }

        public TimeDistributionPlus(long timeDistributionId, long employmentPlusId)
            : this()
        {
            TimeDistributionId = timeDistributionId;
            EmploymentPlusId = employmentPlusId;
        }

        public virtual void SetRatePerHour(decimal? averageWorkingHoursPerPeriod = null)
        {
            if (EmploymentPlus == null)
                throw new KontecgException("You must set EmploymentPlus first.");

            if (EmploymentPlus?.Amount == Money.Zero(KontecgCoreConsts.DefaultCurrency))
                throw new KontecgException("Money must be different from 0");

            RatePerHour = decimal.Divide(EmploymentPlus.Amount.Amount,
                averageWorkingHoursPerPeriod > 0
                    ? averageWorkingHoursPerPeriod.Value
                    : SGNOMConsts.DefaultAverageWorkingHoursPerPeriod);
        }
    }
}
