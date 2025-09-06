using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.Timing
{
    [Table("work_shifts", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class WorkShift : AuditedEntity, IMustHaveCompany, IPassivable
    {
        public const int MaxDisplayNameLength = 50;

        public const int MaxTimesLength = 1000;

        public const int MaxLegalLength = 100;

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public virtual int RegimeId { get; set; }

        [Required]
        [ForeignKey("RegimeId")]
        public virtual WorkRegime Regime { get; set; }

        [Required]
        public virtual DateTime StartDate { get; set; }

        [Required]
        [StringLength(MaxTimesLength)]
        public virtual string HoursWorking { get; set; }

        [StringLength(MaxTimesLength)]
        public virtual string RestingTimesPerShift { get; set; }

        [Required]
        public virtual decimal AverageHoursPerShift { get; set; }

        [StringLength(MaxLegalLength)]
        public virtual string Legal { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual int VisualOrder { get; set; }

        public virtual bool IsActive { get; set; }

        public WorkShift()
        {
            IsActive = true;
            VisualOrder = 0;
        }

        public WorkShift(string displayName, WorkRegime regime, DateTime startDate, string hoursWorking, string restingTimesPerShift, decimal averageHoursPerShift, string legal)
            : this()
        {
            DisplayName = displayName;
            Regime = regime;
            StartDate = startDate;
            HoursWorking = hoursWorking;
            RestingTimesPerShift = restingTimesPerShift;
            AverageHoursPerShift = averageHoursPerShift;
            Legal = legal;
        }

        public WorkShift(string displayName, int regimeId, DateTime startDate, string hoursWorking, string restingTimesPerShift, decimal averageHoursPerShift, string legal)
            : this()
        {
            DisplayName = displayName;
            RegimeId = regimeId;
            StartDate = startDate;
            HoursWorking = hoursWorking;
            RestingTimesPerShift = restingTimesPerShift;
            AverageHoursPerShift = averageHoursPerShift;
            Legal = legal;
        }

        public WorkShift(string displayName, WorkRegime regime, DateTime startDate, string hoursWorking, string restingTimesPerShift, decimal averageHoursPerShift, string legal, int visualOrder)
        {
            DisplayName = displayName;
            Regime = regime;
            StartDate = startDate;
            HoursWorking = hoursWorking;
            RestingTimesPerShift = restingTimesPerShift;
            AverageHoursPerShift = averageHoursPerShift;
            Legal = legal;
            IsActive = true;
            VisualOrder = visualOrder;
        }

        public WorkShift(string displayName, int regimeId, DateTime startDate, string hoursWorking, string restingTimesPerShift, decimal averageHoursPerShift, string legal, int visualOrder)
        {
            DisplayName = displayName;
            RegimeId = regimeId;
            StartDate = startDate;
            HoursWorking = hoursWorking;
            RestingTimesPerShift = restingTimesPerShift;
            AverageHoursPerShift = averageHoursPerShift;
            Legal = legal;
            IsActive = true;
            VisualOrder = visualOrder;
        }
    }
}
