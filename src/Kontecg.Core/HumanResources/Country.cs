using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.HumanResources
{
    [Table("countries", Schema = "gen")]
    public class Country : Entity<long>
    {
        /// <summary>
        ///     Max length of the <see cref="RegionName" /> property.
        /// </summary>
        public const int MaxRegionNameLength = 100;

        /// <summary>
        ///     Max length of the <see cref="Name" /> property.
        /// </summary>
        public const int MaxCountryNameLength = 150;

        /// <summary>
        ///     Max length of the <see cref="Name" /> property.
        /// </summary>
        public const int MaxAcronymNameLength = 3;

        /// <summary>
        ///     Region name.
        /// </summary>
        [Required]
        [StringLength(MaxRegionNameLength)]
        public virtual string RegionName { get; set; }

        /// <summary>
        ///     Country name.
        /// </summary>
        [Required]
        [StringLength(MaxCountryNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        ///     Acronym name.
        /// </summary>
        [StringLength(MaxAcronymNameLength)]
        public virtual string Acronym { get; set; }

        /// <summary>
        ///     International code for countries.
        /// </summary>
        [Required]
        [StringLength(MaxAcronymNameLength)]
        public virtual string InternationalCode { get; set; }

        [ForeignKey("CountryId")]
        public virtual ICollection<State> States { get; set; }

        public Country(string regionName, string name, string acronym, string internationalCode)
        {
            RegionName = regionName;
            Name = name;
            Acronym = acronym;
            InternationalCode = internationalCode;

            SetNormalizedNames();
        }

        public virtual void SetNormalizedNames()
        {
            Name = Name?.ToUpperInvariant();
            RegionName = RegionName?.ToUpperInvariant();
            Acronym = Acronym?.ToUpperInvariant();
        }
    }
}
