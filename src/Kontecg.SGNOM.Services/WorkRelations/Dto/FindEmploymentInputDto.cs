using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kontecg.Workflows;

namespace Kontecg.WorkRelations.Dto
{
    public class FindEmploymentInputDto
    {
        [Required]
        public List<long> EmploymentIds { get; set; } = new();

        [Required]
        public ReviewStatus Source { get; set; } = ReviewStatus.ForReview;

        [Required]
        public ReviewStatus Target { get; set; }
    }
}