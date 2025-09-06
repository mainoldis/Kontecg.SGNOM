using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.Common;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.EntityHistory;
using Kontecg.Extensions;
using Kontecg.Linq.Extensions;
using Kontecg.MultiCompany.Dto;
using Kontecg.Organizations.Dto;
using Kontecg.Runtime.Session;
using Kontecg.Workflows;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Organizations
{
    [KontecgAuthorize(PermissionNames.HumanResources)]
    [UseCase(Description = "Servicio de Gestión de Estructura y Plantillas Empresariales")]
    public class TemplateDocumentAppService : SGNOMAppServiceBase, ITemplateDocumentAppService
    {
        private readonly ICommonLookupAppService _commonLookupAppService;
        private readonly IRepository<TemplateDocument> _templateDocumentRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITemplateJobPositionRepository _templateJobPositionRepository;
        private readonly IRepository<WorkPlaceUnit, long> _workPlaceUnitRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public TemplateDocumentAppService(
            ICommonLookupAppService commonLookupAppService,
            IRepository<TemplateDocument> templateDocumentRepository,
            ITemplateRepository templateRepository,
            ITemplateJobPositionRepository templateJobPositionRepository,
            IRepository<WorkPlaceUnit, long> workPlaceUnitRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _commonLookupAppService = commonLookupAppService;
            _templateDocumentRepository = templateDocumentRepository;
            _templateJobPositionRepository = templateJobPositionRepository;
            _workPlaceUnitRepository = workPlaceUnitRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _templateRepository = templateRepository;
        }

        public TemplateDocumentOutputDto GetTemplateDocument(FindTemplateDocumentInputDto input)
        {
            var document = _templateDocumentRepository.GetAll().FirstOrDefault(t => t.Review == ReviewStatus.Confirmed);

            if (document != null)
            {
                TemplateDocumentOutputDto output = ObjectMapper.Map<TemplateDocumentOutputDto>(document);
                output.Company = ObjectMapper.Map<CompanyInfoDto>(_commonLookupAppService.GetCompanyInfo());
                output.Template = GetTemplate(new FindTemplateInputDto {DocumentId = document.Id, OrganizationUnitCode = input.OrganizationUnitCode});
                return output;
            }

            return new TemplateDocumentOutputDto();
        }

        public async Task<TemplateDocumentOutputDto> GetTemplateDocumentAsync(FindTemplateDocumentInputDto input)
        {
            var document = await _templateDocumentRepository.GetAll().FirstOrDefaultAsync(t => t.Review == ReviewStatus.Confirmed);

            if (document != null)
            {
                TemplateDocumentOutputDto output = ObjectMapper.Map<TemplateDocumentOutputDto>(document);
                output.Company = ObjectMapper.Map<CompanyInfoDto>(await _commonLookupAppService.GetCompanyInfoAsync());
                output.Template = await GetTemplateAsync(new FindTemplateInputDto { DocumentId = document.Id, OrganizationUnitCode = input.OrganizationUnitCode });
                return output;
            }

            return await Task.FromResult(new TemplateDocumentOutputDto());
        }

        public ListResultDto<TemplateListDto> GetTemplate(FindTemplateInputDto input)
        {
            var workPlaces = _workPlaceUnitRepository
                .GetAllIncluding(w => w.Classification, w => w.WorkPlacePayment, w => w.Parent)
                .WhereIf(!input.OrganizationUnitCode.IsNullOrWhiteSpace(), w => w.Code.StartsWith(input.OrganizationUnitCode))
                .Where(w => w.Classification.Level == 3)
                .ToList();

            var templates = _templateRepository.GetAllIncluding(
                t => t.Occupation,
                t => t.Occupation.Group,
                t => t.Occupation.Category,
                t => t.Occupation.Responsibility,
                t => t.ScholarshipLevel)
                .Where(t => t.DocumentId == input.DocumentId).ToList();

            var join = templates.Join(
                workPlaces, template => template.OrganizationUnitId, unit => unit.Id, (t, w) =>
                {
                    t.OrganizationUnit = w;
                    return t;
                })
                .OrderBy(t => t.OrganizationUnitCode)
                .ThenByDescending(t => t.Occupation.Group, ComplexityGroupComparer.Instance).ToList();

            var templateCount = join.Count();

            return new ListResultDto<TemplateListDto>()
            {
                Items = join.Select(t =>
                {
                    var templateDto = ObjectMapper.Map<TemplateListDto>(t);
                    return templateDto;
                }).ToList()
            };
        }

        public async Task<ListResultDto<TemplateListDto>> GetTemplateAsync(FindTemplateInputDto input)
        {
            var workPlaces = await _workPlaceUnitRepository
                .GetAllIncluding(w => w.Classification, w => w.WorkPlacePayment, w => w.Parent)
                .WhereIf(!input.OrganizationUnitCode.IsNullOrWhiteSpace(), w => w.Code.StartsWith(input.OrganizationUnitCode))
                .Where(w => w.Classification.Level == 3)
                .ToListAsync();

            var templates = await _templateRepository.GetAllIncluding(
                    t => t.Occupation,
                    t => t.Occupation.Group,
                    t => t.Occupation.Category,
                    t => t.Occupation.Responsibility,
                    t => t.ScholarshipLevel)
                .Where(t => t.DocumentId == input.DocumentId).ToListAsync();

            var join = templates.Join(
                    workPlaces, template => template.OrganizationUnitId, unit => unit.Id, (t, w) =>
                    {
                        t.OrganizationUnit = w;
                        return t;
                    })
                .OrderBy(t => t.OrganizationUnit, WorkPlaceComparer.Instance)
                .ThenByDescending(t => t.Occupation.Group, ComplexityGroupComparer.Instance).ToList();

            var templateCount = join.Count();

            return await Task.FromResult(new ListResultDto<TemplateListDto>()
            {
                Items = join.Select(t =>
                {
                    var templateDto = ObjectMapper.Map<TemplateListDto>(t);
                    return templateDto;
                }).ToList()
            });
        }

        public TemplateDocumentOutputDto GetJobPositionDocument(FindTemplateDocumentInputDto input)
        {
            var document = _templateDocumentRepository.GetAll().FirstOrDefault(t => t.Review == ReviewStatus.Confirmed);

            if (document != null)
            {
                TemplateDocumentOutputDto output = ObjectMapper.Map<TemplateDocumentOutputDto>(document);
                output.Company = ObjectMapper.Map<CompanyInfoDto>(_commonLookupAppService.GetCompanyInfo());
                output.JobPositions = GetJobPositions(new FindTemplateInputDto { DocumentId = document.Id, OrganizationUnitCode = input.OrganizationUnitCode, Contract = input.ContractString});
                return output;
            }

            return new TemplateDocumentOutputDto();
        }

        public async Task<TemplateDocumentOutputDto> GetJobPositionDocumentAsync(FindTemplateDocumentInputDto input)
        {
            var document = await _templateDocumentRepository.GetAll().FirstOrDefaultAsync(t => t.Review == ReviewStatus.Confirmed);
            if (document != null)
            {
                TemplateDocumentOutputDto output = ObjectMapper.Map<TemplateDocumentOutputDto>(document);
                output.Company = ObjectMapper.Map<CompanyInfoDto>(await _commonLookupAppService.GetCompanyInfoAsync());
                output.JobPositions = await GetJobPositionsAsync(new FindTemplateInputDto { DocumentId = document.Id, OrganizationUnitCode = input.OrganizationUnitCode, Contract = input.ContractString });
                return output;
            }

            return await Task.FromResult(new TemplateDocumentOutputDto());
        }

        public ListResultDto<JobPositionListDto> GetJobPositions(FindTemplateInputDto input)
        {
            var result = new ListResultDto<JobPositionListDto>();
            var persons = PersonManager.Persons.ToList();

            using var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using (_currentUnitOfWorkProvider.Current.SetCompanyId(KontecgSession.GetCompanyId()))
            {
                var workPlaces = _workPlaceUnitRepository
                    .GetAllIncluding(w => w.Classification,
                        w => w.WorkPlacePayment,
                        w => w.Parent)
                    .WhereIf(!input.OrganizationUnitCode.IsNullOrWhiteSpace(), w => w.Code.StartsWith(input.OrganizationUnitCode))
                    .Where(w => w.Classification.Level == 3)
                    .ToList();

                var templates = _templateJobPositionRepository.GetJobPositions(input.DocumentId, null,
                    workPlaces.Select(i => i.Id).ToArray());

                var templateWithPersons = templates.GroupJoin(
                        persons,
                        relation => relation.Document?.PersonId,
                        person => person.Id,
                        (relation, persons) => new
                        {
                            JobPosition = relation,
                            Person = persons
                        })
                    .SelectMany(r => r.Person.DefaultIfEmpty(), (r, p) =>
                    {
                        if (r.JobPosition.DocumentId.HasValue) r.JobPosition.Document.Person = p;
                        return r.JobPosition;
                    });

                var join = templateWithPersons.Join(workPlaces, template => template.OrganizationUnitId, unit => unit.Id,
                        (t, w) =>
                        {
                            t.OrganizationUnit = w;
                            return t;
                        })
                    .OrderBy(t => t.OrganizationUnit, WorkPlaceComparer.Instance)
                    .ThenByDescending(t => t.Occupation.Group, ComplexityGroupComparer.Instance).ToList();

                result.Items = join.Select(t =>
                {
                    var positionDto = ObjectMapper.Map<JobPositionListDto>(t);
                    return positionDto;
                }).ToList();
            }

            uow.Complete();
            return result;
        }

        public async Task<ListResultDto<JobPositionListDto>> GetJobPositionsAsync(FindTemplateInputDto input)
        {
            var result = new ListResultDto<JobPositionListDto>();
            var persons = await PersonManager.Persons.ToListAsync();

            using var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using (_currentUnitOfWorkProvider.Current.SetCompanyId(KontecgSession.GetCompanyId()))
            {
                var workPlaces = await _workPlaceUnitRepository
                    .GetAllIncluding(w => w.Classification,
                        w => w.WorkPlacePayment,
                        w => w.Parent)
                    .WhereIf(!input.OrganizationUnitCode.IsNullOrWhiteSpace(), w => w.Code.StartsWith(input.OrganizationUnitCode))
                    .Where(w => w.Classification.Level == 3)
                    .ToListAsync();

                var templates = await _templateJobPositionRepository.GetJobPositionsAsync(input.DocumentId, null,
                    workPlaces.Select(i => i.Id).ToArray());

                var templateWithPersons = templates.GroupJoin(
                    persons,
                    relation => relation.Document?.PersonId,
                    person => person.Id,
                    (relation, persons) => new
                    {
                        JobPosition = relation,
                        Person = persons
                    })
                    .SelectMany(r => r.Person.DefaultIfEmpty(), (r, p) =>
                    {
                        if (r.JobPosition.DocumentId.HasValue) r.JobPosition.Document.Person = p;
                        return r.JobPosition;
                    });

                var join = templateWithPersons.Join(workPlaces, template => template.OrganizationUnitId, unit => unit.Id,
                        (t, w) =>
                        {
                            t.OrganizationUnit = w;
                            return t;
                        })
                    .OrderBy(t => t.OrganizationUnit, WorkPlaceComparer.Instance)
                    .ThenByDescending(t => t.Occupation.Group, ComplexityGroupComparer.Instance).ToList();

                result.Items = join.Select(t =>
                {
                    var positionDto = ObjectMapper.Map<JobPositionListDto>(t);
                    return positionDto;
                }).ToList();
            }

            await uow.CompleteAsync();
            return result;
        }
    }
}
