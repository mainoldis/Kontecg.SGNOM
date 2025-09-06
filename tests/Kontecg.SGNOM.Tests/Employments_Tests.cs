using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Itenso.TimePeriod;
using Kontecg.BlobStoring;
using Kontecg.Configuration;
using Kontecg.Data;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Dto;
using Kontecg.Extensions;
using Kontecg.HumanResources;
using Kontecg.Json;
using Kontecg.Organizations;
using Kontecg.Salary;
using Kontecg.Storage;
using Kontecg.Storage.Blobs;
using Kontecg.Threading;
using Kontecg.Timing;
using Kontecg.Timing.Dto;
using Kontecg.Views.WorkRelations;
using Kontecg.Workflows;
using Kontecg.WorkRelations;
using Kontecg.WorkRelations.Dto;
using Kontecg.WorkRelations.Exporting;
using Shouldly;
using Xunit;

namespace Kontecg.SGNOM.Tests
{
    public class Employments_Tests : SGNOMModuleTestBase
    {
        public Employments_Tests()
        {
            var admin = GetDefaultCompanyAdmin();
            KontecgSession.UserId = admin.Id;
            KontecgSession.CompanyId = admin.CompanyId;
        }

        [Fact]
        public void Generate_new_employment_code_Test()
        {
            var generator = Resolve<ICodeGenerator>();

            var newCode = generator.CreateEmploymentCode(Clock.Now, EmploymentType.A);
            newCode.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Generate_new_document_code_Test()
        {
            var generator = Resolve<ICodeGenerator>();

            var newCode = generator.CreateTimeDistributionDocumentCode(Clock.Now);
            newCode.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Generate_new_employment_group_id_Test()
        {
            var generator = Resolve<ICodeGenerator>();

            var newCode = generator.CreateOrUpdateEmploymentGroupId();
            newCode.ToString().ShouldNotBeEmpty();
        }

        [Theory]
        [InlineData(ContractType.I, ContractSubType.I, "74121038045", 21000, "2024-10-01", "01080103", 400349,"N", 12, "419010", null, false, false)]
        [InlineData(ContractType.I, ContractSubType.I, "82121726486", 22631, "2024-10-01", "01080103", 400349, "N", 12, "416016", null, false, false)]
        [InlineData(ContractType.I, ContractSubType.I, "83092519800", 21695, "2024-10-01", "01080103", 400349, "N", 12, "416016", null, false, false)]
        [InlineData(ContractType.I, ContractSubType.I, "84101023141", 21683, "2024-10-01", "01080103", 400349, "N", 12, "416016", null, false, false)]
        public void Generate_next_Test(ContractType contractType, ContractSubType contractSubType, string identityCard, int exp, DateTime since, string ouCode, int cc, string shift, int objectiveId, string occupationCode, string expiration, bool byAssignment, bool byOfficial)
        {
            WithUnitOfWork(() =>
            {
                var personManager = Resolve<IPersonManager>();
                var workPlaceManager = Resolve<WorkPlaceUnitManager>();
                var generator = Resolve<ICodeGenerator>();
                var unitOfWorkManager = Resolve<IUnitOfWorkManager>();
                var employmentRepository = Resolve<IEmploymentRepository>();
                var summaryRepository = Resolve<IRepository<EmploymentSummary>>();
                var occupationRepository = Resolve<IOccupationRepository>();
                var workShitRepository = Resolve<IWorkShiftRepository>();

                var person = personManager.FindPersonByIdentityCard(identityCard);
                if (person != null)
                {
                    var contract = new EmploymentDocument();
                    var lastByPersonId = employmentRepository.LastByPersonId(person.Id);
                    contract.Previous = lastByPersonId is {Count: > 0} ? lastByPersonId[0] : null;
                    contract.Summary = summaryRepository.Get(objectiveId);

                    contract.MadeOn = since;
                    contract.Contract = contractType;
                    contract.ContractSubType = contractType == ContractType.D ? contractSubType : null;
                    
                    contract.Type = contract.Summary.Type;
                    contract.SubType =
                        contract.Previous is {Contract: ContractType.D} && contract.Type == EmploymentType.R
                            ? EmploymentSubType.DI
                            : (objectiveId == 18 ? EmploymentSubType.MP : EmploymentSubType.I);

                    var workShift = workShitRepository.GetWorkShiftByName(shift);
                    var pattern = new WorkYear(since, workShift?.ToWorkPattern() ?? WorkPattern.Default);
                    if (pattern.WorkingPeriods.HasIntersectionPeriods(since))
                    {
                        var periods = pattern.WorkingPeriods.IntersectionPeriods(since);
                        periods.SortByEnd(ListSortDirection.Descending);
                        since = periods.End;
                    }

                    if (contract.Previous is {Type: EmploymentType.B} &&
                        TimeCompare.IsSameDay(contract.Previous.EffectiveSince, since))
                        since = contract.Previous.EffectiveSince;

                    if (contract.Type != EmploymentType.B)
                    {
                        contract.WorkShift = workShift;
                    }
                    else
                    {
                        contract.Previous.ShouldNotBeNull();
                        contract.WorkShift = contract.Previous.WorkShift;
                    }

                    contract.EffectiveSince = since;
                    contract.EmployeeSalaryForm = contract.Previous?.EmployeeSalaryForm ?? EmployeeSalaryForm.Royal;
                    contract.Exp = contract.Previous != null && contract.SubType != EmploymentSubType.DI
                        ? contract.Previous.Exp
                        : exp;

                    contract.PersonId = person.Id;
                    contract.Code = generator.CreateEmploymentCode(contract.EffectiveSince, contract.Type);
                    contract.SetGroup(generator.CreateOrUpdateEmploymentGroupId(contract.GroupId));

                    if (contract.Type != EmploymentType.B)
                    {
                        //RULE: La ubicación se debe dar por la plantilla o un supuesto puesto a ocupar
                        var wp = workPlaceManager.GetByCode(ouCode);

                        contract.OrganizationUnitId = wp.Id;
                        contract.WorkPlacePaymentCode = wp.WorkPlacePayment.Code;
                        contract.ThirdLevelCode = wp.Code;
                        contract.ThirdLevelDisplayName = wp.DisplayName;
                        contract.SecondLevelCode = wp.Parent.Code;
                        contract.SecondLevelDisplayName = wp.Parent.DisplayName;
                        contract.FirstLevelCode = wp.Parent.Parent.Code;
                        contract.FirstLevelDisplayName = wp.Parent.Parent.DisplayName;
                        //FIXME: contract.CenterCost = wp.CenterCosts.FirstOrDefault()?.Code ?? cc;

                        var occupation = occupationRepository.GetByCode(occupationCode);
                        contract.ComplexityGroup = occupation.Group.Group;
                        contract.Occupation = occupation.DisplayName;
                        contract.OccupationCategory = occupation.Category.Code;
                        contract.OccupationCode = occupation.Code;
                        contract.Responsibility = occupation.Responsibility.DisplayName;
                        contract.Salary = occupation.Group.BaseSalary;

                        contract.ExpirationDate = !expiration.IsNullOrEmpty() ? DateTime.Parse(expiration) : null;
                        if (contract.ExpirationDate != null &&
                            pattern.WorkingPeriods.HasIntersectionPeriods(contract.ExpirationDate.Value))
                        {
                            var periods = pattern.WorkingPeriods.IntersectionPeriods(contract.ExpirationDate.Value);
                            periods.SortByEnd(ListSortDirection.Descending);
                            contract.SetExpirationDate(periods.End);
                        }
                    }
                    else
                    {
                        contract.Previous.ShouldNotBeNull();
                        contract.OrganizationUnitId = contract.Previous.OrganizationUnitId;
                        contract.WorkPlacePaymentCode = contract.Previous.WorkPlacePaymentCode;
                        contract.ThirdLevelCode = contract.Previous.ThirdLevelCode;
                        contract.ThirdLevelDisplayName = contract.Previous.ThirdLevelDisplayName;
                        contract.SecondLevelCode = contract.Previous.SecondLevelCode;
                        contract.SecondLevelDisplayName = contract.Previous.SecondLevelDisplayName;
                        contract.FirstLevelCode = contract.Previous.FirstLevelCode;
                        contract.FirstLevelDisplayName = contract.Previous.FirstLevelDisplayName;
                        contract.CenterCost = contract.Previous.CenterCost;
                        contract.ComplexityGroup = contract.Previous.ComplexityGroup;
                        contract.Occupation = contract.Previous.Occupation;
                        contract.OccupationCategory = contract.Previous.OccupationCategory;
                        contract.OccupationCode = contract.Previous.OccupationCode;
                        contract.Responsibility = contract.Previous.Responsibility;
                        contract.Salary = contract.Previous.Salary;
                        contract.ExpirationDate = null;
                        contract.Plus.AddRange(contract.Previous.Plus);
                    }

                    contract.ByOfficial = byOfficial;
                    contract.ByAssignment = byAssignment;
                    contract.CalculateSalary();
                    contract.SetRatePerHour();
                    contract.Review = ReviewStatus.Confirmed;
                    employmentRepository.Insert(contract);
                    unitOfWorkManager.Current.SaveChanges();
                }
            });
        }

        [Fact]
        public void Get_timeline_by_ou_Test()
        {
            var tempFileCacheManager = Resolve<ITempFileCacheManager>();
            var documentsExporter = Resolve<IEmploymentDocumentExcelExporter>();
            var workRelationshipService = Resolve<IWorkRelationsAppService>();
            var blobStorage = Resolve<IBlobContainer<HumanResourcesContainer>>();
            var filter = new FilterRequest() {Exp = 22631, Year = 2025};
            var documents = workRelationshipService.GetTimelineForEmploymentDocuments(filter);

            documents.ShouldNotBeNull();

            FileDto exportToFile = documentsExporter.ExportToFile(@"employment\timeline", documents.Items.ToList());
            var buffer = tempFileCacheManager.GetFile(exportToFile.FileToken);
            AsyncHelper.RunSync(() => blobStorage.SaveAsync(exportToFile.FileName, buffer, true));
        }

        [Fact]
        public async Task Get_timeline_by_ou_TestAsync()
        {
            var tempFileCacheManager = Resolve<ITempFileCacheManager>();
            var documentsExporter = Resolve<IEmploymentDocumentExcelExporter>();
            var workRelationshipService = Resolve<IWorkRelationsAppService>();
            var blobStorage = Resolve<IBlobContainer<HumanResourcesContainer>>();
            var documents = await workRelationshipService.GetTimelineForEmploymentDocumentsAsync(new FilterRequest() { MaxResultCount = int.MaxValue });

            documents.ShouldNotBeNull();

            FileDto exportToFile = await documentsExporter.ExportToFileAsync(@"employment\timeline", documents.Items.ToList());

            var buffer = tempFileCacheManager.GetFile(exportToFile.FileToken);
            await blobStorage.SaveAsync(exportToFile.FileName, buffer, true);
        }

        [Fact]
        public void Get_documents_by_ou_Test()
        {
            var tempFileCacheManager = Resolve<ITempFileCacheManager>();
            var documentsExporter = Resolve<IEmploymentDocumentExcelExporter>();
            var workRelationshipService = Resolve<IWorkRelationsAppService>();
            var blobStorage = Resolve<IBlobContainer<HumanResourcesContainer>>();
            var documents = workRelationshipService.GetEmploymentDocuments(new FilterRequest(){MaxResultCount = int.MaxValue});

            documents.ShouldNotBeNull();

            FileDto exportToFile = documentsExporter.ExportToFile(@"employment\documents", documents.Items.ToList());
            var buffer = tempFileCacheManager.GetFile(exportToFile.FileToken);
            AsyncHelper.RunSync(() => blobStorage.SaveAsync(exportToFile.FileName, buffer, true));
        }

        [Fact]
        public async Task Get_documents_by_ou_TestAsync()
        {
            var tempFileCacheManager = Resolve<ITempFileCacheManager>();
            var documentsExporter = Resolve<IEmploymentDocumentExcelExporter>();
            var workRelationshipService = Resolve<IWorkRelationsAppService>();
            var blobStorage = Resolve<IBlobContainer<HumanResourcesContainer>>();

            var documents = await workRelationshipService.GetEmploymentDocumentsAsync(new FilterRequest() { MaxResultCount = int.MaxValue });

            documents.ShouldNotBeNull();

            FileDto exportToFile = await documentsExporter.ExportToFileAsync(@"employment\documents", documents.Items.ToList());
            var buffer = tempFileCacheManager.GetFile(exportToFile.FileToken);
            await blobStorage.SaveAsync(exportToFile.FileName, buffer, true);
        }

        [Fact]
        public void Get_unemployed_Test()
        {
            var tempFileCacheManager = Resolve<ITempFileCacheManager>();
            var documentsExporter = Resolve<IEmploymentDocumentExcelExporter>();
            var workRelationshipService = Resolve<IWorkRelationsAppService>();
            var unemployed = workRelationshipService.GetUnemployed(new FilterRequest());
            var blobStorage = Resolve<IBlobContainer<HumanResourcesContainer>>();

            unemployed.ShouldNotBeNull();

            FileDto exportToFile = documentsExporter.ExportToFile(@"employment\unemployed", unemployed.Items.ToList());
            var buffer = tempFileCacheManager.GetFile(exportToFile.FileToken);
            AsyncHelper.RunSync(() => blobStorage.SaveAsync(exportToFile.FileName, buffer, true));
        }

        [Fact]
        public async Task Get_unemployed_TestAsync()
        {
            var tempFileCacheManager = Resolve<ITempFileCacheManager>();
            var documentsExporter = Resolve<IEmploymentDocumentExcelExporter>();
            var workRelationshipService = Resolve<IWorkRelationsAppService>();
            var blobStorage = Resolve<IBlobContainer<HumanResourcesContainer>>();
            var unemployed = await workRelationshipService.GetUnemployedAsync(new FilterRequest());

            unemployed.ShouldNotBeNull();

            FileDto exportToFile = await documentsExporter.ExportToFileAsync(@"employment\unemployed", unemployed.Items.ToList());
            var buffer = tempFileCacheManager.GetFile(exportToFile.FileToken);
            await blobStorage.SaveAsync(exportToFile.FileName, buffer, true);
        }

        [Fact]
        public void Get_expired_documents_Test()
        {
            var tempFileCacheManager = Resolve<ITempFileCacheManager>();
            var documentsExporter = Resolve<IEmploymentDocumentExcelExporter>();
            var workRelationshipService = Resolve<IWorkRelationsAppService>();
            var blobStorage = Resolve<IBlobContainer<HumanResourcesContainer>>();
            var expiredSoonDocuments = workRelationshipService.GetExpiredDocuments();

            expiredSoonDocuments.ShouldNotBeNull();

            FileDto exportToFile = documentsExporter.ExportToFile(@"employment\expired", expiredSoonDocuments.Items.ToList());
            var buffer = tempFileCacheManager.GetFile(exportToFile.FileToken);
            AsyncHelper.RunSync(() => blobStorage.SaveAsync(exportToFile.FileName, buffer, true));
        }

        [Fact]
        public async Task Get_expired_documents_TestAsync()
        {
            var tempFileCacheManager = Resolve<ITempFileCacheManager>();
            var documentsExporter = Resolve<IEmploymentDocumentExcelExporter>();
            var workRelationshipService = Resolve<IWorkRelationsAppService>();
            var blobStorage = Resolve<IBlobContainer<HumanResourcesContainer>>();
            var expiredSoonDocuments = await workRelationshipService.GetExpiredDocumentsAsync();

            expiredSoonDocuments.ShouldNotBeNull();

            FileDto exportToFile = await documentsExporter.ExportToFileAsync(@"employment\expired", expiredSoonDocuments.Items.ToList());
            var buffer = tempFileCacheManager.GetFile(exportToFile.FileToken);
            await blobStorage.SaveAsync(exportToFile.FileName, buffer, true);
        }

        [Fact]
        public void Export_work_relationship_Test()
        {
            var workRelationshipService = Resolve<IWorkRelationsAppService>();
            workRelationshipService.ExportCurrentWorkRelationship();
        }

        [Fact]
        public async Task Export_work_relationship_TestAsync()
        {
            using var disposable = KontecgSession.Use(1, 2);
            {
                var workRelationshipService = Resolve<IWorkRelationsAppService>();
                await workRelationshipService.ExportCurrentWorkRelationshipAsync();
            }
        }

        [Theory]
        [InlineData(21000)]
        public void Map_employment_document_Test(int exp)
        {
            var workRelationshipService = Resolve<IWorkRelationsAppService>();

            var legalEmploymentDocumentDto = workRelationshipService.GetEmploymentDocumentByExp(new FilterRequest(){Exp = exp});

            LegalEmploymentDocument paper = new();
            paper.DataSource = new List<LegalEmploymentDocumentDto> { legalEmploymentDocumentDto, legalEmploymentDocumentDto };

            var blobStorage = Resolve<IBlobContainer<HumanResourcesContainer>>();
            using var ms = new MemoryStream();
            paper.ExportToPdf(ms);
            AsyncHelper.RunSync(() => blobStorage.SaveAsync($"employment\\{exp}.pdf", ms.ToArray(), true));
        }

        [Theory]
        [InlineData(24973)]
        [InlineData(17956)]
        [InlineData(22631)]
        [InlineData(23689)]
        [InlineData(24688)]
        public async Task Map_employment_document_TestAsync(int exp)
        {
            var workRelationshipService = Resolve<IWorkRelationsAppService>();

            var legalEmploymentDocumentDto = await workRelationshipService.GetEmploymentDocumentByExpAsync(new FilterRequest() { Exp = exp });

            LegalEmploymentDocument paper = new();
            paper.DataSource = new List<LegalEmploymentDocumentDto> { legalEmploymentDocumentDto, legalEmploymentDocumentDto };
            var blobStorage = Resolve<IBlobContainer<HumanResourcesContainer>>();
            using var ms = new MemoryStream();
            await paper.ExportToPdfAsync(ms);
            await blobStorage.SaveAsync($"employment\\{exp}.pdf", ms.ToArray(), true);
        }

        [Fact]
        public void Save_all_valid_indexes_Test()
        {
            var settings = Resolve<SettingManager>();

            var record = new EmploymentIndexSettingRecord(1, 40000, 60000, 69999, 48000, 59999, 70000, 79999);

            settings.ChangeSettingForCompany(1, SGNOMSettings.Employment.IndexRanges, record.ToJsonString());
        }

        [Fact]
        public void Get_all_valid_indexes_Test()
        {
            var indexRepository = Resolve<IEmploymentIndexRepository>();

            var availableExp = indexRepository.GetAllAvailableExp(ContractType.I, ContractSubType.I);

            availableExp.ShouldNotBeEmpty();
        }

        [Fact]
        public void Create_a_copy_Test()
        {
            using var disposable = KontecgSession.Use(1, 2);
            {
                WithUnitOfWork(() =>
                {
                    var repo = Resolve<IRepository<EmploymentDocumentToGenerate, long>>();
                    var docs = repo.GetAll().Where(d => !d.Confirmed).ToList();
                    foreach (var documentToGenerate in docs)
                    {
                        documentToGenerate.Confirmed = true;
                        repo.Update(documentToGenerate);
                    }
                });
            }
        }

        [Fact]
        public void Clean_all_copies_Test()
        {
            using var disposable = KontecgSession.Use(1, 2);
            {
                WithUnitOfWork(() =>
                {
                    var repo = Resolve<IRepository<EmploymentDocumentToGenerate, long>>();
                    var docs = repo.GetAll().Where(d => d.Confirmed).ToList();
                    foreach (var documentToGenerate in docs)
                    {
                        documentToGenerate.Confirmed = false;
                        repo.Update(documentToGenerate);
                    }
                });
            }
        }

        [Fact]
        public void Clone_all_documents_Test()
        {
            using var disposable = KontecgSession.Use(1, 2);
            {
                WithUnitOfWork(() =>
                {
                    var repo = Resolve<IEmploymentRepository>();
                    var documentGenerator = Resolve<IEmploymentDocumentGenerator>();
                    var docs = repo.CurrentRelationship().Where(r => r.OrganizationUnitId != 1).ToList();
                    foreach (var documentToCopy in docs)
                    {
                        var cloned = documentGenerator.Clone(documentToCopy);
                        cloned.MadeOn = WorkCalendarTool.GetStartOfMonth(cloned.MadeOn);
                        cloned.EffectiveSince = WorkCalendarTool.GetStartOfMonth(cloned.EffectiveSince);
                        repo.Insert(cloned);
                    }
                });

                WithUnitOfWork(() =>
                {
                    var workRelationshipService = Resolve<IWorkRelationsAppService>();
                    workRelationshipService.ReviewOrConfirm(new FindEmploymentInputDto { Target = ReviewStatus.Confirmed });
                });
            }
        }

        [Fact]
        public void Review_all_documents_Test()
        {
            using var disposable = KontecgSession.Use(1, 2);
            {
                WithUnitOfWork(() =>
                {
                    var workRelationshipService = Resolve<IWorkRelationsAppService>();
                    workRelationshipService.ReviewOrConfirm(new FindEmploymentInputDto { Source = ReviewStatus.Reviewed, Target = ReviewStatus.Confirmed});
                });
            }
        }

        [Fact]
        public async Task Update_all_person_data_TestAsync()
        {
            using var disposable = KontecgSession.Use(1, 2);
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var dataCollectorService = Resolve<IDataCollectorService>();
                    await dataCollectorService.ForcePersonToChangeTheirScholarshipDataAsync(1);
                });
            }
        }
    }
}
