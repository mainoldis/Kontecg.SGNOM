using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Dependency;
using Kontecg.Organizations.Dto;

namespace Kontecg.Organizations
{
    public interface ITemplateDocumentAppService : IApplicationService, ITransientDependency
    {
        TemplateDocumentOutputDto GetTemplateDocument(FindTemplateDocumentInputDto input);

        Task<TemplateDocumentOutputDto> GetTemplateDocumentAsync(FindTemplateDocumentInputDto input);

        ListResultDto<TemplateListDto> GetTemplate(FindTemplateInputDto input);

        Task<ListResultDto<TemplateListDto>> GetTemplateAsync(FindTemplateInputDto input);

        TemplateDocumentOutputDto GetJobPositionDocument(FindTemplateDocumentInputDto input);

        Task<TemplateDocumentOutputDto> GetJobPositionDocumentAsync(FindTemplateDocumentInputDto input);

        ListResultDto<JobPositionListDto> GetJobPositions(FindTemplateInputDto input);

        Task<ListResultDto<JobPositionListDto>> GetJobPositionsAsync(FindTemplateInputDto input);
    }
}
