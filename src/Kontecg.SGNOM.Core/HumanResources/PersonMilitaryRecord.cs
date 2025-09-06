using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.HumanResources
{
    [Table("person_military_records", Schema = "docs")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class PersonMilitaryRecord : AuditedEntity
    {
        [Required]
        public virtual long PersonId { get; set; }

        [NotMapped]
        public virtual Person Person { get; set; }

        public virtual int LocationId { get; set; }

        [Required]
        [ForeignKey("LocationId")]
        public virtual MilitaryLocationDefinition Location { get; set; }

        public virtual DateTime? LastUpdated { get; set; }

        [Required]
        public virtual bool SuitableForMilitaryService { get; set; }

        [Required]
        public virtual bool CompletedActiveMilitaryService { get; set; }

    }
}
