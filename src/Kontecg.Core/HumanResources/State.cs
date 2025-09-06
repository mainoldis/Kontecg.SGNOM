using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.HumanResources
{
    [Table("states", Schema = "gen")]
    public class State : Entity<long>
    {
        /// <summary>
        ///     Max length of the <see cref="Name" /> property.
        /// </summary>
        public const int MaxStateNameLength = 60;

        /// <summary>
        ///     Max length of the <see cref="Code" /> property.
        /// </summary>
        public const int MaxCodeLength = 4;

        /// <summary>
        ///     Max length of the <see cref="RegionName" /> property.
        /// </summary>
        public const int MaxRegionNameLength = 50;

        /// <summary>
        ///     State name.
        /// </summary>
        [Required]
        [StringLength(MaxStateNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        ///     Code for states.
        /// </summary>
        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        /// <summary>
        ///     Region.
        /// </summary>
        [StringLength(MaxRegionNameLength)]
        public virtual string RegionName { get; set; }

        /// <summary>
        ///     Country.
        /// </summary>
        [Required]
        public virtual Country Country { get; set; }

        [ForeignKey("StateId")]
        public virtual ICollection<City> Cities { get; set; }

        public State(string name, string code, string regionName)
        {
            Name = name;
            Code = code;
            RegionName = regionName;

            SetNormalizedNames();
        }

        public virtual void SetNormalizedNames()
        {
            RegionName = RegionName?.ToUpperInvariant();
        }
    }
}
