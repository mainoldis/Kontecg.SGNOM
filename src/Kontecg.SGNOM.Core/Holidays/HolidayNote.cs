using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using NMoneys;
using Kontecg.Accounting;

namespace Kontecg.Holidays
{
    [Table("holiday_notes", Schema = "vac")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class HolidayNote : AuditedEntity<long>, IMustHaveCompany
    {
        public virtual long DocumentId { get; set; }

        [Required]
        [ForeignKey("DocumentId")]
        public virtual HolidayDocument Document { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        public virtual int? PeriodId { get; set; }

        [Required]
        public virtual DateTime Since { get; set; }

        [Required]
        public virtual DateTime Until { get; set; }

        [Required]
        public virtual int Days { get; set; }

        [Required]
        public virtual decimal Hours { get; set; }

        [Required]
        public virtual Money Amount { get; set; }

        /// <summary>
        ///     Currency.
        ///     Default: CUP
        /// </summary>
        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        [Required]
        public virtual AccountingNoteStatus Status { get; set; }

        public HolidayNote()
        {
            Status = AccountingNoteStatus.ToAnalyze;
        }
    }
}
