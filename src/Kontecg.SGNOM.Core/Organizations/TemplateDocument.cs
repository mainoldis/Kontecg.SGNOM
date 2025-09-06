using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Identity;
using Kontecg.MultiCompany;
using Kontecg.Workflows;

namespace Kontecg.Organizations
{
    [Table("template_documents", Schema = "est")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class TemplateDocument : AuditedEntity, IMustHaveCompany, IMustHaveReview
    {
        [Required]
        public virtual int DocumentDefinitionId { get; set; }

        [Required]
        public virtual DateTime MadeOn { get; set; }

        /// <inheritdoc />
        [Required]
        public string Code { get; set; }

        public virtual int MadeById { get; set; }

        [Required]
        [ForeignKey("MadeById")]
        public virtual SignOnDocument MadeBy { get; set; }

        public virtual int ApprovedById { get; set; }

        [Required]
        [ForeignKey("ApprovedById")]
        public virtual SignOnDocument ApprovedBy { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual ReviewStatus Review { get; set; }

        public virtual List<Template> Templates { get; set; }

        public TemplateDocument()
        {
            Templates = new List<Template>();
            Review = ReviewStatus.ForReview;
        }

        public TemplateDocument(int documentDefinitionId, DateTime madeOn, int madeById, int approvedById, int companyId)
            :this()
        {
            DocumentDefinitionId = documentDefinitionId;
            MadeOn = madeOn;
            MadeById = madeById;
            ApprovedById = approvedById;
            CompanyId = companyId;
        }
    }
}
