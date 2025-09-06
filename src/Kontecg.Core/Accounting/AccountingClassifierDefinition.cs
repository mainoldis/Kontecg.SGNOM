using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontecg.Accounting
{
    [Table("accounting_classifier_definitions", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public class AccountingClassifierDefinition : Entity
    {
        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int MaxDescriptionLength = 150;

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }
        
        /// <inheritdoc />
        public AccountingClassifierDefinition(string description)
        {
            Description = description;
        }
    }
}