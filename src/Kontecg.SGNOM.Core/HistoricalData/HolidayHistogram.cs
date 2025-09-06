using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Kontecg.Accounting;
using Kontecg.MultiCompany;
using System;
using NMoneys;

namespace Kontecg.HistoricalData
{
    [Table("holiday_histogram", Schema = "vac")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class HolidayHistogram : CreationAuditedEntity<long>, IMustHaveCompany
    {
        [Required]
        public virtual int DocumentDefinitionId { get; set; }

        [Required]
        public virtual int DocumentId { get; set; }

        [Required]
        public virtual long PersonId { get; set; }

        [Required]
        public virtual Guid GroupId { get; set; }

        [Required]
        public virtual DateTime Since { get; set; }

        [Required]
        public virtual DateTime Until { get; set; }

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

        [Required]
        public virtual int CompanyId { get; set; }

        public HolidayHistogram()
        {
            Status = AccountingNoteStatus.ToAnalyze;
        }

        public HolidayHistogram(
            int companyId,
            int documentDefinitionId,
            int accountingDocumentId,
            long personId,
            Guid groupId,
            DateTime since,
            DateTime until,
            decimal hours,
            Money amount)
        {
            CompanyId = companyId;
            DocumentDefinitionId = documentDefinitionId;
            DocumentId = accountingDocumentId;
            PersonId = personId;
            GroupId = groupId;
            Since = since;
            Until = until;
            Hours = hours;
            Amount = amount;
            Currency = amount.CurrencyCode;
        }
    }
}
