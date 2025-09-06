using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Accounting;
using System.ComponentModel.DataAnnotations;

namespace Kontecg.Holidays
{
    [Table("holiday_note_transitions", Schema = "vac")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class HolidayNoteTransition : CreationAuditedEntity<long>, IMustHaveCompany
    {
        public virtual long HolidayNoteId { get; set; }

        [Required]
        [ForeignKey("HolidayNoteId")]
        public virtual HolidayNote HolidayNote { get; set; }

        public virtual long DocumentId { get; set; }

        /// <inheritdoc />
        public virtual int CompanyId { get; set; }

        public virtual AccountingNoteStatus Status { get; set; }

        /// <inheritdoc />
        public HolidayNoteTransition(long holidayNoteId, long documentId, AccountingNoteStatus status)
        {
            HolidayNoteId = holidayNoteId;
            DocumentId = documentId;
            Status = status;
        }
    }
}