using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using Kontecg.Accounting;

namespace Kontecg.Adjustments
{
    [Table("adjustment_document_transitions", Schema = "aju")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class AdjustmentDocumentTransition : CreationAuditedEntity<long>, IMustHaveCompany
    {
        public virtual long AdjustmentDocumentId { get; set; }

        [Required]
        [ForeignKey("AdjustmentDocumentId")]
        public virtual AdjustmentDocument AdjustmentDocument { get; set; }

        public virtual long DocumentId { get; set; }

        /// <inheritdoc />
        public virtual int CompanyId { get; set; }

        public virtual AccountingNoteStatus Status { get; set; }

        /// <inheritdoc />
        public AdjustmentDocumentTransition(long adjustmentDocumentId, long documentId, AccountingNoteStatus status)
        {
            AdjustmentDocumentId = adjustmentDocumentId;
            DocumentId = documentId;
            Status = status;
        }
    }
}