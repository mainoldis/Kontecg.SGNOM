using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Accounting;
using System.ComponentModel.DataAnnotations;

namespace Kontecg.SocialSecurity
{
    [Table("subsidy_note_transitions", Schema = "sub")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class SubsidyNoteTransition : CreationAuditedEntity<long>, IMustHaveCompany
    {
        public virtual long SubsidyNoteId { get; set; }

        [Required]
        [ForeignKey("SubsidyNoteId")]
        public virtual SubsidyNote SubsidyNote { get; set; }

        public virtual long DocumentId { get; set; }

        /// <inheritdoc />
        public virtual int CompanyId { get; set; }

        public virtual AccountingNoteStatus Status { get; set; }

        /// <inheritdoc />
        public SubsidyNoteTransition(long subsidyNoteId, long documentId, AccountingNoteStatus status)
        {
            SubsidyNoteId = subsidyNoteId;
            DocumentId = documentId;
            Status = status;
        }
    }
}