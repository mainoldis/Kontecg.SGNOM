using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Workflows;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Kontecg.Accounting
{
    [Table("accounting_voucher_documents", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public class AccountingVoucherDocument : AuditedEntity, IMustHaveCompany, IMustHaveReview
    {
        //RULE: 6- Estado

        public const int MaxCodeLength = 5;

        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int MaxDescriptionLength = 250;

        public virtual int DocumentDefinitionId { get; set; }

        [Required]
        [ForeignKey("DocumentDefinitionId")]
        public virtual DocumentDefinition DocumentDefinition { get; set; }

        [Required]
        public DateTime MadeOn { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        public virtual int DocumentId { get; set; }

        [Required]
        [ForeignKey("DocumentId")]
        public virtual AccountingDocument Document { get; set; }

        public virtual int CompanyId { get; set; }

        public virtual List<AccountingVoucherNote> Notes { get; set; }

        [Required]
        public virtual ReviewStatus Review { get; set; }

        public AccountingVoucherDocument()
        {
            Notes = new List<AccountingVoucherNote>();
            Review = ReviewStatus.ForReview;
        }

        public AccountingVoucherDocument(DocumentDefinition documentDefinition, AccountingDocument document, string description, DateTime madeOn, string code)
            : this()
        {
            Check.NotNull(document.Period, nameof(document.Period));
            DocumentDefinition = documentDefinition;
            Document = document;
            Description = description;
            MadeOn = madeOn;
            Code = code;
        }

        public AccountingVoucherDocument(int documentDefinitionId, int documentId, string description, DateTime madeOn, string code)
            : this()
        {
            DocumentDefinitionId = documentDefinitionId;
            DocumentId = documentId;
            Description = description;
            MadeOn = madeOn;
            Code = code;
        }
    }
}
