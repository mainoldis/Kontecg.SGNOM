using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.HumanResources
{
    [Table("cities", Schema = "gen")]
    public class City : Entity<long>
    {
        /// <summary>
        ///     Max length of the <see cref="Name" /> property.
        /// </summary>
        public const int MaxCityNameLength = 60;

        /// <summary>
        ///     Max length of the <see cref="Code" /> property.
        /// </summary>
        public const int MaxCodeLength = 4;

        /// <summary>
        ///     State name.
        /// </summary>
        [Required]
        [StringLength(MaxCityNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        ///     Code for states.
        /// </summary>
        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        /// <summary>
        ///     State.
        /// </summary>
        [Required]
        public virtual State State { get; set; }

        public City(string name, string code)
        {
            Name = name;
            Code = code;

            SetNormalizedNames();
        }

        public virtual void SetNormalizedNames()
        {
            Name = Name?.ToUpperInvariant();
        }
    }
}
