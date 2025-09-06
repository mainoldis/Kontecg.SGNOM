using System.ComponentModel.DataAnnotations;
using Kontecg.Dto;

namespace Kontecg.Common.Dto
{
    public class FindCitiesInput : PagedInputDto
    {
        [Required]
        public string State { get; set; }
    }
}
