using Kontecg.Application.Services.Dto;

namespace Kontecg.Accounting.Dto
{
    public class CenterCostDefinitionDto : EntityDto
    {
        public int Code { get; set; }

        public string Description { get; set; }

        public string Reference { get; set; }

        public bool IsActive { get; set; }

        public AccountDefinitionDto AccountDefinition { get; set; }
    }
}