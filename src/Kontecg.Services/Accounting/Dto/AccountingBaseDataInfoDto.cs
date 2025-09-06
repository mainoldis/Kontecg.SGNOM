using Kontecg.Application.Services.Dto;

namespace Kontecg.Accounting.Dto
{
    public class AccountingBaseDataInfoDto
    {
        public ListResultDto<AccountDefinitionDto> Accounts { get; set; }

        public ListResultDto<CenterCostDefinitionDto> CenterCosts { get; set; }

        public ListResultDto<ExpenseItemDefinitionDto> ExpenseItems { get; set; }

        public ListResultDto<AccountingFunctionDefinitionDto> Functions { get; set; }

        public ListResultDto<AccountingClassifierDefinitionDto> Classifiers { get; set; }

        public ListResultDto<DocumentDefinitionDto> Documents { get; set; }
    }
}