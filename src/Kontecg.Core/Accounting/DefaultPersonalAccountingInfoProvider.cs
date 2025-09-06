using Kontecg.Domain.Uow;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.HumanResources;

namespace Kontecg.Accounting
{
    internal class DefaultPersonalAccountingInfoProvider : IPersonalAccountingInfoProvider
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public DefaultPersonalAccountingInfoProvider(
            IPersonRepository personRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _personRepository = personRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        public PersonalAccountingInfo[] GetPersonalAccountingInformation(int? companyId)
        {
            PersonalAccountingInfo[] accountingInfos;
            using var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using (_currentUnitOfWorkProvider.Current.SetCompanyId(companyId))
            {
                var persons = _personRepository.GetAllList();
                var personsWithAccounts = persons.Select(person => new PersonalAccountingInfo {Person = person});
                accountingInfos = personsWithAccounts.ToArray();
            }
            uow.Complete();
            return accountingInfos;
        }

        public async Task<PersonalAccountingInfo[]> GetPersonalAccountingInformationAsync(int? companyId)
        {
            PersonalAccountingInfo[] accountingInfos;
            using var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using (_currentUnitOfWorkProvider.Current.SetCompanyId(companyId))
            {
                var persons = await _personRepository.GetAllListAsync();
                var personsWithAccounts = persons.Select(person => new PersonalAccountingInfo { Person = person });
                accountingInfos = personsWithAccounts.ToArray();
            }
            await uow.CompleteAsync();
            return accountingInfos;
        }
    }
}
