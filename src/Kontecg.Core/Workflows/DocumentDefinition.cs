using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Accounting;
using Kontecg.Domain;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.Workflows
{
    [Table("document_definitions", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public class DocumentDefinition : FullAuditedEntity, IExtendableObject, IPassivable, IMustHaveReferenceGroup
    {
        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int MaxDescriptionLength = 150;

        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int MaxLegalLength = 150;

        /// <summary>
        ///     Maximum length of <see cref="LegalTypeAssemblyQualifiedName" /> property.
        ///     Value: 512.
        /// </summary>
        public const int MaxLegalTypeAssemblyQualifiedNameLength = 512;

        /// <summary>
        ///     Max length of the <see cref="Reference" /> property.
        /// </summary>
        public const int MaxReferenceLength = 15;

        [Required]
        public virtual int Code { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [Required]
        [StringLength(MaxReferenceLength)]
        public virtual string Reference { get; set; }

        [Required]
        [StringLength(MaxReferenceLength)]
        public virtual string ReferenceGroup { get; set; }

        [StringLength(MaxLegalLength)]
        public virtual string Legal { get; set; }

        /// <summary>
        ///     AssemblyQualifiedName of the entity type.
        /// </summary>
        [StringLength(MaxLegalTypeAssemblyQualifiedNameLength)]
        public virtual string LegalTypeAssemblyQualifiedName { get; set; }

        /// <summary>
        /// The logical ID of the workflow. This ID is the same across versions. 
        /// </summary>
        public virtual string WorkflowDefinitionDefinitionId { get; set; } = default!;

        public virtual string ExtensionData { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual List<ViewName> Views { get; set; }

        public virtual List<DocumentSignOnDefinition> SignOnDefinitions { get; set; }

        public DocumentDefinition()
        {
            IsActive = true;
            Views = new List<ViewName>();
            SignOnDefinitions = new List<DocumentSignOnDefinition>();
        }

        public DocumentDefinition(int code, string description, string reference, string referenceGroup)
            : this()
        {
            Code = code;
            Description = description;
            Reference = reference;
            ReferenceGroup = referenceGroup;
            SetNormalizedNames();
        }

        public virtual void SetNormalizedNames()
        {
            Description = Description?.ToUpperInvariant();
            Reference = Reference?.ToUpperInvariant();
            ReferenceGroup = ReferenceGroup?.ToUpperInvariant();
        }

        public override string ToString()
        {
            return $"{Code} - {Description}";
        }
    }
}
