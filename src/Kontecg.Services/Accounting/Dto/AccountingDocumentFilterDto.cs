using System.ComponentModel.DataAnnotations;

namespace Kontecg.Accounting.Dto
{
    public class AccountingDocumentFilterDto
    {
        [Required]
        public int DocumentId { get; set; }
    }
}
