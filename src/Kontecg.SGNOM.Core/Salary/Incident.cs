using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Itenso.TimePeriod;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.Salary
{
    [Table("incidents", Schema = "sal")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class Incident : AuditedEntity<Guid>, IMustHaveCompany
    {
        public const int MaxAnnotationsLength = 500;

        [Required]
        public virtual long PersonId { get; set; }

        [Required]
        public virtual long EmploymentId { get; set; }

        [Required]
        [StringLength(PaymentDefinition.MaxNameLength)]
        public virtual string Key { get; set; }

        [Required]
        public virtual DateTime Start { get; set; }

        [Required]
        public virtual DateTime End { get; set; }

        [Required]
        public virtual decimal Hours { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual bool Tight { get; set; }

        [StringLength(MaxAnnotationsLength)]
        public virtual string Annotation { get; set; }

        public Incident()
        {
        }

        public Incident(long employmentId, long personId, string key, DateTime start, DateTime end, string annotation = null, bool tight = false)
        {
            EmploymentId = employmentId;
            PersonId = personId;
            Key = key;
            Start = start;
            End = end;
            var calculator = new DurationCalculator();
            Hours = (decimal) calculator.CalcDuration(Start, End).TotalHours;
            Tight = tight;
            Annotation = annotation;
        }

        public Incident(long employmentId, long personId, string key, DateTime start, decimal hours, string annotation = null, bool tight = false)
        {
            EmploymentId = employmentId;
            PersonId = personId;
            Key = key;
            Start = start;
            End = start.AddHours((double) hours);
            Hours = hours;
            Tight = tight;
            Annotation = annotation;
        }
    }
}
