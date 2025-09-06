using System.ComponentModel.DataAnnotations;

namespace Kontecg.Organizations.Dto
{
    public class CreateWorkPlaceUnitInput : CreateOrganizationUnitInput
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ClassificationId { get; set; }

        public long? WorkPlacePaymentId { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxMembersApproved { get; set; }
    }
}
