using Kontecg.Workflows;
using System.ComponentModel.DataAnnotations;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Kontecg.Timing;
using System;

namespace Kontecg.Accounting
{
    [Table("accounting_documents", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public class AccountingDocument : AuditedEntity, IMustHaveCompany, IMustHaveReview
    {
        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int MaxDescriptionLength = 150;

        /// <summary>
        ///     Max length of the <see cref="Code" /> property.
        /// </summary>
        public const int MaxCodeLength = 10;

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        public virtual int DocumentDefinitionId { get; set; }

        [Required]
        [ForeignKey("DocumentDefinitionId")]
        public virtual DocumentDefinition DocumentDefinition { get; set; }

        [Required]
        public DateTime MadeOn { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        public virtual int PeriodId { get; set; }

        [Required]
        [ForeignKey("PeriodId")]
        public virtual Period Period { get; set; }

        public virtual int CompanyId { get; set; }

        public virtual List<AccountingVoucherDocument> Vouchers { get; set; }

        [Required]
        public virtual ReviewStatus Review { get; set; }

        [Required]
        public virtual bool Exported { get; set; }

        public AccountingDocument()
        {
            MadeOn = Clock.Now;
            Vouchers = new List<AccountingVoucherDocument>();
            Review = ReviewStatus.ForReview;
            Exported = false;
        }

        public AccountingDocument(int documentDefinitionId, string description, int periodId)
            : this()
        {
            DocumentDefinitionId = documentDefinitionId;
            PeriodId = periodId;
            Description = description;
        }

        public AccountingDocument(int documentDefinitionId, string description, int periodId, DateTime madeOn, string code)
        {
            DocumentDefinitionId = documentDefinitionId;
            PeriodId = periodId;
            Description = description;
            MadeOn = madeOn;
            Code = code;
            Vouchers = new List<AccountingVoucherDocument>();
            Review = ReviewStatus.ForReview;
            Exported = false;
        }
    }
}
