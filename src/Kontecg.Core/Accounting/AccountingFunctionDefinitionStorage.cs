using Kontecg.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Kontecg.MultiCompany;

namespace Kontecg.Accounting
{
    [Table("accounting_function_definition_storage", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public class AccountingFunctionDefinitionStorage : AuditedEntity
    {
        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int MaxDescriptionLength = 150;

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [Required]
        public virtual string Script { get; set; }

        /// <inheritdoc />
        public AccountingFunctionDefinitionStorage()
        {
        }

        /// <inheritdoc />
        public AccountingFunctionDefinitionStorage(string description, string script)
            : this()
        {
            Description = description;
            Script = script;

            SetNormalizedNames();
        }

        public virtual void SetNormalizedNames()
        {
            Description = Description?.ToUpperInvariant();
        }
    }
}