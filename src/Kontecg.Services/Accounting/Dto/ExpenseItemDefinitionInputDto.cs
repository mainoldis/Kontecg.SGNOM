using System.ComponentModel.DataAnnotations;

namespace Kontecg.Accounting.Dto
{
    public class ExpenseItemDefinitionInputDto
    {
        [Required]
        public int Code { get; set; }

        [Required]
        public string Description { get; set; }

        public string Reference { get; set; }

        public int? CenterCost { get; set; }
    }
}