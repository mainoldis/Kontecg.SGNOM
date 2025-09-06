using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.HumanResources;

namespace Kontecg.Identity
{
    [Table("signs", Schema = "docs")]
    public class Sign : AuditedEntity, IMustHaveCompany
    {
        public const int MaxFullNameLength = Person.MaxNameLength + Person.MaxSurnameLength * 2;

        public const int MaxOccupationLength = 250;

        /// <summary>
        ///     Maximum length of the <see cref="IdentityCard" /> property.
        /// </summary>
        public const int MaxIdentityCardLength = 11;

        public const int MaxCodeLength = 8;

        /// <summary>
        ///     "^[0-9]{11,}$".
        /// </summary>
        public const string IdentityCardRegex = "^[0-9]{11,}$";

        [StringLength(MaxFullNameLength)]
        public virtual string FullName { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        [Required]
        [StringLength(MaxOccupationLength)]
        public virtual string Occupation { get; set; }

        [StringLength(MaxIdentityCardLength)]
        public virtual string IdentityCard { get; set; }

        [Required]
        public virtual bool Owner { get; set; }

        /// <inheritdoc />
        public virtual int CompanyId { get; set; }

        public Sign()
        {
        }

        public Sign(string fullName, string occupation, string identityCard, bool owner)
        {
            FullName = fullName;
            Occupation = occupation;
            IdentityCard = identityCard;
            Owner = owner;
        }
    }
}
