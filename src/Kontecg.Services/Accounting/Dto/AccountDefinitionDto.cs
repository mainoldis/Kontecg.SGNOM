using Kontecg.Application.Services.Dto;

namespace Kontecg.Accounting.Dto
{
    public class AccountDefinitionDto : EntityDto
    {
        public int Account { get; set; }

        public int SubAccount { get; set; }

        public int SubControl { get; set; }

        public int Analysis { get; set; }

        public string Description { get; set; }

        public string Reference { get; set; }

        public string Kind { get; set; }

        public bool IsActive { get; set; }
    }
}