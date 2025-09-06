using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.HumanResources;
using Kontecg.MultiCompany;

namespace Kontecg.Identity
{
    [Table("sign_on_documents", Schema = "docs")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class SignOnDocument : AuditedEntity, IMustHaveCompany
    {
        public const int MaxFullNameLength = Person.MaxNameLength + Person.MaxSurnameLength * 2;
        public const int MaxOccupationLength = 250;
        /// <summary>
        ///     Maximum length of the <see cref="IdentityCard" /> property.
        /// </summary>
        public const int MaxIdentityCardLength = 11;
        public const int MaxCodeMaxLength = 8;
        /// <summary>
        ///     "^[0-9]{11,}$".
        /// </summary>
        public const string IdentityCardRegex = "^[0-9]{11,}$";

        [Required]
        public virtual int Order { get; set; }

        public virtual int DocumentDefinitionId { get; set; }

        [StringLength(MaxFullNameLength)]
        public virtual string FullName { get; set; }

        [Required]
        [StringLength(MaxCodeMaxLength)]
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

        /// <inheritdoc />
        public SignOnDocument()
        {
        }

        /// <inheritdoc />
        public SignOnDocument(string code, string occupation, string identityCard, string fullName, int order)
        {
            Order = order;
            FullName = fullName;
            Code = code;
            Occupation = occupation;
            IdentityCard = identityCard;
        }
    }
}
