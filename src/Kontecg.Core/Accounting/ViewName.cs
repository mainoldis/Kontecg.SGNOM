using Kontecg.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Kontecg.MultiCompany;
using Kontecg.Domain;
using System.Collections.Generic;
using Kontecg.Workflows;

namespace Kontecg.Accounting
{
    [Table("view_names", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public class ViewName : CreationAuditedEntity, IMustHaveReferenceGroup
    {
        /// <summary>
        ///     Max length of the <see cref="ReferenceGroup" /> property.
        /// </summary>
        public const int MaxReferenceLength = 15;

        public const int MaxDescriptionLength = 250;

        public const int MaxNameLength = 1024;

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [Required]
        [StringLength(MaxReferenceLength)]
        public virtual string ReferenceGroup { get; set; }

        public virtual List<DocumentDefinition> Documents { get; set; }

        /// <inheritdoc />
        public ViewName()
        {
            Documents = new List<DocumentDefinition>();
        }

        /// <inheritdoc />
        public ViewName(string name, string description, string referenceGroup)
            :this()
        {
            Name = name;
            Description = description;
            ReferenceGroup = referenceGroup;
        }
    }
}