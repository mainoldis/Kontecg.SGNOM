using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.Domain.Uow;
using Kontecg.HumanResources;
using Kontecg.Runtime.Session;

namespace Kontecg.WorkRelations
{
    internal class DefaultWorkRelationshipProvider : IWorkRelationshipProvider
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public DefaultWorkRelationshipProvider(
            IPersonRepository personRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _personRepository = personRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;

            KontecgSession = NullKontecgSession.Instance;
        }

        public IKontecgSession KontecgSession { get; set; }

        public WorkRelationship[] GetWorkRelationshipInformation()
        {
            WorkRelationship[] workRelationshipInformation;
            using var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using (_currentUnitOfWorkProvider.Current.SetCompanyId(KontecgSession.GetCompanyId()))
            {
                var persons = _personRepository.GetAllList();

                var grouping = persons.Select(p => new WorkRelationship
                {
                    PersonId = p.Id,
                    Person = p
                });

                workRelationshipInformation = grouping.ToArray();
            }
            uow.Complete();
            return workRelationshipInformation;
        }

        public async Task<WorkRelationship[]> GetWorkRelationshipInformationAsync()
        {
            WorkRelationship[] workRelationshipInformation;
            using var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using (_currentUnitOfWorkProvider.Current.SetCompanyId(KontecgSession.GetCompanyId()))
            {
                var persons = await _personRepository.GetAllListAsync();

                var grouping = persons.Select(p => new WorkRelationship
                {
                    PersonId = p.Id,
                    Person = p
                });

                workRelationshipInformation = grouping.ToArray();
            }
            await uow.CompleteAsync();
            return workRelationshipInformation;
        }
    }
}
