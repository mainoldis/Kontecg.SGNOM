using System.Linq;
using Kontecg.Authorization;
using Kontecg.EntityHistory;
using Kontecg.WorkRelations.Dto;
using System.Threading.Tasks;
using Kontecg.Application.Services.Dto;
using Kontecg.Domain.Repositories;
using Kontecg.MultiCompany;
using Kontecg.Organizations;
using Kontecg.UI;
using System;
using System.Collections.Generic;
using Kontecg.Application.Features;
using Kontecg.BlobStoring;
using Kontecg.Storage;
using Kontecg.WorkRelations.Exporting;
using Kontecg.Dto;
using Kontecg.Features;
using Kontecg.HumanResources;
using Kontecg.HumanResources.Dto;
using Kontecg.Linq.Extensions;
using Kontecg.Storage.Blobs;
using Kontecg.Threading;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.WorkRelations
{
    [KontecgAuthorize(SGNOMPermissions.WorkRelations)]
    [UseCase(Description = "Servicio de Gestión de Relaciones Laborales")]
    public class WorkRelationsAppService : SGNOMAppServiceBase, IWorkRelationsAppService
    {
        private readonly IWorkRelationshipProvider _workRelationshipProvider;
        private readonly IWorkRelationshipExcelExporter _workRelationshipExporter;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<WorkPlaceUnit, long> _workPlaceUnitRepository;
        private readonly IBlobContainer<HumanResourcesContainer> _blogContainer;

        public WorkRelationsAppService(
            IWorkRelationshipProvider workRelationshipProvider,
            IWorkRelationshipExcelExporter workRelationshipExporter,
            IEmploymentRepository employmentRepository,
            IPersonRepository personRepository,
            IRepository<Company> companyRepository,
            IRepository<WorkPlaceUnit, long> workPlaceUnitRepository,
            ITempFileCacheManager tempFileCacheManager, 
            IBlobContainer<HumanResourcesContainer> blogContainer)
        {
            _workRelationshipProvider = workRelationshipProvider;
            _workRelationshipExporter = workRelationshipExporter;
            _employmentRepository = employmentRepository;
            _personRepository = personRepository;
            _companyRepository = companyRepository;
            _workPlaceUnitRepository = workPlaceUnitRepository;
            _tempFileCacheManager = tempFileCacheManager;
            _blogContainer = blogContainer;
        }
        
        [RequiresFeature(CoreFeatureNames.ExportingFeature, CoreFeatureNames.ExportingExcelFeature, RequiresAll = true)]
        [KontecgAuthorize(SGNOMPermissions.EmploymentsExport)]
        public void ExportCurrentWorkRelationship()
        {
            var relationshipInformation = _workRelationshipProvider.GetWorkRelationshipInformation();
            FileDto exportToFile = _workRelationshipExporter.ExportToFile(relationshipInformation.Select(ObjectMapper.Map<WorkRelationshipDto>).ToList());

            var buffer = _tempFileCacheManager.GetFile(exportToFile.FileToken);
            AsyncHelper.RunSync(() => _blogContainer.SaveAsync($"{HumanResourcesContainer.Documents}/{exportToFile.FileName}", buffer, true));
        }

        [RequiresFeature(CoreFeatureNames.ExportingFeature, CoreFeatureNames.ExportingExcelFeature, RequiresAll = true)]
        [KontecgAuthorize(SGNOMPermissions.EmploymentsExport)]
        public async Task ExportCurrentWorkRelationshipAsync()
        {
            var relationshipInformation = await _workRelationshipProvider.GetWorkRelationshipInformationAsync();
            FileDto exportToFile = await _workRelationshipExporter.ExportToFileAsync(relationshipInformation.Select(ObjectMapper.Map<WorkRelationshipDto>).ToList());

            var buffer = _tempFileCacheManager.GetFile(exportToFile.FileToken);
            await _blogContainer.SaveAsync($"{HumanResourcesContainer.Documents}/{exportToFile.FileName}", buffer, true);
        }

        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public PagedResultDto<EmploymentDocumentInfoDto> GetEmploymentDocuments(FilterRequest input)
        {
            var relationshipInformation = _employmentRepository.CurrentRelationship(input);

            var query = (from employment in relationshipInformation
                         join wp in _workPlaceUnitRepository.GetAllIncluding(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                         join person in PersonManager.Persons on employment.PersonId equals person.Id
                         join company in _companyRepository.GetAll() on employment.CompanyId equals company.Id
                         select new
                         {
                             employment,
                             wp,
                             person,
                             company
                         }).ToList();

            var totalCount = query.Count;
            var items = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public async Task<PagedResultDto<EmploymentDocumentInfoDto>> GetEmploymentDocumentsAsync(FilterRequest input)
        {
            var relationshipInformation = await _employmentRepository.CurrentRelationshipAsync(input);

            var query = (from employment in relationshipInformation
                join wp in await _workPlaceUnitRepository.GetAllIncludingAsync(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                join person in PersonManager.Persons on employment.PersonId equals person.Id
                join company in await _companyRepository.GetAllAsync() on employment.CompanyId equals company.Id
                select new
                {
                    employment,
                    wp,
                    person,
                    company
                }).ToList();

            var totalCount = query.Count;
            var items = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public PagedResultDto<EmploymentDocumentInfoDto> GetTimelineForEmploymentDocuments(FilterRequest input)
        {
            var timelineInformation = _employmentRepository.RelationshipByPeriod(input);

            var query = (from employment in timelineInformation
                         join wp in _workPlaceUnitRepository.GetAllIncluding(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                         join person in PersonManager.Persons on employment.PersonId equals person.Id
                         join company in _companyRepository.GetAll() on employment.CompanyId equals company.Id
                         select new
                         {
                             employment,
                             wp,
                             person,
                             company
                         }).ToList();

            var totalCount = query.Count;
            var items = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public async Task<PagedResultDto<EmploymentDocumentInfoDto>> GetTimelineForEmploymentDocumentsAsync(FilterRequest input)
        {
            var timelineInformation = await _employmentRepository.RelationshipByPeriodAsync(input);

            var query = (from employment in timelineInformation 
                join wp in await _workPlaceUnitRepository.GetAllIncludingAsync(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                join person in PersonManager.Persons on employment.PersonId equals person.Id
                join company in await _companyRepository.GetAllAsync() on employment.CompanyId equals company.Id
                select new
                {
                    employment,
                    wp,
                    person,
                    company
                }).ToList();

            var totalCount = query.Count;
            var items = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public LegalEmploymentDocumentDto GetEmploymentDocumentByExp(FilterRequest input)
        {
            var lastByExp = _employmentRepository.LastByExp(input.Exp.Value);

            if (lastByExp.Count > 1)
                throw new UserFriendlyException(L("MoreThenOneExpNumber"));

            if (lastByExp.Count == 0)
                return new LegalEmploymentDocumentDto();

            var document = lastByExp[0];
            document.Person = _personRepository.FirstOrDefault(document.PersonId);

            if (document.Person == null)
                throw new UserFriendlyException(L("PersonNotAssociatedWithExp"));

            document.Company = _companyRepository.FirstOrDefault(document.CompanyId);
            var wp = _workPlaceUnitRepository.FirstOrDefault(document.OrganizationUnitId);
            if (wp != null)
                _workPlaceUnitRepository.EnsurePropertyLoaded(wp, p => p.WorkPlacePayment);

            document.WorkPlaceUnit = wp;

            var documentDto = ObjectMapper.Map<LegalEmploymentDocumentDto>(document);

            switch (document.Type)
            {
                case EmploymentType.A:
                    documentDto.After = ObjectMapper.Map<InnerPartEmploymentDocumentDto>(document);
                    documentDto.After.EmployeeSalaryForm = L(document.EmployeeSalaryForm.ToString());
                    break;
                case EmploymentType.R:

                    if (document.Previous != null)
                    {
                        documentDto.Before = ObjectMapper.Map<InnerPartEmploymentDocumentDto>(document.Previous);
                        documentDto.Before.EmployeeSalaryForm = L(document.Previous?.EmployeeSalaryForm.ToString());
                    }

                    documentDto.After = ObjectMapper.Map<InnerPartEmploymentDocumentDto>(document);
                    documentDto.After.EmployeeSalaryForm = L(document.EmployeeSalaryForm.ToString());
                    break;
                case EmploymentType.B:
                    documentDto.Before = ObjectMapper.Map<InnerPartEmploymentDocumentDto>(document);
                    documentDto.Before.EmployeeSalaryForm = L(document.EmployeeSalaryForm.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return documentDto;
        }

        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public async Task<LegalEmploymentDocumentDto> GetEmploymentDocumentByExpAsync(FilterRequest input)
        {
            var lastByExp = await _employmentRepository.LastByExpAsync(input.Exp.Value);

            if (lastByExp.Count > 1)
                throw new UserFriendlyException(L("MoreThenOneExpNumber"));

            if (lastByExp.Count == 0)
                return new LegalEmploymentDocumentDto();

            var document = lastByExp[0];
            document.Person = await _personRepository.FirstOrDefaultAsync(document.PersonId);

            if (document.Person == null)
                throw new UserFriendlyException(L("PersonNotAssociatedWithExp"));

            document.Company = await _companyRepository.FirstOrDefaultAsync(document.CompanyId);
            var wp = await _workPlaceUnitRepository.FirstOrDefaultAsync(document.OrganizationUnitId);
            if (wp != null)
                await _workPlaceUnitRepository.EnsurePropertyLoadedAsync(wp, p => p.WorkPlacePayment);

            document.WorkPlaceUnit = wp;

            var documentDto = ObjectMapper.Map<LegalEmploymentDocumentDto>(document);

            switch (document.Type)
            {
                case EmploymentType.A:
                    documentDto.After = ObjectMapper.Map<InnerPartEmploymentDocumentDto>(document);
                    documentDto.After.EmployeeSalaryForm = L(document.EmployeeSalaryForm.ToString());
                    break;
                case EmploymentType.R:

                    if (document.Previous != null)
                    {
                        documentDto.Before = ObjectMapper.Map<InnerPartEmploymentDocumentDto>(document.Previous);
                        documentDto.Before.EmployeeSalaryForm = L(document.Previous?.EmployeeSalaryForm.ToString());
                    }

                    documentDto.After = ObjectMapper.Map<InnerPartEmploymentDocumentDto>(document);
                    documentDto.After.EmployeeSalaryForm = L(document.EmployeeSalaryForm.ToString());
                    break;
                case EmploymentType.B:
                    documentDto.Before = ObjectMapper.Map<InnerPartEmploymentDocumentDto>(document);
                    documentDto.Before.EmployeeSalaryForm = L(document.EmployeeSalaryForm.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return documentDto;
        }

        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public PagedResultDto<EmploymentDocumentInfoDto> GetUnemployed(FilterRequest input)
        {
            //TODO: Extender este método para incluir periodo de búsqueda
            var currentUnemployed = _employmentRepository.CurrentRelationship(input).Where(e => e.Type == EmploymentType.B).ToList();

            var query = (from employment in currentUnemployed
                join wp in _workPlaceUnitRepository.GetAllIncluding(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                join person in PersonManager.Persons on employment.PersonId equals person.Id
                join company in _companyRepository.GetAll() on employment.CompanyId equals company.Id
                select new
                {
                    employment,
                    wp,
                    person,
                    company
                }).ToList();

            var totalCount = query.Count;
            var items = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentCode = r.wp.WorkPlacePayment?.Code;
                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public async Task<PagedResultDto<EmploymentDocumentInfoDto>> GetUnemployedAsync(FilterRequest input)
        {
            //TODO: Extender este método para incluir periodo de búsqueda
            var currentUnemployed = (await _employmentRepository.CurrentRelationshipAsync(input)).Where(e => e.Type == EmploymentType.B).ToList();

            var query = (from employment in currentUnemployed
                        join wp in await _workPlaceUnitRepository.GetAllIncludingAsync(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                join person in PersonManager.Persons on employment.PersonId equals person.Id
                join company in await _companyRepository.GetAllAsync() on employment.CompanyId equals company.Id
                select new
                {
                    employment,
                    wp,
                    person,
                    company
                }).ToList();

            var totalCount = query.Count;
            var items = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentCode = r.wp.WorkPlacePayment?.Code;
                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public PagedResultDto<EmploymentDocumentInfoDto> GetExpiredDocuments()
        {
            var relationship = _employmentRepository.GetDocumentToExpireSoon();

            var query = (from employment in relationship
                        join wp in _workPlaceUnitRepository.GetAllIncluding(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                join person in PersonManager.Persons on employment.PersonId equals person.Id
                join company in _companyRepository.GetAll() on employment.CompanyId equals company.Id
                select new
                {
                    employment,
                    wp,
                    person,
                    company
                }).ToList();

            var totalCount = query.Count;
            var items = query;

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentCode = r.wp.WorkPlacePayment?.Code;
                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public async Task<PagedResultDto<EmploymentDocumentInfoDto>> GetExpiredDocumentsAsync()
        {
            var relationship = await _employmentRepository.GetDocumentToExpireSoonAsync();

            var query = (from employment in relationship
                join wp in await _workPlaceUnitRepository.GetAllIncludingAsync(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                join person in PersonManager.Persons on employment.PersonId equals person.Id
                join company in await _companyRepository.GetAllAsync() on employment.CompanyId equals company.Id
                select new
                {
                    employment,
                    wp,
                    person,
                    company
                }).ToList();

            var totalCount = query.Count;
            var items = query;

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentCode = r.wp.WorkPlacePayment?.Code;
                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        /// <inheritdoc />
        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public PagedResultDto<EmploymentDocumentInfoDto> GetDocumentsToReview()
        {
            var relationship = _employmentRepository.GetDocumentsToReview();

            var query = (from employment in relationship
                        join wp in _workPlaceUnitRepository.GetAllIncluding(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                        join person in PersonManager.Persons on employment.PersonId equals person.Id
                        join company in _companyRepository.GetAll() on employment.CompanyId equals company.Id
                        select new
                        {
                            employment,
                            wp,
                            person,
                            company
                        }).ToList();

            var totalCount = query.Count;
            var items = query;

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentCode = r.wp.WorkPlacePayment?.Code;
                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        /// <inheritdoc />
        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public async Task<PagedResultDto<EmploymentDocumentInfoDto>> GetDocumentsToReviewAsync()
        {
            var relationship = await _employmentRepository.GetDocumentsToReviewAsync();

            var query = (from employment in relationship
                        join wp in await _workPlaceUnitRepository.GetAllIncludingAsync(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                        join person in PersonManager.Persons on employment.PersonId equals person.Id
                        join company in await _companyRepository.GetAllAsync() on employment.CompanyId equals company.Id
                        select new
                        {
                            employment,
                            wp,
                            person,
                            company
                        }).ToList();

            var totalCount = query.Count;
            var items = query;

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentCode = r.wp.WorkPlacePayment?.Code;
                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        /// <inheritdoc />
        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public PagedResultDto<EmploymentDocumentInfoDto> GetDocumentsReviewed()
        {
            var relationship = _employmentRepository.GetDocumentsReviewed();

            var query = (from employment in relationship
                        join wp in _workPlaceUnitRepository.GetAllIncluding(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                        join person in PersonManager.Persons on employment.PersonId equals person.Id
                        join company in _companyRepository.GetAll() on employment.CompanyId equals company.Id
                        select new
                        {
                            employment,
                            wp,
                            person,
                            company
                        }).ToList();

            var totalCount = query.Count;
            var items = query;

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentCode = r.wp.WorkPlacePayment?.Code;
                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        /// <inheritdoc />
        [KontecgAuthorize(SGNOMPermissions.EmploymentsList)]
        public async Task<PagedResultDto<EmploymentDocumentInfoDto>> GetDocumentsReviewedAsync()
        {
            var relationship = await _employmentRepository.GetDocumentsReviewedAsync();

            var query = (from employment in relationship
                        join wp in await _workPlaceUnitRepository.GetAllIncludingAsync(w => w.WorkPlacePayment) on employment.OrganizationUnitId equals wp.Id
                        join person in PersonManager.Persons on employment.PersonId equals person.Id
                        join company in await _companyRepository.GetAllAsync() on employment.CompanyId equals company.Id
                        select new
                        {
                            employment,
                            wp,
                            person,
                            company
                        }).ToList();

            var totalCount = query.Count;
            var items = query.OrderBy(e => e.employment.Exp).ToList();

            return new PagedResultDto<EmploymentDocumentInfoDto>(
                totalCount,
                items.Select(r =>
                {
                    var document = ObjectMapper.Map<EmploymentDocumentInfoDto>(r.employment);
                    document.Person = ObjectMapper.Map<PersonDto>(r.person);
                    document.CompanyName = r.company.CompanyName;
                    document.Organism = r.company.Organism;

                    document.WorkPlacePaymentCode = r.wp.WorkPlacePayment?.Code;
                    document.WorkPlacePaymentDescription = r.wp.WorkPlacePayment?.Description;

                    document.EmployeeSalaryForm = L(r.employment.EmployeeSalaryForm.ToString());

                    return document;
                }).ToList());
        }

        /// <inheritdoc />
        public bool OnBoard(EmploymentDocumentInputDto input)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<bool> OnBoardAsync(EmploymentDocumentInputDto input)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool Deactivate(EmploymentDocumentInputDto input)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<bool> DeactivateAsync(EmploymentDocumentInputDto input)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool ChangePosition(EmploymentDocumentInputDto input)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<bool> ChangePositionAsync(EmploymentDocumentInputDto input)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool ReviewOrConfirm(FindEmploymentInputDto input)
        {
            if(input == null)
                throw new ArgumentNullException(nameof(input));

            var relationship = _employmentRepository
                               .GetAll()
                               .WhereIf(input.EmploymentIds is {Count: > 0}, e => input.EmploymentIds.Contains(e.Id))
                               .Where(e => e.Review == input.Source)
                               .Select(e => e.Id)
                               .ToList();

            _employmentRepository.UpdateReviewStatus(input.Target, relationship.ToArray());
            CurrentUnitOfWork.SaveChanges();
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> ReviewOrConfirmAsync(FindEmploymentInputDto input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var relationship = await (await _employmentRepository.GetAllAsync())
                                     .WhereIf(input.EmploymentIds is {Count: > 0}, e => input.EmploymentIds.Contains(e.Id))
                                     .Where(e => e.Review == input.Source)
                                     .Select(e => e.Id)
                                     .ToListAsync();

            await _employmentRepository.UpdateReviewStatusAsync(input.Target, relationship.ToArray());
            await CurrentUnitOfWork.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public void UpdateDocumentOrganizationUnit(UpdateWorkPlaceUnitInputDto input)
        {
            Check.NotNull(input, nameof(input));
            _employmentRepository.UpdateEmploymentsWorkPlace(input.OrganizationUnitId, input.DocumentIds.ToArray());
            CurrentUnitOfWork.SaveChanges();
        }

        public async Task UpdateDocumentOrganizationUnitAsync(UpdateWorkPlaceUnitInputDto input)
        {
            Check.NotNull(input, nameof(input));
            await _employmentRepository.UpdateEmploymentsWorkPlaceAsync(input.OrganizationUnitId, input.DocumentIds.ToArray());
            await CurrentUnitOfWork.SaveChangesAsync();
        }
    }
}
