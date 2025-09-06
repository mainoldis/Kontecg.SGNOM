using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Dependency;
using Kontecg.WorkRelations.Dto;
using System.Threading.Tasks;

namespace Kontecg.WorkRelations
{
    public interface IWorkRelationsAppService : IApplicationService, ITransientDependency
    {
        #region Lists

        void ExportCurrentWorkRelationship();

        Task ExportCurrentWorkRelationshipAsync();

        PagedResultDto<EmploymentDocumentInfoDto> GetEmploymentDocuments(FilterRequest input);

        Task<PagedResultDto<EmploymentDocumentInfoDto>> GetEmploymentDocumentsAsync(FilterRequest input);

        PagedResultDto<EmploymentDocumentInfoDto> GetTimelineForEmploymentDocuments(FilterRequest input);

        Task<PagedResultDto<EmploymentDocumentInfoDto>> GetTimelineForEmploymentDocumentsAsync(FilterRequest input);

        LegalEmploymentDocumentDto GetEmploymentDocumentByExp(FilterRequest input);

        Task<LegalEmploymentDocumentDto> GetEmploymentDocumentByExpAsync(FilterRequest input);

        PagedResultDto<EmploymentDocumentInfoDto> GetUnemployed(FilterRequest input);

        Task<PagedResultDto<EmploymentDocumentInfoDto>> GetUnemployedAsync(FilterRequest input);

        PagedResultDto<EmploymentDocumentInfoDto> GetExpiredDocuments();

        Task<PagedResultDto<EmploymentDocumentInfoDto>> GetExpiredDocumentsAsync();

        PagedResultDto<EmploymentDocumentInfoDto> GetDocumentsToReview();

        Task<PagedResultDto<EmploymentDocumentInfoDto>> GetDocumentsToReviewAsync();

        PagedResultDto<EmploymentDocumentInfoDto> GetDocumentsReviewed();

        Task<PagedResultDto<EmploymentDocumentInfoDto>> GetDocumentsReviewedAsync();

        #endregion

        bool OnBoard(EmploymentDocumentInputDto input);

        Task<bool> OnBoardAsync(EmploymentDocumentInputDto input);

        bool Deactivate(EmploymentDocumentInputDto input);

        Task<bool> DeactivateAsync(EmploymentDocumentInputDto input);

        bool ChangePosition(EmploymentDocumentInputDto input);

        Task<bool> ChangePositionAsync(EmploymentDocumentInputDto input);

        bool ReviewOrConfirm(FindEmploymentInputDto input);

        Task<bool> ReviewOrConfirmAsync(FindEmploymentInputDto input);

        void UpdateDocumentOrganizationUnit(UpdateWorkPlaceUnitInputDto input);

        Task UpdateDocumentOrganizationUnitAsync(UpdateWorkPlaceUnitInputDto input);
    }
}
