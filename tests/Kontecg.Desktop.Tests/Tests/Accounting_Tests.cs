using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Elsa.Extensions;
using Itenso.TimePeriod;
using Kontecg.Accounting;
using Kontecg.Accounting.Dto;
using Kontecg.Accounting.Formulas;
using Kontecg.BlobStoring;
using Kontecg.Domain.Repositories;
using Kontecg.Extensions;
using Kontecg.IO;
using Kontecg.ObjectMapping;
using Kontecg.Storage.Blobs;
using Kontecg.Timing;
using Kontecg.Timing.Dto;
using Kontecg.Views.Accounting;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using static Kontecg.Accounting.Formulas.FormulaEvaluator;

namespace Kontecg.Desktop.Tests
{
    public class Accounting_Tests : DesktopTestModuleTestBase
    {
        private readonly AccountingManager _accountingManager;
        private readonly IAccountingAppService _accountingAppService;
        private Faker<ViewNameResultRecord> _accountingVoucherNoteRecordFaker;

        private readonly ITestOutputHelper _output;

        public Accounting_Tests(ITestOutputHelper output)
            : base()
        {
            _output = output;

            var admin = GetDefaultCompanyAdmin();
            KontecgSession.UserId = admin.Id;
            KontecgSession.CompanyId = admin.CompanyId;

            _accountingManager = Resolve<AccountingManager>();
            _accountingAppService = Resolve<IAccountingAppService>();
        }
        
        [Theory]
        [InlineData(1)]
        public async Task Get_document_Test(int documentId)
        {
            // Arrange
            KontecgSession.Use(1, 2);

            // Act
            var document = await _accountingAppService.GetAccountingDocumentAsync(new AccountingDocumentFilterDto() { DocumentId = documentId });

            var input = new PeriodInputDto { ModuleKey = "SGNOM" };

            var allDocuments = await _accountingAppService.GetAllDocumentsByPeriodAsync(input);

            var allVouchers = await _accountingAppService.GetAllVouchersByPeriodAsync(input);

            // Assert
            document.ShouldNotBeNull();
            document.Id.ShouldBe(documentId);
            document.Description.ShouldNotBeNullOrEmpty();

            allDocuments.ShouldNotBeNull();
            allDocuments.Items.ShouldNotBeEmpty();
            allDocuments.Items.ShouldContain(d => d.Id == documentId);

            allVouchers.ShouldNotBeNull();
            allVouchers.Items.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Get_voucher_document_Test()
        {
            KontecgSession.Use(1, 2);
            
            var input = new AccountingVoucherFilterDto
            {
                DocumentId = 68,
                ScopeId = 1
            };

            var voucher = await _accountingAppService.GetAccountingVoucherInfoAsync(input);

            LegalAccountingVoucher paper = new();
            paper.DataSource = new List<AccountingVoucherOutputDto> { voucher };
            paper.Parameters["Scope"].Value = input.ScopeId;

            var path = $"{AccountingContainer.Vouchers}\\{voucher.Document.Period.Year}\\{voucher.Document.Period.Month}\\{voucher.Description.NormalizeToFileName()}_{voucher.Code}_{input.ScopeId}.pdf";

            var blobStorage = Resolve<IBlobContainer<AccountingContainer>>();
            using var ms = new MemoryStream();
            await paper.ExportToPdfAsync(ms);
            await blobStorage.SaveAsync(path, ms.ToArray(), true);
        }
        
        [Fact]
        public async Task Create_account_definition_Test()
        {
            KontecgSession.Use(1, 2);
            await _accountingManager.InsertOrUpdateDefinitionAsync(new AccountDefinition(999, 0, 0, 0, "Resultado", AccountKind.Credit,"RESULT"));
        }

        [Fact]
        public async Task Execute_view_Test()
        {
            KontecgSession.Use(1, 2);
            var repo = Resolve<IViewRepository>();
            await WithUnitOfWorkAsync(1, async () =>
            {
                //1. Obtener los datos bases para los cálculos para comprobantes
                var accountingData = await _accountingAppService.GetAllBaseDataAsync();
                //2. Inicializar el contexto de evaluación con las variables necesarias
                var ctx = new ExecutionContext();
                ctx.SetVariable("ACCOUNTS",
                    new TableValue<AccountValue>(accountingData.Accounts.Items.Where(a => a.IsActive)
                                                               .Select(r => new AccountValue(r.Account, r.SubAccount, r.SubControl, r.Analysis, r.Reference))
                                                               .ToList()));
                ctx.SetVariable("EXPENSE_ITEMS", new TableValue<ExpenseItemValue>(accountingData
                    .ExpenseItems.Items.Where(a => a.IsActive)
                    .Select(r => new ExpenseItemValue(r.Code, r.Reference))
                    .ToList()));
                ctx.SetVariable("CLASSIFIERS",
                    new TableValue<ClassifierValue>(accountingData.Classifiers.Items
                                                                  .Select(r => new ClassifierValue(r.Id, r.Description)).ToList()));
                ctx.SetVariable("NOTES", new TableValue<NoteValue>([]));

                //3. Obtener el documento de comprobante a extraer
                var input = new AccountingVoucherFilterDto {DocumentId = 68};
                var voucherOutputDto = await _accountingAppService.GetAccountingVoucherInfoAsync(input);

                //4. Selecciono las vistas a ejecutar en dependencia del tipo de documento del comprobante
                var viewsToExecute = voucherOutputDto.Document.DocumentDefinition.Views;

                //5. Ejecutar la vista que contiene los datos
                IList<ViewNameResultRecord> records = new List<ViewNameResultRecord>();
                foreach (var viewNameDto in viewsToExecute)
                    records.AddRange(await repo.ExecuteViewAsync(viewNameDto.Name, voucherOutputDto.Document.Code));

                //6. Por cada clasificador calcular los asientos contables
                foreach (var scope in accountingData.Classifiers.Items)
                {
                    foreach (var rec in records)
                    {
                        //6.1 Convierto las variables listas para ser usadas en el contexto
                        foreach (var variable in rec.ToPropertyDictionary())
                        {
                            if(variable.Value is not null)
                                switch (variable.Value)
                                {
                                    case int:
                                        ctx.SetVariable(variable.Key,
                                            new IntegerValue(Convert.ToInt32(variable.Value)));
                                        break;
                                    case decimal or float or double:
                                        ctx.SetVariable(variable.Key,
                                            new DecimalValue(Convert.ToDecimal(variable.Value)));
                                        break;
                                    case bool:
                                        ctx.SetVariable(variable.Key,
                                            new BooleanValue(Convert.ToBoolean(variable.Value)));
                                        break;
                                    default:
                                        ctx.SetVariable(variable.Key, new StringValue(variable.Value.ToString()));
                                        break;
                                }
                        }

                        ctx.SetVariable("SCOPE", new IntegerValue(scope.Id)); //Mejor usar la variable

                        //6.2 Construyo la fórmula pasando el contexto
                        var formula = new FormulaEvaluator(ctx);

                        //6.3 Obtengo el script a evaluar
                        var fc = accountingData.Functions.Items.FirstOrDefault(fc => fc.Name == rec.Fc && fc.IsActive)
                                               ?.Formula ?? "";

                        //6.4 Evalúo la fórmula
                        formula.Evaluate(fc);
                    }
                }

                //7. Obtengo las notas resultantes
                var notes = ((List<NoteValue>)ctx.GetVariable("NOTES").RawValue)
                            .OrderBy(s => s.ScopeId)
                            .ThenByDescending(n => n.Operation)
                            .ThenBy(o => o.Account)
                            .ThenBy(o => o.SubAccount)
                            .ThenBy(o => o.SubControl)
                            .ThenBy(o => o.Analysis).ToList();

                //8. El último paso sería guardar las notas generadas para el comprobante
            });
        }

        [Theory]
        [InlineData(68)]
        public async Task Export_to_xml_Test(int documentId)
        {
            KontecgSession.Use(1, 2);
            var input = new AccountingVoucherFilterDto {DocumentId = documentId, ScopeId = 1};
            var voucher = await _accountingAppService.GetXmlVoucherAsync(input);
            var path = $"{AccountingContainer.Vouchers}\\{voucher.Document.Period.Year}\\{voucher.Document.Period.Month}\\{voucher.XmlFileExported.FileName.NormalizeToFileName()}_{voucher.Code}_{input.ScopeId}{voucher.XmlFileExported.FileType}";
            var blobStorage = Resolve<IBlobContainer<AccountingContainer>>();
            await blobStorage.SaveAsync(path, voucher.XmlFileExported.File, true);
        }

        [Fact]
        public void Simple_expression_parse_Test()
        {
            var ctx = new ExecutionContext();
            // Inicializar variable R = 0
            ctx.SetVariable("R", new DecimalValue(0));

            var formula = new FormulaEvaluator(ctx);
            // Registrar función SQRT
            formula.RegisterFunction("SQRT", 1, 1, (ctx, args) =>
            {
                var val = args[0].AsDecimal();
                return new DecimalValue((decimal)Math.Sqrt((double)val));
            });

            // Primera evaluación: "R:= 50*7+R"
            formula.Evaluate("R:= 50*7+R");

            // Segunda evaluación: "R:= 7*R"
            formula.Evaluate("R:= SQRT(7*R)");

            // Obtener valor final de R
            var r = ctx.GetVariable("R").AsDecimal();
        }

        [Fact]
        public void Account_function_parse_Test()
        {
            WithUnitOfWork(1, () =>
            {
                var centerCosts = _accountingManager.GetAllCenterCostDefinitions();
                _accountingVoucherNoteRecordFaker = new Faker<ViewNameResultRecord>()
                                                    .RuleFor(u => u.Trans, f => "1")
                                                    .RuleFor(u => u.DocType, f => "4") // Ejemplo valor fijo
                                                    .RuleFor(u => u.DocCod, f => f.Random.ReplaceNumbers("###.25", '#'))
                                                    .RuleFor(u => u.Cc, (f, u) => f.PickRandom(centerCosts).Code)
                                                    .RuleFor(u => u.Cnt, (f, u) => u.Cnt = centerCosts.Single(c => c.Code == u.Cc).AccountDefinition.Account)
                                                    .RuleFor(u => u.Exp, f => f.Random.Int(1, 80000))
                                                    .RuleFor(u => u.Tp, f => "504")
                                                    .RuleFor(u => u.Fc, f => "C_PPEN")
                                                    .RuleFor(u => u.Contrato, f => f.Random.Bool(0.2f))
                                                    .RuleFor(u => u.Jornalero, f => f.Random.Bool(1f)) // Ejemplo valor fijo
                                                    .RuleFor(u => u.Caja, f => f.Random.Bool(0.3f)) // Ejemplo valor fijo
                                                    .RuleFor(u => u.Currency, f => "CUP")
                                                    .RuleFor(u => u.Imp, f => f.Finance.Amount(2500, 15000))
                                                    .RuleFor(u => u.ImpVac, (f, u) => u.Imp * 0.0909M)
                                                    .RuleFor(u => u.AporteSegSoc, (_, u) => u.Imp * 0.125M)
                                                    .RuleFor(u => u.AporteFuerzaLaboral, (_, u) => u.Imp * 0.05M)
                                                    .RuleFor(u => u.ProvisionSegSoc, (_, u) => u.Imp * 0.015M);

                var records = _accountingVoucherNoteRecordFaker.Generate(5);
                var accountingData = _accountingAppService.GetAllBaseData();
                //Preparo el contexto de evaluación
                var ctx = new FormulaEvaluator.ExecutionContext();
                ctx.SetVariable("ACCOUNTS",
                    new FormulaEvaluator.TableValue<AccountValue>(accountingData.Accounts.Items.Where(a => a.IsActive)
                        .Select(r => new AccountValue(r.Account, r.SubAccount, r.SubControl, r.Analysis, r.Reference))
                        .ToList()));
                ctx.SetVariable("EXPENSE_ITEMS", new FormulaEvaluator.TableValue<ExpenseItemValue>(accountingData
                    .ExpenseItems.Items.Where(a => a.IsActive)
                    .Select(r => new ExpenseItemValue(r.Code, r.Reference))
                    .ToList()));
                ctx.SetVariable("CLASSIFIERS",
                    new FormulaEvaluator.TableValue<ClassifierValue>(accountingData.Classifiers.Items
                        .Select(r => new ClassifierValue(r.Id, r.Description)).ToList()));
                ctx.SetVariable("NOTES", new FormulaEvaluator.TableValue<NoteValue>([]));

                foreach (var scope in (int[]) [1, 2])
                {
                    foreach (var rec in records)
                    {
                        //Convierto las variables listas para ser usadas en el contexto
                        foreach (var variable in rec.ToPropertyDictionary())
                        {
                            if (variable.Value is not null)
                                switch (variable.Value)
                                {
                                    case int:
                                        ctx.SetVariable(variable.Key,
                                            new IntegerValue(Convert.ToInt32(variable.Value)));
                                        break;
                                    case decimal or float or double:
                                        ctx.SetVariable(variable.Key,
                                            new DecimalValue(Convert.ToDecimal(variable.Value)));
                                        break;
                                    case bool:
                                        ctx.SetVariable(variable.Key,
                                            new BooleanValue(Convert.ToBoolean(variable.Value)));
                                        break;
                                    default:
                                        ctx.SetVariable(variable.Key, new StringValue(variable.Value.ToString()));
                                        break;
                                }
                        }

                        ctx.SetVariable("SCOPE", new IntegerValue(scope));
                        
                        //Construyo la formula pasando el contexto
                        var formula = new FormulaEvaluator(ctx);

                        var sb = new StringBuilder();
                        var lines = AppFileHelper.ReadLines("Data\\formula_pensiones.txt");
                        foreach (var line in lines)
                            sb.Append(line);

                        formula.Evaluate(sb.ToString());
                    }
                }

                var notes = ((List<NoteValue>) ctx.GetVariable("NOTES").RawValue)
                            .OrderBy(s => s.ScopeId)
                            .ThenByDescending(n => n.Operation)
                            .ThenBy(o => o.Account)
                            .ThenBy(o => o.SubAccount)
                            .ThenBy(o => o.SubControl)
                            .ThenBy(o => o.Analysis).ToList();

            });
        }

        [Fact]
        public async Task InsertOrUpdateDefinitionAsync_Should_Insert_New_AccountDefinition()
        {
            await WithUnitOfWorkAsync(1, async () =>
            {
                // Arrange
                var input = new AccountDefinitionInputDto
                {
                    Account = 1000,
                    SubAccount = 0,
                    SubControl = 0,
                    Analysis = 0,
                    Description = "Test account",
                    Kind = "memo",
                    Reference = "TEST_ACCOUNT"

                };

                // Act
                await _accountingAppService.CreateAccountDefinitionAsync(input);

                // Assert
                var insertedDefinition = (await _accountingManager.GetAllAccountDefinitionsAsync()).FirstOrDefault(a => a.Reference == "TEST_ACCOUNT");
                insertedDefinition.ShouldNotBeNull();
                insertedDefinition.Description.ShouldBe("TEST ACCOUNT");
                insertedDefinition.Kind.ShouldBe(AccountKind.Memo);
            });
        }

        [Fact]
        public async Task InsertOrUpdateDefinitionAsync_Should_Insert_New_ExpenseItemDefinition()
        {
            KontecgSession.Use(1, 2); 
            // Arrange
            var input = new ExpenseItemDefinitionInputDto()
            {
                Code = 900000,
                Description = "Generic Expense Item"
            };

            // Act
            await _accountingAppService.CreateExpenseItemDefinitionAsync(input);

            // Assert
            var insertedDefinition = (await _accountingManager.GetAllExpenseItemDefinitionsAsync()).FirstOrDefault(a => a.Reference == "EG_900000");
            insertedDefinition.ShouldNotBeNull();
            insertedDefinition.Description.ShouldBe("GENERIC EXPENSE ITEM");
            insertedDefinition.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task Open_period_Test()
        {
            PeriodInfo periodInfo = null;
            KontecgSession.Use(1, 2);
            var opm = Resolve<PeriodManager>();
            var opmResultHelper = Resolve<KontecgPeriodResultTypeHelper>();

            var result = await opm.OpenAsync("SGNOM", 2025, YearMonth.July);

            if (result.Result != KontecgPeriodResultType.Success)
            {
                throw opmResultHelper.CreateExceptionForFailedOperationAttempt(result, "ECG");
            }

            periodInfo = result.Period;

            periodInfo.ShouldNotBeNull();
            //periodInfo.ShouldSatisfyAllConditions(p => p.Status.ShouldBe(PeriodStatus.Opened));
        }

        [Fact]
        public async Task Probe_mapping_from_period_Test()
        {
            KontecgSession.Use(1, 2);
            await WithUnitOfWorkAsync(1, async () =>
            {
                var mapper = Resolve<IObjectMapper>();
                var repository = Resolve<IRepository<Period>>();

                var period = await repository.FirstOrDefaultAsync(p =>
                    p.ReferenceGroup == "SGNOM" && p.Year == 2025 && p.Month == YearMonth.October);

                var periodInfoDtoMapped = mapper.Map<PeriodDto>(period);
                var accountingPeriodInfoMapped = mapper.Map<PeriodInfoDto>(period);
            });
        }

        [Fact]
        public async Task CreateOrUpdateDocumentAsync()
        {
            KontecgSession.Use(1, 2);

            var input = new AccountingDocumentInputDto()
            {
                Year = 2025,
                Month = 7,
                Reference = "",
                ReferenceGroup = "sgnom"
            };

            var output = await _accountingAppService.CreateDocumentAsync(input);
        }

        [Fact]
        public async Task CreateOrUpdateVoucherAsync()
        {
            KontecgSession.Use(1, 2);

            var input = new AccountingVoucherInputDto()
            {
                DocumentId = 9002,
                Description = "Comprobante de prueba"
            };

            var output = await _accountingAppService.CreateVoucherAsync(input);
        }
    }
}
