using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Kontecg.Accounting.Dto;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.EntityHistory;
using Kontecg.Extensions;
using Kontecg.Linq.Extensions;
using Kontecg.MimeTypes;
using Kontecg.MultiCompany;
using Kontecg.MultiCompany.Dto;
using Kontecg.Runtime.Session;
using Kontecg.Storage;
using Kontecg.Threading;
using Kontecg.Timing;
using Kontecg.Timing.Dto;
using Kontecg.UI;
using Kontecg.Workflows;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Accounting
{
    [KontecgAuthorize(PermissionNames.Accounting)]
    [UseCase(Description = "Servicio Contable")]
    public class AccountingAppService : KontecgAppServiceBase, IAccountingAppService
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IMimeTypeMap _mimeTypeMap;
        private readonly AccountingManager _accountingManager;
        private readonly IPersonalAccountingHelper _personalAccountingHelper;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<AccountingDocument> _accountingDocumentRepository;
        private readonly IRepository<AccountingVoucherDocument> _accountingVoucherDocumentRepository;
        private readonly PeriodManager _periodManager;
        private readonly IDocumentDefinitionRepository _documentDefinitionRepository;

        public AccountingAppService(
            AccountingManager accountingManager,
            IPersonalAccountingHelper personalAccountingHelper,
            IRepository<AccountingVoucherDocument> accountingVoucherDocumentRepository,
            IRepository<AccountingDocument> accountingDocumentRepository, 
            IDocumentDefinitionRepository documentDefinitionRepository,
            IRepository<Company> companyRepository, 
            PeriodManager periodManager, 
            IBinaryObjectManager binaryObjectManager, 
            ITempFileCacheManager tempFileCacheManager, 
            IMimeTypeMap mimeTypeMap)
        {
            _accountingManager = accountingManager;
            _personalAccountingHelper = personalAccountingHelper;
            _accountingVoucherDocumentRepository = accountingVoucherDocumentRepository;
            _accountingDocumentRepository = accountingDocumentRepository;
            _companyRepository = companyRepository;
            _periodManager = periodManager;
            _binaryObjectManager = binaryObjectManager;
            _tempFileCacheManager = tempFileCacheManager;
            _mimeTypeMap = mimeTypeMap;
            _documentDefinitionRepository = documentDefinitionRepository;

            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
        }

        #region Get Methods

        /// <inheritdoc />
        public CompanyInfoDto GetCompanyInfo(int? companyId = null)
        {
            try
            {
                var currentCompany = companyId != null ? GetCompany(companyId.Value) : GetCurrentCompany();
                var result = ObjectMapper.Map<CompanyInfoDto>(currentCompany);

                if (currentCompany.HasLogo())
                {
                    var logoExtension = _mimeTypeMap.GetExtension(currentCompany.LogoFileType);
                    var logoBinaryObject =
                        AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(currentCompany.LogoId.Value));

                    result.LogoFile = logoBinaryObject != null
                        ? new TempFileInfo(currentCompany.CompanyName + "_logo" + logoExtension, currentCompany.LogoFileType,
                            logoBinaryObject.Bytes)
                        : new TempFileInfo();
                    if (logoBinaryObject != null)
                        _tempFileCacheManager.SetFile(currentCompany.LogoId.Value.ToString(), result.LogoFile);
                }

                if (currentCompany.HasLetterHead())
                {
                    var letterExtension = _mimeTypeMap.GetExtension(currentCompany.LetterHeadFileType);
                    var letterBinaryObject =
                        AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(currentCompany.LetterHeadId.Value));

                    result.LetterHeadFile = letterBinaryObject != null
                        ? new TempFileInfo(currentCompany.CompanyName + "_legal" + letterExtension, currentCompany.LetterHeadFileType,
                            letterBinaryObject.Bytes)
                        : new TempFileInfo();
                    if (letterBinaryObject != null)
                        _tempFileCacheManager.SetFile(currentCompany.LetterHeadId.Value.ToString(), result.LetterHeadFile);
                }

                return result;
            }
            catch (KontecgException)
            {
                return new CompanyInfoDto();
            }
        }

        /// <inheritdoc />
        public async Task<CompanyInfoDto> GetCompanyInfoAsync(int? companyId = null)
        {
            try
            {
                var currentCompany = companyId != null
                    ? await GetCompanyAsync(companyId.Value)
                    : await GetCurrentCompanyAsync();

                var result = ObjectMapper.Map<CompanyInfoDto>(currentCompany);

                if (currentCompany.HasLogo())
                {
                    var logoExtension = _mimeTypeMap.GetExtension(currentCompany.LogoFileType);
                    var logoBinaryObject = await _binaryObjectManager.GetOrNullAsync(currentCompany.LogoId.Value).ConfigureAwait(false);

                    result.LogoFile = logoBinaryObject != null
                        ? new TempFileInfo(currentCompany.CompanyName + "_logo" + logoExtension, currentCompany.LogoFileType,
                            logoBinaryObject.Bytes)
                        : new TempFileInfo();
                    if (logoBinaryObject != null)
                        _tempFileCacheManager.SetFile(currentCompany.LogoId.Value.ToString(), result.LogoFile);
                }

                if (currentCompany.HasLetterHead())
                {
                    var letterExtension = _mimeTypeMap.GetExtension(currentCompany.LetterHeadFileType);
                    var letterBinaryObject = await _binaryObjectManager.GetOrNullAsync(currentCompany.LetterHeadId.Value).ConfigureAwait(false);

                    result.LetterHeadFile = letterBinaryObject != null
                        ? new TempFileInfo(currentCompany.CompanyName + "_legal" + letterExtension, currentCompany.LetterHeadFileType,
                            letterBinaryObject.Bytes)
                        : new TempFileInfo();
                    if (letterBinaryObject != null)
                        _tempFileCacheManager.SetFile(currentCompany.LetterHeadId.Value.ToString(), result.LetterHeadFile);
                }
                return result;
            }
            catch (KontecgException)
            {
                return new CompanyInfoDto();
            }
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingGetAll)]
        public GeneralAccountingInfo GetAllAccountingInfo()
        {
            return new GeneralAccountingInfo
            {
                Company = GetCompanyInfo(),
                AccountingInfo = GetAllBaseData(),
                PersonalAccountingInfo = GetPersonalAccountingInformation(),
                PeriodInfoDto = GetActivePeriodInformation()
            };
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingGetAll)]
        public async Task<GeneralAccountingInfo> GetAllAccountingInfoAsync()
        {
            return new GeneralAccountingInfo
            {
                Company = await GetCompanyInfoAsync(),
                AccountingInfo = await GetAllBaseDataAsync(),
                PersonalAccountingInfo = await GetPersonalAccountingInformationAsync(),
                PeriodInfoDto = await GetActivePeriodInformationAsync()
            };
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingGetBase)]
        public AccountingBaseDataInfoDto GetAllBaseData()
        {
            return new AccountingBaseDataInfoDto
            {
                Accounts = new ListResultDto<AccountDefinitionDto>(_accountingManager.GetAllAccountDefinitions().Select(ObjectMapper.Map<AccountDefinitionDto>).ToList()),
                CenterCosts = new ListResultDto<CenterCostDefinitionDto>(_accountingManager.GetAllCenterCostDefinitions().Select(ObjectMapper.Map<CenterCostDefinitionDto>).ToList()),
                ExpenseItems = new ListResultDto<ExpenseItemDefinitionDto>(_accountingManager.GetAllExpenseItemDefinitions().Select(ObjectMapper.Map<ExpenseItemDefinitionDto>).ToList()),
                Functions = new ListResultDto<AccountingFunctionDefinitionDto>(_accountingManager.GetAllFunctionDefinitions().Select(ObjectMapper.Map<AccountingFunctionDefinitionDto>).ToList()),
                Classifiers = new ListResultDto<AccountingClassifierDefinitionDto>(_accountingManager.GetAllClassifierDefinitions().Select(ObjectMapper.Map<AccountingClassifierDefinitionDto>).ToList()),
                Documents = new ListResultDto<DocumentDefinitionDto>(_accountingManager.GetAllDocumentDefinitions().Select(ObjectMapper.Map<DocumentDefinitionDto>).ToList())
            };
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingGetBase)]
        public async Task<AccountingBaseDataInfoDto> GetAllBaseDataAsync()
        {
            return new AccountingBaseDataInfoDto
            {
                Accounts = new ListResultDto<AccountDefinitionDto>((await _accountingManager.GetAllAccountDefinitionsAsync()).Select(ObjectMapper.Map<AccountDefinitionDto>).ToList()),
                CenterCosts = new ListResultDto<CenterCostDefinitionDto>((await _accountingManager.GetAllCenterCostDefinitionsAsync()).Select(ObjectMapper.Map<CenterCostDefinitionDto>).ToList()),
                ExpenseItems = new ListResultDto<ExpenseItemDefinitionDto>((await _accountingManager.GetAllExpenseItemDefinitionsAsync()).Select(ObjectMapper.Map<ExpenseItemDefinitionDto>).ToList()),
                Functions = new ListResultDto<AccountingFunctionDefinitionDto>((await _accountingManager.GetAllFunctionDefinitionsAsync()).Select(ObjectMapper.Map<AccountingFunctionDefinitionDto>).ToList()),
                Classifiers = new ListResultDto<AccountingClassifierDefinitionDto>((await
                    _accountingManager.GetAllClassifierDefinitionsAsync()).Select(ObjectMapper.Map<AccountingClassifierDefinitionDto>).ToList()),
                Documents = new ListResultDto<DocumentDefinitionDto>((await _accountingManager.GetAllDocumentDefinitionsAsync()).Select(ObjectMapper.Map<DocumentDefinitionDto>).ToList())
            };
        }

        [KontecgAuthorize(PermissionNames.AccountingGetPeriod)]
        public ListResultDto<PeriodInfoDto> GetActivePeriodInformation()
        {
            return new ListResultDto<PeriodInfoDto>(_periodManager.GetCurrentPeriod().Select(ObjectMapper.Map<PeriodInfoDto>).ToList());
        }

        [KontecgAuthorize(PermissionNames.AccountingGetPeriod)]
        public async Task<ListResultDto<PeriodInfoDto>> GetActivePeriodInformationAsync()
        {
            return new ListResultDto<PeriodInfoDto>((await _periodManager.GetCurrentPeriodAsync()).Select(ObjectMapper.Map<PeriodInfoDto>).ToList());
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingListDocuments)]
        public AccountingDocumentListDto GetAllDocumentsByPeriod(PeriodInputDto input)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
                {
                    using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
                    {
                        var documents = _accountingDocumentRepository.GetAllIncluding(
                                                                         d => d.DocumentDefinition, n => n.Period)
                                                                     .Where(d => d.Period.ReferenceGroup == input.ModuleKey)
                                                                     .WhereIf(input.Year.HasValue && input.Month.HasValue,
                                                                         d => d.Period.Year == input.Year &&
                                                                              d.Period.Month == input.Month)
                                                                     .WhereIf(input.Start.HasValue && input.End.HasValue,
                                                                         d => d.Period.Since >= input.Start &&
                                                                              d.Period.Until <= input.End).ToList();

                        var companies = _companyRepository.GetAllList();

                        var query = (from document in documents
                                     join company in companies on document.CompanyId equals company.Id
                                     select new
                                     {
                                         Document = document,
                                         Company = company
                                     }).ToList();

                        return new AccountingDocumentListDto()
                        {
                            TotalCount = query.Count(),
                            Items = query.Select(result =>
                            {
                                var documentDto = ObjectMapper.Map<AccountingDocumentOutputDto>(result.Document);
                                documentDto.Company = ObjectMapper.Map<CompanyInfoDto>(result.Company);
                                return documentDto;
                            }).ToList()
                        };
                    }
                }
            );
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingListDocuments)]
        public async Task<AccountingDocumentListDto> GetAllDocumentsByPeriodAsync(PeriodInputDto input)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
                {
                    var documents = (await _accountingDocumentRepository.GetAllIncludingAsync(
                                        d => d.DocumentDefinition, n => n.Period))
                                    .Where(d => d.Period.ReferenceGroup == input.ModuleKey)
                                    .WhereIf(input.Year.HasValue && input.Month.HasValue,
                                        d => d.Period.Year == input.Year &&
                                             d.Period.Month == input.Month)
                                    .WhereIf(input.Start.HasValue && input.End.HasValue,
                                        d => d.Period.Since >= input.Start &&
                                             d.Period.Until <= input.End).ToList();

                    var companies = await _companyRepository.GetAllListAsync();

                    var query = (from document in documents
                                 join company in companies on document.CompanyId equals company.Id
                                 select new
                                 {
                                     Document = document,
                                     Company = company
                                 }).ToList();

                    return new AccountingDocumentListDto()
                    {
                        TotalCount = query.Count(),
                        Items = query.Select(result =>
                        {
                            var documentDto = ObjectMapper.Map<AccountingDocumentOutputDto>(result.Document);
                            documentDto.Company = ObjectMapper.Map<CompanyInfoDto>(result.Company);
                            return documentDto;
                        }).ToList()
                    };
                }
            });
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingGetPersonal)]
        public PagedResultDto<PersonalAccountingInfoDto> GetPersonalAccountingInformation()
        {
            var accountingInfo = _personalAccountingHelper.CreatePersonalAccountingInfo();
            return new PagedResultDto<PersonalAccountingInfoDto>(
                accountingInfo.Length,
                accountingInfo.Select(ObjectMapper.Map<PersonalAccountingInfoDto>).ToList());
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingGetPersonal)]
        public async Task<PagedResultDto<PersonalAccountingInfoDto>> GetPersonalAccountingInformationAsync()
        {
            var accountingInfo = await _personalAccountingHelper.CreatePersonalAccountingInfoAsync();
            return new PagedResultDto<PersonalAccountingInfoDto>(
                accountingInfo.Length,
                accountingInfo.Select(ObjectMapper.Map<PersonalAccountingInfoDto>).ToList());
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingListVouchers)]
        public AccountingVoucherListDto GetAllVouchersByPeriod(PeriodInputDto input)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
                {
                    var vouchers = _accountingVoucherDocumentRepository.GetAllIncluding(d => d.DocumentDefinition,
                                                                        d => d.Document, d => d.Document.Period)
                                                                    .Where(d =>
                                                                        d.Document.Period.ReferenceGroup ==
                                                                        input.ModuleKey)
                                                                    .WhereIf(input.Year.HasValue && input.Month.HasValue,
                                                                        d => d.Document.Period.Year ==
                                                                             input.Year &&
                                                                             d.Document.Period.Month ==
                                                                             input.Month)
                                                                    .WhereIf(input.Start.HasValue && input.End.HasValue,
                                                                        d => d.Document.Period.Since >=
                                                                             input.Start &&
                                                                             d.Document.Period.Until <=
                                                                             input.End).ToList();

                    var companies = _companyRepository.GetAllList();

                    var query = (from document in vouchers
                                 join company in companies on document.CompanyId equals company.Id
                                 select new
                                 {
                                     Document = document,
                                     Company = company
                                 }).ToList();

                    // Convert to DTO and return
                    return new AccountingVoucherListDto
                    {
                        TotalCount = query.Count(),
                        Items = query.Select(result =>
                        {
                            var documentDto = ObjectMapper.Map<AccountingVoucherOutputDto>(result.Document);
                            documentDto.Company = ObjectMapper.Map<CompanyInfoDto>(result.Company);
                            return documentDto;
                        }).ToList()
                    };
                }
            });
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingListVouchers)]
        public async Task<AccountingVoucherListDto> GetAllVouchersByPeriodAsync(PeriodInputDto input)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
                {
                    var vouchers = (await _accountingVoucherDocumentRepository.GetAllIncludingAsync(
                                       d => d.DocumentDefinition,
                                       d => d.Document,
                                       d => d.Document.Period))
                                   .Where(d => d.Document.Period.ReferenceGroup == input.ModuleKey)
                                   .WhereIf(input.Year.HasValue && input.Month.HasValue,
                                       d => d.Document.Period.Year == input.Year &&
                                            d.Document.Period.Month == input.Month)
                                   .WhereIf(input.Start.HasValue && input.End.HasValue,
                                       d => d.Document.Period.Since >= input.Start &&
                                            d.Document.Period.Until <= input.End).ToList();

                    var companies = await _companyRepository.GetAllListAsync();

                    var query = (from document in vouchers
                                 join company in companies on document.CompanyId equals company.Id
                                 select new
                                 {
                                     Document = document,
                                     Company = company
                                 }).ToList();

                    // Convert to DTO and return
                    return new AccountingVoucherListDto
                    {
                        TotalCount = query.Count(),
                        Items = query.Select(result =>
                        {
                            var documentDto = ObjectMapper.Map<AccountingVoucherOutputDto>(result.Document);
                            documentDto.Company = ObjectMapper.Map<CompanyInfoDto>(result.Company);
                            return documentDto;
                        }).ToList()
                    };
                }
            });
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingListDocuments)]
        public AccountingDocumentOutputDto GetAccountingDocument(AccountingDocumentFilterDto input)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
                {
                    var document = _accountingDocumentRepository.GetAllIncluding(
                                                                    d => d.DocumentDefinition,
                                                                    n => n.Period)
                                                                .FirstOrDefault(d => d.Id == input.DocumentId);

                    return CreateAccountingDocumentDto(document);
                }
            });
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingListDocuments)]
        public async Task<AccountingDocumentOutputDto> GetAccountingDocumentAsync(AccountingDocumentFilterDto input)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
                {
                    var document = await (await _accountingDocumentRepository.GetAllIncludingAsync(
                            d => d.DocumentDefinition,
                            n => n.Period))
                        .FirstOrDefaultAsync(d => d.Id == input.DocumentId);

                    return await CreateAccountingDocumentDtoAsync(document);
                }
            });
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingListVouchers)]
        public AccountingVoucherOutputDto GetAccountingVoucherInfo(AccountingVoucherFilterDto input)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
                {
                    var voucher = _accountingVoucherDocumentRepository.GetAllIncluding(d => d.Document,
                                                                          d => d.Document.DocumentDefinition,
                                                                          d => d.Document.Period,
                                                                          d => d.DocumentDefinition,
                                                                          n => n.Notes)
                                                                      .FirstOrDefault(d =>
                                                                          d.DocumentId == input.DocumentId);

                    return CreateAccountingVoucherDto(voucher);
                }
            });
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingListVouchers)]
        public async Task<AccountingVoucherOutputDto> GetAccountingVoucherInfoAsync(AccountingVoucherFilterDto input)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
                {
                    var voucher = await (await _accountingVoucherDocumentRepository.GetAllIncludingAsync(d => d.Document,
                            d => d.Document.DocumentDefinition,
                            d => d.Document.Period,
                            d => d.DocumentDefinition,
                            n => n.Notes))
                        .FirstOrDefaultAsync(d => d.DocumentId == input.DocumentId);

                    return await CreateAccountingVoucherDtoAsync(voucher);
                }
            });
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingListVouchers)]
        public AccountingVoucherOutputDto GetXmlVoucher(AccountingVoucherFilterDto input)
        {
            return AsyncHelper.RunSync(() => GetXmlVoucherAsync(input));
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingListVouchers)]
        public async Task<AccountingVoucherOutputDto> GetXmlVoucherAsync(AccountingVoucherFilterDto input)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
                {
                    var centerCosts = (await _accountingManager.GetAllCenterCostDefinitionsAsync()).Where(cc => cc.IsActive).Select(cc =>
                        new { Account = cc.AccountDefinition.Account, Value = cc.Code }).ToArray();

                    var voucher = await GetAccountingVoucherInfoAsync(input);

                    using var stream = new MemoryStream();
                    await using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();

                        // Root element
                        writer.WriteStartElement("ComproImportados");

                        // Voucher element
                        writer.WriteStartElement("CompItem");
                        writer.WriteElementString("CompNumero", voucher.Code);
                        writer.WriteElementString("CompAno", voucher.Document.Period.Year.ToString("yy"));
                        writer.WriteElementString("CompMes", voucher.Document.Period.Since.ToString("MM"));
                        writer.WriteElementString("CompSubsi", "EX");
                        writer.WriteElementString("CompDescripcion", voucher.Description);
                        writer.WriteElementString("CompFecha", voucher.MadeOn.ToString("dd/MM/yyyy"));

                        // Partidas element
                        writer.WriteStartElement("Partidas");

                        foreach (var note in voucher.Notes)
                        {
                            var isOc = centerCosts.Any(cc => cc.Account == note.Account && cc.Value == note.SubAccount);

                            var subCuenta = isOc
                                ? note.SubControl.ToString()
                                : note.SubAccount.ToString();

                            var subControl = isOc ? "0" : note.SubControl.ToString();

                            var partOc = isOc ? note.SubAccount.ToString() : "0";

                            // PartidasItem element
                            writer.WriteStartElement("PartidasItem");
                            writer.WriteElementString("PartCuenta", note.Account.ToString());
                            writer.WriteElementString("PartSubCuenta", subCuenta);
                            writer.WriteElementString("PartSubControl", subControl);
                            writer.WriteElementString("PartAnalisis", note.Analysis.ToString());
                            writer.WriteElementString("PartImporte", Math.Abs(note.Amount.Amount).ToString("F2", CultureInfo.InvariantCulture));
                            writer.WriteElementString("PartDebito", note.Operation == AccountOperation.Credit ? "0" : "1");
                            writer.WriteElementString("PartOC", partOc);
                            writer.WriteElementString("PartFijo", "1");
                            writer.WriteElementString("PartCB", "0");
                            writer.WriteEndElement(); // Close PartidasItem
                        }

                        writer.WriteEndElement(); // Close Partidas
                        writer.WriteEndElement(); // Close CompItem
                        writer.WriteEndElement(); // Close ComproImportados

                        writer.WriteEndDocument();
                        writer.Flush();
                    }

                    voucher.XmlFileExported = new TempFileInfo(voucher.Description, ".xml", stream.ToArray());
                    return voucher;
                }
            });
        }

        #endregion

        #region Create methods

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingAccountsCreate)]
        public async Task CreateAccountDefinitionAsync(AccountDefinitionInputDto input)
        {
            if (Enum.TryParse(input.Kind, true, out AccountKind result))
            {
                await _accountingManager.InsertOrUpdateDefinitionAsync(new AccountDefinition(input.Account, input.SubAccount, input.SubControl, input.Analysis, input.Description, result, input.Reference));
            }
            else
                throw new InvalidCastException($"Can't convert input kind value {input.Kind} to AccountKind");
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingAccountsCreate)]
        public void CreateAccountDefinition(AccountDefinitionInputDto input)
        {
            AsyncHelper.RunSync(() => CreateAccountDefinitionAsync(input));
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingCenterCostsCreate)]
        public async Task CreateCenterCostDefinitionAsync(CenterCostDefinitionInputDto input)
        {
            try
            {
                var accountDefinition = (await _accountingManager.GetAllAccountDefinitionsAsync()).SingleOrDefault(acc =>
                    acc.Account == input.Account && acc.SubAccount == 0 && acc.SubControl == 0 && acc.Analysis == 0 && acc.IsActive);

                if (accountDefinition == null)
                    throw new UserFriendlyException(L("AccountNotExists{0}", input.Account.ToString()));

                await _accountingManager.InsertOrUpdateDefinitionAsync(new CenterCostDefinition(accountDefinition, input.Description, input.Code, input.Reference));
            }
            catch (InvalidOperationException)
            {
                throw new UserFriendlyException(L("AccountDuplicated"));
            }
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingCenterCostsCreate)]
        public void CreateCenterCostDefinition(CenterCostDefinitionInputDto input)
        {
            AsyncHelper.RunSync(() => CreateCenterCostDefinitionAsync(input));
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingExpenseItemsCreate)]
        public async Task CreateExpenseItemDefinitionAsync(ExpenseItemDefinitionInputDto input)
        {
            try
            {
                if (input.CenterCost is <= 0)
                    throw new UserFriendlyException(L("CenterCostMustBeGreaterThanZero"));

                CenterCostDefinition centerCostDefinition = input.CenterCost != null ? (await _accountingManager.GetAllCenterCostDefinitionsAsync())
                    .SingleOrDefault(cc => cc.Code == input.CenterCost && cc.IsActive) : null;

                await _accountingManager.InsertOrUpdateDefinitionAsync(
                    new ExpenseItemDefinition(input.Description, input.Code, input.Reference)
                        { CenterCostDefinition = centerCostDefinition });
            }
            catch (InvalidOperationException)
            {
                throw new UserFriendlyException(L("CenterCostDuplicated"));
            }
        }

        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingExpenseItemsCreate)]
        public void CreateExpenseItemDefinition(ExpenseItemDefinitionInputDto input)
        {
            AsyncHelper.RunSync(() => CreateExpenseItemDefinitionAsync(input));
        }

        /// <param name="input"></param>
        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingDocumentsCreate)]
        public async Task<AccountingDocumentOutputDto> CreateDocumentAsync(AccountingDocumentInputDto input)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.GetCompanyId()))
                {
                    var currentPeriod = await _periodManager.GetCurrentPeriodAsync(input.ReferenceGroup);
                    var documentDefinition = await _documentDefinitionRepository.GetByReferenceAsync(input.Reference);
                    var description = !input.Description.IsNullOrWhiteSpace() ? input.Description : $"{documentDefinition.Description} {ObjectMapper.Map<string>(currentPeriod.Month)} {currentPeriod.Year}";
                    var code = "(AUTO)";
                    var document = new AccountingDocument(documentDefinition.Id, description, currentPeriod.Id, input.MadeOn ?? Clock.Now, code);
                    var docId = await _accountingDocumentRepository.InsertAndGetIdAsync(document);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    return await CreateAccountingDocumentDtoAsync(document);
                }
            });
        }

        /// <param name="input"></param>
        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingDocumentsCreate)]
        public AccountingDocumentOutputDto CreateDocument(AccountingDocumentInputDto input)
        {
            return AsyncHelper.RunSync(() => CreateDocumentAsync(input));
        }

        /// <param name="input"></param>
        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingVouchersCreate)]
        public async Task<AccountingVoucherOutputDto> CreateVoucherAsync(AccountingVoucherInputDto input)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.GetCompanyId()))
                {
                    var documentDefinition = await _documentDefinitionRepository.GetByReferenceAsync("COMPROBANTE");
                    var documentBase = await _accountingDocumentRepository.GetAsync(input.DocumentId);
                    var description = !input.Description.IsNullOrWhiteSpace() ? input.Description : $"{documentDefinition.Description} {documentBase.Description}";
                    var code = "AUTO";
                    var document = new AccountingVoucherDocument(documentDefinition.Id, documentBase.Id, description, input.MadeOn ?? Clock.Now, code);

                    var docId = await _accountingVoucherDocumentRepository.InsertAndGetIdAsync(document);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    return await CreateAccountingVoucherDtoAsync(document);
                }
            });
        }

        /// <param name="input"></param>
        /// <inheritdoc />
        [KontecgAuthorize(PermissionNames.AccountingVouchersCreate)]
        public AccountingVoucherOutputDto CreateVoucher(AccountingVoucherInputDto input)
        {
            return AsyncHelper.RunSync(() => CreateVoucherAsync(input));
        }

        #endregion

        #region Private Methods

        private Company GetCompany(int companyId)
        {
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                return CompanyManager.GetById(companyId);
            }
        }

        private async Task<Company> GetCompanyAsync(int companyId)
        {
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                return await CompanyManager.GetByIdAsync(companyId);
            }
        }

        private async Task<AccountingDocumentOutputDto> CreateAccountingDocumentDtoAsync(AccountingDocument document)
        {
            var dto = ObjectMapper.Map<AccountingDocumentOutputDto>(document);
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                dto.Company = await GetCompanyInfoAsync(document.CompanyId);
            }
            return dto;
        }

        private AccountingDocumentOutputDto CreateAccountingDocumentDto(AccountingDocument document)
        {
            var dto = ObjectMapper.Map<AccountingDocumentOutputDto>(document);
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                dto.Company = GetCompanyInfo(document.CompanyId);
            }
            return dto;
        }

        private async Task<AccountingVoucherOutputDto> CreateAccountingVoucherDtoAsync(AccountingVoucherDocument document)
        {
            var dto = ObjectMapper.Map<AccountingVoucherOutputDto>(document);
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                dto.Company = await GetCompanyInfoAsync(document.CompanyId);
            }
            return dto;
        }

        private AccountingVoucherOutputDto CreateAccountingVoucherDto(AccountingVoucherDocument document)
        {
            var dto = ObjectMapper.Map<AccountingVoucherOutputDto>(document);
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                dto.Company = GetCompanyInfo(document.CompanyId);
            }
            return dto;
        }

        #endregion
    }
}
