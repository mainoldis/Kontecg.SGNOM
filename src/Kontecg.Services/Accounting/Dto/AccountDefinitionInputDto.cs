using System.ComponentModel.DataAnnotations;

namespace Kontecg.Accounting.Dto
{
    public class AccountDefinitionInputDto
    {
        [Required]
        public int Account { get; set; }

        [Required]
        public int SubAccount { get; set; }

        [Required]
        public int SubControl { get; set; }

        [Required]
        public int Analysis { get; set; }

        [Required]
        public string Description { get; set; }

        public string Reference { get; set; }

        [Required]
        public string Kind { get; set; }
    }
}