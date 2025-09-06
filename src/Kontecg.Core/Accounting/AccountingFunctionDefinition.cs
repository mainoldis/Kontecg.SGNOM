using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.Accounting
{
    [Table("accounting_function_definitions", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public class AccountingFunctionDefinition : AuditedEntity, IPassivable
    {
        /// <summary>
        ///     Max length of the <see cref="Name" /> property.
        /// </summary>
        public const int MaxNameLength = 50;

        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int MaxDescriptionLength = 150;

        /// <summary>
        ///     Max length of the <see cref="Reference" /> property.
        /// </summary>
        public const int MaxReferenceLength = 50;

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [Required]
        [StringLength(MaxReferenceLength)]
        public virtual string Reference { get; set; }

        public virtual int StorageId { get; set; }

        [ForeignKey("StorageId")]
        public virtual AccountingFunctionDefinitionStorage Storage { get; set; }

        public virtual bool IsActive { get; set; }

        public AccountingFunctionDefinition()
        {
            IsActive = true;
        }

        public AccountingFunctionDefinition(string name, string description, string reference)
            :this()
        {
            Name = name;
            Description = description;
            Reference = reference;

            SetNormalizedNames();
        }

        public virtual void SetNormalizedNames()
        {
            Name = Name?.ToUpperInvariant();
            Description = Description?.ToUpperInvariant();
            Reference = Reference?.ToUpperInvariant();
        }
    }
}
