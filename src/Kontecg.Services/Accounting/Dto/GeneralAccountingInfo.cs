using Kontecg.Application.Services.Dto;
using Kontecg.MultiCompany.Dto;
using Kontecg.Timing.Dto;

namespace Kontecg.Accounting.Dto
{
    public class GeneralAccountingInfo
    {
        public CompanyInfoDto Company { get; set; }

        public AccountingBaseDataInfoDto AccountingInfo { get; set; }

        public PagedResultDto<PersonalAccountingInfoDto> PersonalAccountingInfo { get; set; }

        public ListResultDto<PeriodInfoDto> PeriodInfoDto { get; set; }
    }
}
