using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.HumanResources;
using Kontecg.Linq;
using Kontecg.MultiCompany;
using Kontecg.Salary;
using Kontecg.WorkRelations;

namespace Kontecg.Accounting
{
    /// <summary>
    ///     Implementation of <see cref="IPersonalAccountingInfoProvider" />.
    /// </summary>
    public class PersonalAccountingInfoProvider : IPersonalAccountingInfoProvider, ITransientDependency
    {
        private readonly IPersonRepository _personRepository;
        private readonly IRepository<PersonAccount, long> _personalAccountRepository;
        private readonly IRepository<CenterCostDefinition> _centerCostDefinitionRepository;
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public PersonalAccountingInfoProvider(
            IPersonRepository personRepository,
            IRepository<PersonAccount, long> personalAccountRepository,
            IRepository<CenterCostDefinition> centerCostDefinitionRepository,
            IEmploymentRepository employmentRepository,
            IRepository<Company> companyRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _companyRepository = companyRepository;
            _personRepository = personRepository;
            _personalAccountRepository = personalAccountRepository;
            _centerCostDefinitionRepository = centerCostDefinitionRepository;
            _employmentRepository = employmentRepository;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public PersonalAccountingInfo[] GetPersonalAccountingInformation(int? companyId)
        {
            // RULE: Los datos contables deben partir desde las cuentas personales y sin filtro de empresa?
            using var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using (_currentUnitOfWorkProvider.Current.SetCompanyId(companyId))
            {
                var persons = _personRepository.GetAllList();
                var relationship = _employmentRepository.CurrentRelationship();
                var personalAccounts = _personalAccountRepository.GetAllList().Where(a => a.IsActive).ToList();
                var centerCostDefinitions = _centerCostDefinitionRepository.GetAllIncluding(cc => cc.AccountDefinition)
                    .Where(cc => cc.IsActive).ToList();
                var companies = _companyRepository.GetAllList().Where(c => c.IsActive).ToList();

                var accountingInfos = (from person in persons
                                       join account in personalAccounts on person.Id equals account.PersonId into personAccounts
                                       from account in personAccounts.DefaultIfEmpty()
                                       join relation in relationship on person.Id equals relation.PersonId into personRelations
                                       from relation in personRelations.DefaultIfEmpty()
                                       join centerCost in centerCostDefinitions on relation?.CenterCost equals centerCost.Code into relationCenterCosts
                                       from centerCost in relationCenterCosts.DefaultIfEmpty()
                                       join company in companies on relation?.CompanyId equals company.Id into relationCompanies
                                       from company in relationCompanies.DefaultIfEmpty()
                                       select new PersonalAccountingInfo
                                       {
                                           Person = person,
                                           PayablePerATM = account?.IsActive ?? false,
                                           BankAccount = account?.AccountNumber,
                                           Currency = account?.Currency,
                                           Company = company,
                                           Exp = relation?.Exp,
                                           IsContract = relation != null ? relation.Contract == ContractType.D : null,
                                           PayablePerRate = relation != null ? relation.EmployeeSalaryForm != EmployeeSalaryForm.Average : null,
                                           CenterCost = centerCost?.Code,
                                           Account = centerCost?.AccountDefinition.Account,
                                           DocumentGroup = relation?.GroupId,
                                           LastDocumentId = relation?.Type == EmploymentType.B ? relation.PreviousId : relation?.Id,
                                           DocumentId = relation?.Id,
                                           Type = relation?.EmployeeSalaryForm.ToString()
                                       }).ToArray();

                uow.Complete();
                return accountingInfos;
            }
        }

        public async Task<PersonalAccountingInfo[]> GetPersonalAccountingInformationAsync(int? companyId)
        {
            // RULE: Los datos contables deben partir desde las cuentas personales y sin filtro de empresa?
            using var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using (_currentUnitOfWorkProvider.Current.SetCompanyId(companyId))
            {
                var persons = await _personRepository.GetAllListAsync();
                var relationship = await _employmentRepository.CurrentRelationshipAsync();
                var personalAccounts =
                    await AsyncQueryableExecuter.ToListAsync(
                        (await _personalAccountRepository.GetAllAsync()).Where(a => a.IsActive));
                var centerCostDefinitions = await AsyncQueryableExecuter.ToListAsync(
                    (await _centerCostDefinitionRepository.GetAllIncludingAsync(cc => cc.AccountDefinition))
                    .Where(cc => cc.IsActive));
                var companies = await AsyncQueryableExecuter.ToListAsync((await _companyRepository.GetAllAsync()).Where(a => a.IsActive));

                var accountingInfos = (from person in persons
                                       join account in personalAccounts on person.Id equals account.PersonId into personAccounts
                                       from account in personAccounts.DefaultIfEmpty()
                                       join relation in relationship on person.Id equals relation.PersonId into personRelations
                                       from relation in personRelations.DefaultIfEmpty()
                                       join centerCost in centerCostDefinitions on relation?.CenterCost equals centerCost.Code into relationCenterCosts
                                       from centerCost in relationCenterCosts.DefaultIfEmpty()
                                       join company in companies on relation?.CompanyId equals company.Id into relationCompanies
                                       from company in relationCompanies.DefaultIfEmpty()
                                       select new PersonalAccountingInfo
                                       {
                                           Person = person,
                                           PayablePerATM = account?.IsActive ?? false,
                                           BankAccount = account?.AccountNumber,
                                           Currency = account?.Currency,
                                           Company = company,
                                           Exp = relation?.Exp,
                                           IsContract = relation != null ? relation.Contract == ContractType.D : null,
                                           PayablePerRate = relation != null ? relation.EmployeeSalaryForm != EmployeeSalaryForm.Average : null,
                                           CenterCost = centerCost?.Code,
                                           Account = centerCost?.AccountDefinition.Account,
                                           DocumentGroup = relation?.GroupId,
                                           LastDocumentId = relation?.Type == EmploymentType.B ? relation.PreviousId : relation?.Id,
                                           DocumentId = relation?.Id,
                                           Type = relation?.EmployeeSalaryForm.ToString()
                                       }).ToArray();

                await uow.CompleteAsync();
                return accountingInfos;
            }
        }
    }
}
