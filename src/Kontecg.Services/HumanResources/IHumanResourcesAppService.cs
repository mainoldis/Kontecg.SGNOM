using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Common.Dto;
using Kontecg.HumanResources.Dto;

namespace Kontecg.HumanResources
{
    public interface IHumanResourcesAppService : ICrudAppService<PersonDto, long, FindPersonsInput, PersonDto, PersonDto, EntityDto<long>, EntityDto<long>>
    {
        PagedResultDto<PersonDto> GetAgedPeople(bool nextYear = false);
    }
}
