using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("person_family_relationships", Schema = "docs")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PersonFamilyRelationship : AuditedEntity
    {
        [Required]
        public virtual long PersonId { get; set; }

        [NotMapped]
        public virtual Person Person { get; set; }

        [Required]
        public virtual long PersonOnRelationId { get; set; }

        [NotMapped]
        public virtual Person PersonOnRelation { get; set; }

        [Required]
        public virtual RelationshipKind Kind { get; set; }

        [Required]
        public virtual bool Cohabits { get; set; }

        public PersonFamilyRelationship(long personId, long personOnRelationId, RelationshipKind kind, bool cohabits = true)
        {
            PersonId = personId;
            PersonOnRelationId = personOnRelationId;
            Kind = kind;
            Cohabits = cohabits;
        }
    }
}
