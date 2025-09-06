using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Domain.Repositories;
using Kontecg.Json;
using Kontecg.Organizations;
using Kontecg.Organizations.Dto;
using Kontecg.Salary;
using Kontecg.Timing;
using Kontecg.Views.Organizations;
using Kontecg.Workflows;
using Kontecg.WorkRelations;
using Xunit;

namespace Kontecg.SGNOM.Tests
{
    public class Template_Tests : SGNOMModuleTestBase
    {
        private readonly ITemplateDocumentAppService _templateDocumentAppService;
        private readonly IRepository<TemplateDocument> _templateDocumentRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITemplateJobPositionRepository _templateJobPositionRepository;
        private readonly IOccupationRepository _occupationRepository;
        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly IRepository<WorkPlaceUnit, long> _workPlaceUnitRepository;
        private readonly IEmploymentRepository _employmentRepository;

        public Template_Tests()
        {
            var admin = GetDefaultCompanyAdmin();
            KontecgSession.UserId = admin.Id;
            KontecgSession.CompanyId = admin.CompanyId;
            _templateDocumentAppService = LocalIocManager.Resolve<ITemplateDocumentAppService>();
            _templateDocumentRepository = LocalIocManager.Resolve<IRepository<TemplateDocument>>();
            _templateRepository = LocalIocManager.Resolve<ITemplateRepository>();
            _templateJobPositionRepository = LocalIocManager.Resolve<ITemplateJobPositionRepository>();
            _occupationRepository = LocalIocManager.Resolve<IOccupationRepository>();
            _workShiftRepository = LocalIocManager.Resolve<IWorkShiftRepository>();
            _workPlaceUnitRepository = LocalIocManager.Resolve<IRepository<WorkPlaceUnit, long>>();
            _employmentRepository = LocalIocManager.Resolve<IEmploymentRepository>();
        }

        [Fact]
        public async Task Insert_document_Test()
        {
            await WithUnitOfWorkAsync(KontecgSession.CompanyId, () =>
            {
                var relationship = _employmentRepository.CurrentRelationship().Where(e => e.Type != EmploymentType.B);
                var directorGeneral =
                    relationship.FirstOrDefault(r => r.OccupationCode == "62250039" || r.OccupationCode == "724001");
                var directorCapitalHumano =
                    relationship.FirstOrDefault(r => r.OccupationCode == "62230041" || r.OccupationCode == "723003");

                var document = _templateDocumentRepository.GetAll().FirstOrDefault();
                if (document != null)
                {
                    document.MadeOn = Clock.Now;
                    document.Review = ReviewStatus.Confirmed;
                    return _templateDocumentRepository.UpdateAsync(document);
                }

                return _templateDocumentRepository.InsertAsync(new TemplateDocument()
                {
                    MadeOn = Clock.Now,
                    Review = ReviewStatus.Confirmed
                });
            });
        }

        [Theory]
        [InlineData("723008", "01080101", 400339, "N")]
        [InlineData("415007", "01080101", 400339, "N")]
        [InlineData("415001", "01080101", 400339, "N")]
        [InlineData("415015", "01080101", 400339, "N")]
        [InlineData("411003", "01080101", 400339, "N")]
        [InlineData("411004", "01080101", 400339, "N")]
        [InlineData("604001", "01080101", 400339, "N")]

        [InlineData("419010", "01080102", 400349, "N")]
        [InlineData("416016", "01080102", 400349, "N", 8)]
        [InlineData("416003", "01080102", 400349, "N", 2)]
        [InlineData("411006", "01080102", 400349, "N")]
        [InlineData("604001", "01080102", 400349, "N")]

        public void Create_fake_job_position_Test(string occupationCode, string ouCode, int centerCost, string workShit, int approved = 1)
        {
            WithUnitOfWork(KontecgSession.CompanyId, () =>
            {
                var document = _templateDocumentRepository.GetAll().FirstOrDefault();
                var occupation = _occupationRepository.GetByCode(occupationCode);
                var workPlace = _workPlaceUnitRepository.GetAllIncluding(w => w.Classification)
                    .FirstOrDefault(w => w.Code == ouCode && w.Classification.Level >= 3);
                var workShift = _workShiftRepository.GetWorkShiftByName(workShit);

                if (document != null && occupation != null && workPlace != null && workShift != null)
                {
                    string[] pattern = {workShit};

                    var fakeTemplate = new Template
                    {
                        DocumentId = document.Id,
                        OrganizationUnitId = workPlace.Id,
                        OrganizationUnitCode = workPlace.Code,
                        CenterCost = centerCost,
                        Occupation = occupation,
                        EmployeeSalaryForm = EmployeeSalaryForm.Royal,
                        WorkShift = pattern.ToJsonString(),
                        Proposals = approved,
                        Approved = approved
                    };

                    _templateRepository.Insert(fakeTemplate);
                }
            });
        }

        [Fact]
        public void Update_fake_job_position_Test()
        {
            WithUnitOfWork(KontecgSession.CompanyId, () =>
            {
                var document = _templateDocumentRepository.GetAll().FirstOrDefault();
                var exactOccupationSpecification = new ExactOccupationSpecification(_occupationRepository.GetByCode("416003"));

                var template = _templateRepository.GetAllIncluding(t => t.Occupation).ToList().FirstOrDefault(t =>
                    t.DocumentId == document.Id && t.CenterCost == 400349 &&
                    exactOccupationSpecification.IsSatisfiedBy(t.Occupation));
                if (template != null)
                {
                    template.Approved = 2;
                    _templateRepository.Update(template);
                }
            });
        }

        [Fact]
        public void Refresh_all_code_position_Test()
        {
            WithUnitOfWork(KontecgSession.CompanyId, () =>
            {
                _templateJobPositionRepository.UpdateCodes();
            });
        }

        [Fact]
        public void Get_template_with_employments_Test()
        {
            WithUnitOfWork(KontecgSession.CompanyId, () =>
            {
                var templateWithEmployments = _templateJobPositionRepository.GetJobPositions(1, null);
            });
        }

        [Fact]
        public async Task Export_template_document_Test()
        {
            TemplateDocumentOutputDto templateDocumentOutputDto = new ();

            await WithUnitOfWorkAsync(KontecgSession.CompanyId, async () =>
            {
                templateDocumentOutputDto = await _templateDocumentAppService.GetTemplateDocumentAsync(new FindTemplateDocumentInputDto());
            });

            LegalTemplateDocument paper = new();
            paper.DataSource = new List<TemplateDocumentOutputDto> () {templateDocumentOutputDto};
            await paper.ExportToPdfAsync("Plantilla_de_cargos_" + (templateDocumentOutputDto.Company?.CompanyName ?? "") + ".pdf");

            SummaryByWorkPlaceAndCategoryDocument summaryByWorkPlaceAndCategoryPaper = new(templateDocumentOutputDto);
            await summaryByWorkPlaceAndCategoryPaper.ExportToPdfAsync("Resumen_de_plantilla_por_area_y_categoria_ocupacional_" + (templateDocumentOutputDto.Company?.CompanyName ?? "") + ".pdf");

            SummaryByGroupAndCategoryDocument summaryByGroupAndCategoryPaper = new(templateDocumentOutputDto);
            await summaryByGroupAndCategoryPaper.ExportToPdfAsync("Resumen_de_plantilla_por_grupo_salarial_y_categoria_ocupacional_" + (templateDocumentOutputDto.Company?.CompanyName ?? "") + ".pdf");
        }

        [Fact]
        public async Task Export_template_document_with_jobs_Test()
        {
            TemplateDocumentOutputDto templateDocumentOutputDto = new();

            await WithUnitOfWorkAsync(KontecgSession.CompanyId, async () =>
            {
                templateDocumentOutputDto = await _templateDocumentAppService.GetJobPositionDocumentAsync(new FindTemplateDocumentInputDto());
            });

            LegalJobPositionDocument paper = new();
            paper.DataSource = new List<TemplateDocumentOutputDto>() { templateDocumentOutputDto };
            await paper.ExportToPdfAsync("Registro_de_datos_principales_" + (templateDocumentOutputDto.Company?.CompanyName ?? "") + ".pdf");
        }
    }
}
