using System;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using Kontecg.Workflows;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Kontecg.Timing;
using Kontecg.WorkRelations;

namespace Kontecg.Claims
{
    [Table("claim_documents", Schema = "rec")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class ClaimDocument : CreationAuditedEntity<long>, IMustHaveCompany, IMustHaveReview
    {
        public const int MaxCodeLength = 20;

        [Required]
        public virtual int DocumentDefinitionId { get; set; }

        [Required]
        public virtual DateTime MadeOn { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        [Required]
        public virtual long PersonId { get; set; }

        [Required]
        [ForeignKey("EmploymentId")]
        public virtual EmploymentDocument EmploymentDocument { get; set; }

        [Required]
        public virtual Guid GroupId { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual ReviewStatus Review { get; set; }

        public ClaimDocument()
        {
            MadeOn = Clock.Now;
            Review = ReviewStatus.ForReview;
        }
    }
}