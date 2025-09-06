using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Identity;

namespace Kontecg.HumanResources
{
    /// <summary>
    ///     Removes the person from all entities when a person is deleted.
    /// </summary>
    public class PersonRemover :
        IEventHandler<EntityDeletedEventData<Person>>,
        ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<PersonOrganizationUnit, long> _personOrganizationUnitRepository;
        private readonly IRepository<Sign> _signRepository;

        public PersonRemover(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<PersonOrganizationUnit, long> personOrganizationUnitRepository,
            IRepository<Sign> signRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _personOrganizationUnitRepository = personOrganizationUnitRepository;
            _signRepository = signRepository;
        }

        public virtual void HandleEvent(EntityDeletedEventData<Person> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _personOrganizationUnitRepository.Delete(uou => uou.PersonId == eventData.Entity.Id);
                _signRepository.Delete(s => s.IdentityCard == eventData.Entity.IdentityCard);
            });
        }
    }
}
