using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Itenso.TimePeriod;
using Kontecg.Data;
using Kontecg.Domain;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Salary;

namespace Kontecg.HumanResources
{
    [Table("person_background_times", Schema = "docs")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PersonBackgroundTime : AuditedEntity, IMustHaveCompany
    {
        public virtual int CompanyId { get; set; }

        /// <summary>
        ///     Id of the Person.
        /// </summary>
        [Required]
        public virtual long PersonId { get; set; }

        [NotMapped]
        public virtual Person Person { get; set; }

        [Required]
        public virtual Guid GroupId { get; set; }

        [Required]
        public virtual int Year { get; set; }

        public virtual YearMonth? Month { get; set; }

        [Required]
        [StringLength(PaymentDefinition.MaxNameLength)]
        public virtual string Key { get; set; }

        [Required]
        public virtual decimal Hours { get; set; }

        [Required]
        public virtual GenerationSystemData Kind { get; set; }

        public PersonBackgroundTime(long personId, Guid groupId, int year, YearMonth? month, string key, decimal hours)
        {
            PersonId = personId;
            GroupId = groupId;
            Year = year;
            Month = month;
            Key = key;
            Hours = hours;
        }
    }
}
