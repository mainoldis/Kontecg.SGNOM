using Kontecg.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontecg.Currencies
{
    [Table("bank_offices", Schema = "gen")]
    public class BankOffice : AuditedEntity
    {
        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int DescriptionLength = 150;

        /// <summary>
        ///     Max length of the <see cref="Name" /> property.
        /// </summary>
        public const int MaxNameLength = 20;

        /// <summary>
        ///     Display name of the bank.
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        public virtual int BankId { get; set; }

        [Required]
        [ForeignKey("BankId")]
        public virtual Bank Bank { get; set; }

        /// <summary>
        ///     Description of the bank.
        /// </summary>
        [Required]
        [StringLength(DescriptionLength)]
        public virtual string Description { get; set; }

        public BankOffice(int bankId, string name, string description)
        {
            BankId = bankId;
            Name = name;
            Description = description;
            SetNormalizedNames();
        }

        public virtual void SetNormalizedNames()
        {
            Name = Name?.ToUpperInvariant();
            Description = Description?.ToUpperInvariant();
        }
    }
}