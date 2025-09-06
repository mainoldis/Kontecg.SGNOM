using System.Threading.Tasks;
using Kontecg.Accounting.Dto;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.MultiCompany.Dto;
using Kontecg.Timing.Dto;

namespace Kontecg.Accounting
{
    /// <summary>  
    /// Interface for accounting application services.  
    /// Provides methods for managing accounting-related operations.  
    /// </summary>  
    public interface IAccountingAppService : IApplicationService
    {
        CompanyInfoDto GetCompanyInfo(int? companyId = null);

        Task<CompanyInfoDto> GetCompanyInfoAsync(int? companyId = null);

        /// <summary>  
        /// Retrieves general accounting information, including company details and personal accounting data.  
        /// </summary>  
        /// <returns>A <see cref="GeneralAccountingInfo"/> object containing company and personal accounting details.</returns>  
        GeneralAccountingInfo GetAllAccountingInfo();

        /// <summary>  
        /// Asynchronously retrieves general accounting information, including company details and personal accounting data.  
        /// </summary>  
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GeneralAccountingInfo"/> object.</returns>  
        Task<GeneralAccountingInfo> GetAllAccountingInfoAsync();

        AccountingBaseDataInfoDto GetAllBaseData();

        Task<AccountingBaseDataInfoDto> GetAllBaseDataAsync();

        ListResultDto<PeriodInfoDto> GetActivePeriodInformation();

        Task<ListResultDto<PeriodInfoDto>> GetActivePeriodInformationAsync();

        /// <summary>  
        /// Retrieves all accounting documents for a specified period, filtered by year, month, or date range.  
        /// </summary>  
        /// <param name="input">The period input details, including year, month, and date range.</param>  
        /// <returns>A <see cref="AccountingDocumentListDto"/> containing the list of documents for the specified period.</returns>  
        AccountingDocumentListDto GetAllDocumentsByPeriod(PeriodInputDto input);

        /// <summary>  
        /// Asynchronously retrieves all accounting documents for a specified period, filtered by year, month, or date range.  
        /// </summary>  
        /// <param name="input">The period input details, including year, month, and date range.</param>  
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="AccountingDocumentListDto"/>.</returns>  
        Task<AccountingDocumentListDto> GetAllDocumentsByPeriodAsync(PeriodInputDto input);

        /// <summary>  
        /// Retrieves personal accounting information, including details about individual financial transactions and balances.  
        /// </summary>  
        /// <returns>A paged result containing personal accounting information.</returns>  
        PagedResultDto<PersonalAccountingInfoDto> GetPersonalAccountingInformation();

        /// <summary>  
        /// Asynchronously retrieves personal accounting information, including details about individual financial transactions and balances.  
        /// </summary>  
        /// <returns>A task that represents the asynchronous operation. The task result contains a paged result of personal accounting information.</returns>  
        Task<PagedResultDto<PersonalAccountingInfoDto>> GetPersonalAccountingInformationAsync();

        /// <summary>  
        /// Retrieves all accounting vouchers for a specified period, filtered by year, month, or date range.  
        /// </summary>  
        /// <param name="input">The period input details, including year, month, and date range.</param>  
        /// <returns>A <see cref="AccountingVoucherListDto"/> containing the list of vouchers for the specified period.</returns>  
        AccountingVoucherListDto GetAllVouchersByPeriod(PeriodInputDto input);

        /// <summary>  
        /// Asynchronously retrieves all accounting vouchers for a specified period, filtered by year, month, or date range.  
        /// </summary>  
        /// <param name="input">The period input details, including year, month, and date range.</param>  
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="AccountingVoucherListDto"/>.</returns>  
        Task<AccountingVoucherListDto> GetAllVouchersByPeriodAsync(PeriodInputDto input);

        /// <summary>  
        /// Retrieves a specific accounting document by its unique identifier, including its associated metadata.  
        /// </summary>  
        /// <param name="input">The document input details, including the document ID.</param>  
        /// <returns>A <see cref="AccountingDocumentOutputDto"/> containing the document details and metadata.</returns>  
        AccountingDocumentOutputDto GetAccountingDocument(AccountingDocumentFilterDto input);

        /// <summary>  
        /// Asynchronously retrieves a specific accounting document by its unique identifier, including its associated metadata.  
        /// </summary>  
        /// <param name="input">The document input details, including the document ID.</param>  
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="AccountingDocumentOutputDto"/>.</returns>  
        Task<AccountingDocumentOutputDto> GetAccountingDocumentAsync(AccountingDocumentFilterDto input);

        /// <summary>  
        /// Retrieves accounting voucher information by its unique identifier, including associated notes and metadata.  
        /// </summary>  
        /// <param name="input">The voucher input details, including the voucher ID.</param>  
        /// <returns>A <see cref="AccountingVoucherOutputDto"/> containing the voucher details, notes, and metadata.</returns>  
        AccountingVoucherOutputDto GetAccountingVoucherInfo(AccountingVoucherFilterDto input);

        /// <summary>  
        /// Asynchronously retrieves accounting voucher information by its unique identifier, including associated notes and metadata.  
        /// </summary>  
        /// <param name="input">The voucher input details, including the voucher ID.</param>  
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="AccountingVoucherOutputDto"/>.</returns>  
        Task<AccountingVoucherOutputDto> GetAccountingVoucherInfoAsync(AccountingVoucherFilterDto input);

        /// <summary>  
        /// Generates an XML file for a specific accounting voucher, including all relevant details and metadata.  
        /// </summary>  
        /// <param name="input">The voucher input details, including the voucher ID.</param>
        /// <returns>A <see cref="AccountingVoucherOutputDto"/> containing the voucher details, notes, metadata and the XML file information and download link.</returns>
        AccountingVoucherOutputDto GetXmlVoucher(AccountingVoucherFilterDto input);

        /// <summary>  
        /// Asynchronously generates an XML file for a specific accounting voucher, including all relevant details and metadata.  
        /// </summary>  
        /// <param name="input">The voucher input details, including the voucher ID.</param>  
        /// <returns>A <see cref="AccountingVoucherOutputDto"/> containing the voucher details, notes, metadata and the XML file information and download link.</returns>
        Task<AccountingVoucherOutputDto> GetXmlVoucherAsync(AccountingVoucherFilterDto input);

        Task CreateAccountDefinitionAsync(AccountDefinitionInputDto input);

        void CreateAccountDefinition(AccountDefinitionInputDto input);

        Task CreateCenterCostDefinitionAsync(CenterCostDefinitionInputDto input);

        void CreateCenterCostDefinition(CenterCostDefinitionInputDto input);

        Task CreateExpenseItemDefinitionAsync(ExpenseItemDefinitionInputDto input);

        void CreateExpenseItemDefinition(ExpenseItemDefinitionInputDto input);

        Task<AccountingDocumentOutputDto> CreateDocumentAsync(AccountingDocumentInputDto input);

        AccountingDocumentOutputDto CreateDocument(AccountingDocumentInputDto input);

        Task<AccountingVoucherOutputDto> CreateVoucherAsync(AccountingVoucherInputDto input);

        AccountingVoucherOutputDto CreateVoucher(AccountingVoucherInputDto input);
    }
}
