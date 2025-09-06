using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Kontecg.Accounting;
using NMoneys;
using System;

namespace Kontecg.SocialSecurity
{
    [Table("subsidy_notes", Schema = "sub")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class SubsidyNote : AuditedEntity<long>, IMustHaveCompany
    {
        public virtual long DocumentId { get; set; }

        [Required]
        [ForeignKey("DocumentId")]
        public virtual SubsidyDocument Document { get; set; }

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

        public virtual decimal? ReservedForHoliday { get; set; }

        public virtual Money? AmountReservedForHoliday { get; set; }

        [Required]
        public virtual AccountingNoteStatus Status { get; set; }

        public SubsidyNote()
        {
            Status = AccountingNoteStatus.ToAnalyze;
        }
    }
}
