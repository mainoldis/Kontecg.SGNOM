using System.ComponentModel.DataAnnotations;

namespace Kontecg.Accounting.Dto
{
    public class AccountingVoucherFilterDto
    {
        [Required]
        public int DocumentId { get; set; }

        public int? ScopeId { get; set; }
    }
}
