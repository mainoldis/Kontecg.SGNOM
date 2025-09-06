using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("person_holiday_schedules", Schema = "docs")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PersonHolidaySchedule : AuditedEntity
    {
        private static readonly int[] ValidValues = [0, 5, 7, 10, 15, 30];

        [Required]
        public virtual long PersonId { get; set; }

        [NotMapped]
        public virtual Person Person { get; set; }

        [Required]
        public virtual int Year { get; set; }

        public virtual int JanuaryFirstFortnightCalendarDays { get; set; }
        public virtual int JanuarySecondFortnightCalendarDays { get; set; }
        public virtual int? JanuaryFirstFortnightDaysOff { get; set; }
        public virtual int? JanuarySecondFortnightDaysOf { get; set; }
        public virtual int FebruaryFirstFortnightCalendarDays { get; set; }
        public virtual int FebruarySecondFortnightCalendarDays { get; set; }
        public virtual int? FebruaryFirstFortnightDaysOff { get; set; }
        public virtual int? FebruarySecondFortnightDaysOf { get; set; }
        public virtual int MarchFirstFortnightCalendarDays { get; set; }
        public virtual int MarchSecondFortnightCalendarDays { get; set; }
        public virtual int? MarchFirstFortnightDaysOff { get; set; }
        public virtual int? MarchSecondFortnightDaysOf { get; set; }
        public virtual int AprilFirstFortnightCalendarDays { get; set; }
        public virtual int AprilSecondFortnightCalendarDays { get; set; }
        public virtual int? AprilFirstFortnightDaysOff { get; set; }
        public virtual int? AprilSecondFortnightDaysOf { get; set; }
        public virtual int MayFirstFortnightCalendarDays { get; set; }
        public virtual int MaySecondFortnightCalendarDays { get; set; }
        public virtual int? MayFirstFortnightDaysOff { get; set; }
        public virtual int? MaySecondFortnightDaysOf { get; set; }
        public virtual int JuneFirstFortnightCalendarDays { get; set; }
        public virtual int JuneSecondFortnightCalendarDays { get; set; }
        public virtual int? JuneFirstFortnightDaysOff { get; set; }
        public virtual int? JuneSecondFortnightDaysOf { get; set; }
        public virtual int JulyFirstFortnightCalendarDays { get; set; }
        public virtual int JulySecondFortnightCalendarDays { get; set; }
        public virtual int? JulyFirstFortnightDaysOff { get; set; }
        public virtual int? JulySecondFortnightDaysOf { get; set; }
        public virtual int AugustFirstFortnightCalendarDays { get; set; }
        public virtual int AugustSecondFortnightCalendarDays { get; set; }
        public virtual int? AugustFirstFortnightDaysOff { get; set; }
        public virtual int? AugustSecondFortnightDaysOf { get; set; }
        public virtual int SeptemberFirstFortnightCalendarDays { get; set; }
        public virtual int SeptemberSecondFortnightCalendarDays { get; set; }
        public virtual int? SeptemberFirstFortnightDaysOff { get; set; }
        public virtual int? SeptemberSecondFortnightDaysOf { get; set; }
        public virtual int OctoberFirstFortnightCalendarDays { get; set; }
        public virtual int OctoberSecondFortnightCalendarDays { get; set; }
        public virtual int? OctoberFirstFortnightDaysOff { get; set; }
        public virtual int? OctoberSecondFortnightDaysOf { get; set; }
        public virtual int NovemberFirstFortnightCalendarDays { get; set; }
        public virtual int NovemberSecondFortnightCalendarDays { get; set; }
        public virtual int? NovemberFirstFortnightDaysOff { get; set; }
        public virtual int? NovemberSecondFortnightDaysOf { get; set; }
        public virtual int DecemberFirstFortnightCalendarDays { get; set; }
        public virtual int DecemberSecondFortnightCalendarDays { get; set; }
        public virtual int? DecemberFirstFortnightDaysOff { get; set; }
        public virtual int? DecemberSecondFortnightDaysOf { get; set; }

        /// <inheritdoc />
        public PersonHolidaySchedule(long personId, int year, int januaryFirstFortnightCalendarDays = 0, int januarySecondFortnightCalendarDays = 0, int februaryFirstFortnightCalendarDays = 0, int februarySecondFortnightCalendarDays = 0, int marchFirstFortnightCalendarDays = 0, int marchSecondFortnightCalendarDays = 0, int aprilFirstFortnightCalendarDays = 0, int aprilSecondFortnightCalendarDays = 0, int mayFirstFortnightCalendarDays = 0, int maySecondFortnightCalendarDays = 0, int juneFirstFortnightCalendarDays = 0, int juneSecondFortnightCalendarDays = 0, int julyFirstFortnightCalendarDays = 0, int julySecondFortnightCalendarDays = 0, int augustFirstFortnightCalendarDays = 0, int augustSecondFortnightCalendarDays = 0, int septemberFirstFortnightCalendarDays = 0, int septemberSecondFortnightCalendarDays = 0, int octoberFirstFortnightCalendarDays = 0, int octoberSecondFortnightCalendarDays = 0, int novemberFirstFortnightCalendarDays = 0, int novemberSecondFortnightCalendarDays = 0, int decemberFirstFortnightCalendarDays = 0, int decemberSecondFortnightCalendarDays = 0)
        {
            PersonId = personId;
            Year = year;
            JanuaryFirstFortnightCalendarDays = januaryFirstFortnightCalendarDays;
            JanuarySecondFortnightCalendarDays = januarySecondFortnightCalendarDays;
            FebruaryFirstFortnightCalendarDays = februaryFirstFortnightCalendarDays;
            FebruarySecondFortnightCalendarDays = februarySecondFortnightCalendarDays;
            MarchFirstFortnightCalendarDays = marchFirstFortnightCalendarDays;
            MarchSecondFortnightCalendarDays = marchSecondFortnightCalendarDays;
            AprilFirstFortnightCalendarDays = aprilFirstFortnightCalendarDays;
            AprilSecondFortnightCalendarDays = aprilSecondFortnightCalendarDays;
            MayFirstFortnightCalendarDays = mayFirstFortnightCalendarDays;
            MaySecondFortnightCalendarDays = maySecondFortnightCalendarDays;
            JuneFirstFortnightCalendarDays = juneFirstFortnightCalendarDays;
            JuneSecondFortnightCalendarDays = juneSecondFortnightCalendarDays;
            JulyFirstFortnightCalendarDays = julyFirstFortnightCalendarDays;
            JulySecondFortnightCalendarDays = julySecondFortnightCalendarDays;
            AugustFirstFortnightCalendarDays = augustFirstFortnightCalendarDays;
            AugustSecondFortnightCalendarDays = augustSecondFortnightCalendarDays;
            SeptemberFirstFortnightCalendarDays = septemberFirstFortnightCalendarDays;
            SeptemberSecondFortnightCalendarDays = septemberSecondFortnightCalendarDays;
            OctoberFirstFortnightCalendarDays = octoberFirstFortnightCalendarDays;
            OctoberSecondFortnightCalendarDays = octoberSecondFortnightCalendarDays;
            NovemberFirstFortnightCalendarDays = novemberFirstFortnightCalendarDays;
            NovemberSecondFortnightCalendarDays = novemberSecondFortnightCalendarDays;
            DecemberFirstFortnightCalendarDays = decemberFirstFortnightCalendarDays;
            DecemberSecondFortnightCalendarDays = decemberSecondFortnightCalendarDays;
        }
    }
}
