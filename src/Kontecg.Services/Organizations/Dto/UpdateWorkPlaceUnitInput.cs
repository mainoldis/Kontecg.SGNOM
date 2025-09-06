using System;
using System.ComponentModel.DataAnnotations;

namespace Kontecg.Organizations.Dto
{
    public class UpdateWorkPlaceUnitInput : UpdateOrganizationUnitInput
    {
        [Required]
        [Range(1, Int32.MaxValue)]
        public int ClassificationId { get; set; }

        public long? WorkPlacePaymentId { get; set; }

        [Required]
        [StringLength(WorkPlaceUnit.MaxAcronymLength)]
        public string Acronym { get; set; }

        public int MaxMembersApproved { get; set; }
    }
}