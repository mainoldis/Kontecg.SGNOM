using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Primitives;
using Kontecg.Timing;

namespace Kontecg.HumanResources
{
    [Table("persons", Schema = "docs")]
    public class Person : FullAuditedAggregateRoot<long>, IExtendableObject, ISoftDelete
    {
        /// <summary>
        ///     Maximum length of the <see cref="Name" /> property.
        /// </summary>
        public const int MaxNameLength = 64;

        /// <summary>
        ///     Maximum length of the <see cref="Surname" /> property.
        /// </summary>
        public const int MaxSurnameLength = 64;

        /// <summary>
        ///     Maximum length of the <see cref="IdentityCard" /> property.
        /// </summary>
        public const int MaxIdentityCardLength = 11;

        /// <summary>
        ///     "^[0-9]{11,}$".
        /// </summary>
        public const string IdentityCardRegex = "^[0-9]{11,}$";

        /// <summary>
        ///     Maximum length of the <see cref="PhotoFileType" /> property.
        /// </summary>
        public const int MaxPhotoMimeTypeLength = 64;

        public const int MaxScholarshipLevelLength = 150;

        public const int MaxScholarshipLength = 150;

        /// <summary>
        ///     Initialize an instance of <see cref="Person" /> class.
        /// </summary>
        public Person()
        {
            OrganizationUnits = [];
        }

        /// <summary>
        ///     Initialize an instance of <see cref="Person" /> class.
        /// </summary>
        public Person(string name, string surname, string lastname, string identityCard, Gender gender, DateTime birthDate)
            : this()
        {
            Name = name;
            Surname = surname;
            Lastname = lastname;
            IdentityCard = identityCard;
            Gender = gender;
            BirthDate = birthDate;
            OrganizationUnits = new List<PersonOrganizationUnit>();

            SetNormalizedNames();
        }

        /// <summary>
        ///     Name of the person.
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        ///     Surname of the person.
        /// </summary>
        [Required]
        [StringLength(MaxSurnameLength)]
        public virtual string Surname { get; set; }

        /// <summary>
        ///     Surname of the person.
        /// </summary>
        [Required]
        [StringLength(MaxSurnameLength)]
        public virtual string Lastname { get; set; }

        /// <summary>
        ///     Return full name
        /// </summary>
        [NotMapped]
        public virtual string FullName => Name + " " + Surname + " " + Lastname;

        [Required]
        [StringLength(MaxIdentityCardLength)]
        public virtual string IdentityCard { get; set; }

        [Required]
        public virtual Gender Gender { get; set; }

        [Required]
        public virtual DateTime BirthDate { get; set; }

        public virtual Guid? PhotoId { get; set; }

        [MaxLength(MaxPhotoMimeTypeLength)]
        public virtual string PhotoFileType { get; set; }

        [NotMapped]
        public virtual byte[] Photo { get; set; }

        public virtual Etnia Etnia { get; set; }

        public virtual Address OfficialAddress { get; set; }

        public virtual Address Address { get; set; }

        [StringLength(MaxScholarshipLevelLength)]
        public virtual string ScholarshipLevel { get; set; }

        [StringLength(MaxScholarshipLength)]
        public virtual string Scholarship { get; set; }

        public virtual ClothingSize ClothingSize { get; set; }

        public virtual string ExtensionData { get; set; }

        public virtual List<PersonOrganizationUnit> OrganizationUnits { get; set; }

        public virtual void SetNormalizedNames()
        {
            Name = Name?.ToUpperInvariant();
            Surname = Surname?.ToUpperInvariant();
            Lastname = Lastname?.ToUpperInvariant();
        }

        [NotMapped]
        public virtual int Age =>
            BirthDate > Clock.Now
                ? 0
                : Clock.Now.Year - BirthDate.Year + (BirthDate.Month > Clock.Now.Month ||
                                                     (BirthDate.Month == Clock.Now.Month &&
                                                      BirthDate.Day > Clock.Now.Day)
                    ? -1
                    : 0);

        /// <summary>
        ///     Check if a person has Photo
        /// </summary>
        /// <returns></returns>
        public bool HasPhoto()
        {
            return PhotoId != null && PhotoFileType != null;
        }

        /// <summary>
        ///     Reset photo fields
        /// </summary>
        public void ClearPhoto()
        {
            PhotoId = null;
            PhotoFileType = null;
        }
        public override string ToString()
        {
            return $"{FullName} (ID: {Id}, IdentityCard: {IdentityCard}, Gender: {Gender}, Age: {Age})";
        }
    }
}
