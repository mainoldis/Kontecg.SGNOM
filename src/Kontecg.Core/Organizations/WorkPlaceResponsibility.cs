using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.Organizations
{
    [Table("workplace_responsibilities", Schema = "est")]
    public class WorkPlaceResponsibility : Entity
    {
        public const int MaxCodeMaxLength = 8;

        public int ClassificationId { get; set; }

        [Required]
        [ForeignKey("ClassificationId")]
        public WorkPlaceClassification Classification { get; set; }

        [Required]
        [StringLength(MaxCodeMaxLength)]
        public virtual string OccupationCode { get; set; }

        /// <inheritdoc />
        public WorkPlaceResponsibility()
        {
        }

        /// <inheritdoc />
        public WorkPlaceResponsibility(WorkPlaceClassification classification, string occupationCode)
        {
            Classification = classification;
            OccupationCode = occupationCode;
        }

        /// <inheritdoc />
        public WorkPlaceResponsibility(int classificationId, string occupationCode)
        {
            ClassificationId = classificationId;
            OccupationCode = occupationCode;
        }
    }
}