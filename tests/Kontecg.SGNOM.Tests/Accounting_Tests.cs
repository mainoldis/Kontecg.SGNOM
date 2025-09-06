using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Accounting;
using Kontecg.Accounting.Dto;
using Kontecg.BlobStoring;
using Kontecg.Collections.Extensions;
using Kontecg.Domain.Repositories;
using Kontecg.EFCore.EFPlus;
using Kontecg.HumanResources;
using Kontecg.ObjectMapping;
using Kontecg.Salary;
using Kontecg.Storage.Blobs;
using Kontecg.Taxes;
using Kontecg.Threading;
using Kontecg.Timing;
using Kontecg.Views.Accounting;
using Kontecg.Workflows;
using Kontecg.WorkRelations;
using NMoneys;
using Shouldly;
using Xunit;

namespace Kontecg.SGNOM.Tests
{
    public class Accounting_Tests : SGNOMModuleTestBase
    {
        private readonly IAccountingAppService _accountingAppService;

        public Accounting_Tests()
        {
            var admin = GetDefaultCompanyAdmin();
            KontecgSession.UserId = admin.Id;
            KontecgSession.CompanyId = admin.CompanyId;

            _accountingAppService = Resolve<IAccountingAppService>();
        }

        [Fact]
        public async Task Get_personal_accounting_info_Test()
        {
            var accountingService = LocalIocManager.Resolve<IAccountingAppService>();
            var blobStorage = Resolve<IBlobContainer<AccountingContainer>>();
            var accountingInfo = await accountingService.GetAllAccountingInfoAsync();
            accountingInfo.ShouldNotBeNull();

            LegalPersonalAccountingDocument paper = new();
            paper.DataSource = new List<GeneralAccountingInfo> { accountingInfo };

            using var s = new MemoryStream();
            await paper.ExportToPdfAsync(s);
            await blobStorage.SaveAsync($"{AccountingContainer.Documents}\\Datos_contables_personales_{accountingInfo.Company.CompanyName}.pdf", s.ToArray(), true);
        }

        [Fact]
        public async Task Store_personal_accounting_info_Test()
        {
            var personalAccountingHelper = LocalIocManager.Resolve<IPersonalAccountingHelper>();

            var personalAccountingInfos = await personalAccountingHelper.CreatePersonalAccountingInfoAsync();
            personalAccountingInfos.ShouldNotBeEmpty();

            await personalAccountingInfos.ForEachAsync(pa => personalAccountingHelper.SaveAsync(pa));
        }

        [Fact]
        public void Create_accounting_document_summary_Test()
        {
            KontecgSession.Use(1, 2);
            var summaryRepository = Resolve<IRepository<AccountingDocumentSummary,long>>();
            var periodRepository = Resolve<IRepository<Period>>();
            var taxManager = Resolve<TaxManager>();
            var taxRepository = Resolve<IRepository<AccountingTaxSummary, long>>();
            var distributionDocumentRepository = Resolve<IRepository<TimeDistributionDocument>>();
            var distributionRepository = Resolve<IRepository<TimeDistribution, long>>();

            WithUnitOfWork(KontecgSession.CompanyId, () =>
            {
                var lastMonth = new WorkMonth(Clock.Now, WorkPattern.Default).GetPreviousMonth();
                var period = periodRepository.GetAll().FirstOrDefault(p => p.Year == lastMonth.Year && p.Month == lastMonth.Month && p.Status == PeriodStatus.Opened);
                period.ShouldNotBeNull("Debe existir un período abierto");

                var input = new AccountingDocumentInputDto
                {
                    Reference = "najust",
                    ReferenceGroup = "sgnom",
                    Year = period.Year,
                    Month = (int) period.Month
                };
                var document = _accountingAppService.CreateDocument(input);
                document.ShouldNotBeNull();

                var distributionDocumentsIds = distributionDocumentRepository.GetAll()
                    .Where(doc => doc.PeriodId == period.Id && doc.Review == ReviewStatus.Confirmed).Select(s => s.Id).ToList();

                var distributions = distributionRepository.GetAllIncluding(t => t.PaymentDefinition, t => t.Plus)
                                                          .Where(t => distributionDocumentsIds.Contains(t.DocumentId) &&
                                                                      t.Status == AccountingNoteStatus.ToMake).ToList()
                                                          .GroupBy(k => new
                                                          {
                                                              k.CompanyId,
                                                              k.PersonId,
                                                              k.GroupId,
                                                              Currency = k.Currency.Value
                                                          })
                                                          .Select(e => new
                                                          {
                                                              e.Key.CompanyId,
                                                              e.Key.PersonId,
                                                              e.Key.GroupId,
                                                              e.Key.Currency,
                                                              Amount = e.Aggregate(Money.Zero(e.Key.Currency), (m, f) => Money.Add(m, f.Amount ?? Money.Zero(e.Key.Currency))),
                                                              ReservedForHoliday = e.Sum(f => f.ReservedForHoliday ?? 0),
                                                              AmountReservedForHoliday = e.Aggregate(Money.Zero(e.Key.Currency), (m, f) => Money.Add(m, f.AmountReservedForHoliday ?? Money.Zero(e.Key.Currency)))
                                                          })
                                                          .ToList();

                distributions.ShouldNotBeEmpty();

                var taxes = new List<AccountingTaxSummary>();
                var summary = new List<AccountingDocumentSummary>();

                distributions.ForEach(k => taxes.Add(new AccountingTaxSummary(document.Id, k.PersonId, k.GroupId, taxManager.Calculate(k.PersonId, k.GroupId, TaxType.SocialSecurity, k.Amount), k.Currency, TaxType.SocialSecurity)));
                distributions.ForEach(k => taxes.Add(new AccountingTaxSummary(document.Id, k.PersonId, k.GroupId, taxManager.Calculate(k.PersonId, k.GroupId, TaxType.Income, k.Amount), k.Currency, TaxType.Income)));

                distributions.ForEach(k => summary.Add(new AccountingDocumentSummary(document.Id, k.PersonId, k.GroupId,
                    k.Amount, Money.Zero(k.Currency), k.ReservedForHoliday, k.AmountReservedForHoliday,
                    taxManager.Calculate(k.PersonId, k.GroupId, TaxType.SocialSecurity, k.Amount),
                    taxManager.Calculate(k.PersonId, k.GroupId, TaxType.Income, k.Amount),
                    Money.Zero(k.Currency), k.Currency)));

                for (int i = 0; i < taxes.Count; i++)
                {
                    if (taxes[i].Tax > Money.Zero(taxes[i].Currency))
                        taxRepository.Insert(taxes[i]);
                }

                for (int i = 0; i < summary.Count; i++)
                {
                    if (summary[i].NetIncome > Money.Zero(taxes[i].Currency))
                        summaryRepository.Insert(summary[i]);
                }

                AsyncHelper.RunSync(() =>
                    distributionRepository.BatchUpdateAsync(
                        d => new TimeDistribution() { Status = AccountingNoteStatus.Made },
                        distribution => distribution.Status == AccountingNoteStatus.ToMake));
            });
        }

        [Theory]
        [InlineData(2400, 120)]
        [InlineData(8400, 420)]
        [InlineData(9520, 476)]
        [InlineData(15230, 773)]
        [InlineData(32000, 2450)]
        public void Calculate_social_security_tax_Test(decimal salary, decimal tax)
        {
            var taxManager = Resolve<TaxManager>();
            WithUnitOfWork(1, () =>
            {
                var result = taxManager.Calculate(0, Guid.Empty, TaxType.SocialSecurity, salary);

                result.ShouldBeEquivalentTo(tax);
            });
        }

        [Fact]
        public void Calculate_income_tax_Test()
        {
            KontecgSession.Use(1, 2);
            var taxManager = Resolve<TaxManager>();
            var employment = Resolve<IEmploymentRepository>();
            WithUnitOfWork(() =>
            {
                var doc = employment.LastByExp(22631);
                var result = taxManager.Calculate(doc[0].PersonId, doc[0].GroupId, TaxType.Income, 4422.07M);
                result = taxManager.Calculate(doc[0].PersonId, doc[0].GroupId, TaxType.Income, 15564.73M, result);

                var socialTax = taxManager.Calculate(doc[0].PersonId, doc[0].GroupId, TaxType.SocialSecurity, 15564.73M);
            });
        }

        [Fact]
        public void Add_income_tax_to_person_Test()
        {
            KontecgSession.Use(1, 2);
            var taxManager = Resolve<TaxManager>();
            var employment = Resolve<IEmploymentRepository>();
            WithUnitOfWork(() =>
            {
                var doc = employment.LastByExp(22631);
                taxManager.CreateTaxForPerson(new PersonTax(doc[0].PersonId, doc[0].GroupId, "2400 de Seguridad Social, porque me da la gana", TaxType.SocialSecurity, MathType.MinimumWage, 2400));
            });
        }
    }
}
