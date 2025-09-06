using Kontecg.Application.Services.Dto;
using Kontecg.Currencies.Dtos;

namespace Kontecg.Accounting.Dto
{
    public class AccountingVoucherNoteDto : EntityDto
    {
        public int ScopeId { get; set; }

        public AccountOperation Operation { get; set; }

        public int Account { get; set; }

        public int SubAccount { get; set; }

        public int SubControl { get; set; }

        public int Analysis { get; set; }

        public MoneyDto Amount { get; set; }
    }
}
