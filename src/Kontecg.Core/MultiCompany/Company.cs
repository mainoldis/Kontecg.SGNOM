using System;
using System.ComponentModel.DataAnnotations;
using Kontecg.Authorization.Users;
using Kontecg.HumanResources;

namespace Kontecg.MultiCompany
{
    /// <summary>
    ///     Represents a Company in the system.
    ///     A company is a isolated customer for the application
    ///     which has it's own users, roles and other application entities.
    /// </summary>
    public class Company : KontecgCompany<User>
    {
        public const string DefaultCompanyReup = "104.0.02754";

        /// <summary>
        ///     Maximum length of the <see cref="LogoFileType"/> and <see cref="LetterHeadFileType"/> properties.
        /// </summary>
        public const int MaxMimeTypeLength = 64;

        /// <summary>
        ///     Maximum length of the <see cref="Reup" /> property.
        /// </summary>
        public const int MaxReupLength = 11;

        /// <summary>
        ///     Maximum length of the <see cref="Organism" /> property.
        /// </summary>
        public const int MaxOrganismLength = 250;

        [StringLength(MaxReupLength)]
        public virtual string Reup { get; set; }

        [Required]
        public virtual Address Address { get; set; }

        [StringLength(MaxOrganismLength)]
        public virtual string Organism { get; set; }

        public virtual Guid? LogoId { get; set; }

        [MaxLength(MaxMimeTypeLength)]
        public virtual string LogoFileType { get; set; }

        public virtual Guid? LetterHeadId { get; set; }

        [MaxLength(MaxMimeTypeLength)]
        public virtual string LetterHeadFileType { get; set; }

        public Company(string companyName, string name)
            : base(companyName, name)
        {
        }

        public Company(string companyName, string name, string reup, Address address, string organism)
            : base(companyName, name)
        {
            Reup = reup;
            Address = address;
            Organism = organism;
        }

        protected Company()
        {
        }

        public virtual bool HasLogo()
        {
            return LogoId != null && LogoFileType != null;
        }

        public void ClearLogo()
        {
            LogoId = null;
            LogoFileType = null;
        }

        public virtual bool HasLetterHead()
        {
            return LetterHeadId != null && LetterHeadFileType != null;
        }

        public void ClearLetterHead()
        {
            LetterHeadId = null;
            LetterHeadFileType = null;
        }
    }
}
