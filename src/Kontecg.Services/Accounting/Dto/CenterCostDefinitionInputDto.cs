using System.ComponentModel.DataAnnotations;

namespace Kontecg.Accounting.Dto
{
    public class CenterCostDefinitionInputDto
    {
        [Required]
        public int Account { get; set; }

        [Required]
        public int Code { get; set; }

        [Required]
        public string Description { get; set; }

        public string Reference { get; set; }
    }
}