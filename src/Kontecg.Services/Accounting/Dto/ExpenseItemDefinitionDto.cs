using Kontecg.Application.Services.Dto;

namespace Kontecg.Accounting.Dto
{
    public class ExpenseItemDefinitionDto : EntityDto
    {
        public int Code { get; set; }

        public string Description { get; set; }

        public string Reference { get; set; }

        public CenterCostDefinitionDto CenterCostDefinition { get; set; }

        public bool IsActive { get; set; }
    }
}