using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.Organizations
{
    [Table("workplace_classifications", Schema = "est")]
    public class WorkPlaceClassification : Entity
    {
        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int DescriptionLength = 150;

        [StringLength(DescriptionLength)]
        public virtual string Description { get; set; }

        public virtual int Level { get; set; }

        public WorkPlaceClassification()
        {
        }

        public WorkPlaceClassification(string description, int level)
        {
            Description = description;
            Level = level;

            SetNormalizedDescription();
        }

        public virtual void SetNormalizedDescription()
        {
            Description = Description?.ToUpperInvariant();
        }
    }
}
