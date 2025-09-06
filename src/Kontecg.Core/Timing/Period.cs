using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Itenso.TimePeriod;
using Kontecg.Domain;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Extensions;
using Kontecg.MultiCompany;

namespace Kontecg.Timing
{
    [Table("periods", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Company)]
    [Serializable]
    public class Period : AuditedEntity, IMustHaveCompany, IMustHaveReferenceGroup
    {
        /// <summary>
        ///     Max length of the <see cref="ReferenceGroup" /> property.
        /// </summary>
        public const int MaxReferenceLength = 15;

        [Required]
        public virtual int Year { get; set; }

        [Required]
        public virtual YearMonth Month { get; set; }

        [Required]
        public virtual YearQuarter Quarter { get; set; }

        [Required]
        public virtual DateTime Since { get; set; }

        [Required]
        public virtual DateTime Until { get; set; }

        [Required]
        public virtual PeriodStatus Status { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        [StringLength(MaxReferenceLength)]
        public virtual string ReferenceGroup { get; set; }

        public Period()
        {
            Status = PeriodStatus.Opened;
            Month timeRange = new(Now.Today);
            Since = timeRange.Start;
            Until = timeRange.End;
            Year = timeRange.Year;
            Month = timeRange.YearMonth;
            Quarter = TimeTool.GetQuarterOfMonth(timeRange.YearMonth);
            ReferenceGroup = KontecgCoreConsts.DefaultReferenceGroup;
        }

        public Period(string referenceGroup, ITimeCalendar calendar, int? year, YearMonth? month)
        {
            Status = PeriodStatus.Opened;
            Month timeRange = year != null && month != null ? new(year.Value, month.Value, calendar) : new(Now.Today, calendar);
            Since = timeRange.Start;
            Until = timeRange.End;
            Year = timeRange.Year;
            Month = timeRange.YearMonth;
            Quarter = TimeTool.GetQuarterOfMonth(timeRange.YearMonth);
            ReferenceGroup = referenceGroup.IsNullOrEmpty() ? KontecgCoreConsts.DefaultReferenceGroup : referenceGroup;
        }

        public Period(string referenceGroup, ITimeCalendar calendar, DateTime since, DateTime until)
        {
            Status = PeriodStatus.Opened;
            Month timeRange = new(since, calendar);
            Quarter = TimeTool.GetQuarterOfMonth(timeRange.YearMonth);
            Month = timeRange.YearMonth;
            Year = timeRange.Year;
            Since = since;
            Until = until;
            ReferenceGroup = referenceGroup.IsNullOrEmpty() ? KontecgCoreConsts.DefaultReferenceGroup : referenceGroup;
        }
    }
}
