using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontecg.WorkRelations.Dto
{
    public class UpdateWorkPlaceUnitInputDto
    {
        [Required]
        public List<long> DocumentIds { get; set; }

        [Required]
        public long OrganizationUnitId { get; set; }
    }
}