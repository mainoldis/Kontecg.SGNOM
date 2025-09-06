using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("person_law_penalties", Schema = "docs")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PersonLawPenalty : AuditedEntity
    {
        [Required]
        public virtual long PersonId { get; set; }

        [NotMapped]
        public virtual Person Person { get; set; }

        public virtual int CauseId { get; set; }

        [Required]
        [ForeignKey("CauseId")]
        public virtual LawPenaltyCauseDefinition Cause { get; set; }

        public virtual int LawPenaltyDefinitionId { get; set; }

        [Required]
        [ForeignKey("LawPenaltyDefinitionId")]
        public virtual LawPenaltyDefinition LawPenalty { get; set; }

        [Required]
        public virtual DateTime Notification { get; set; }

        [Required]
        public virtual DateTime Since { get; set; }

        public virtual DateTime? Until { get; set; }

        [Required]
        public virtual bool Rehab { get; set; }

        public PersonLawPenalty()
        {
            Rehab = false;
        }

        public PersonLawPenalty(long personId, int causeId, int lawPenaltyDefinitionId, DateTime notification, bool startsOnNotification = true, TimeSpan? rehab = null)
            : this()
        {
            PersonId = personId;
            CauseId = causeId;
            LawPenaltyDefinitionId = lawPenaltyDefinitionId;
            Notification = notification;
            Since = startsOnNotification ? notification : notification + TimeSpan.FromDays(1);
            if(rehab != null)
                Until = Since + rehab;
        }
    }
}
