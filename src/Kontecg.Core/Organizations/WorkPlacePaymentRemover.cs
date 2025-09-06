using System.Linq;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;

namespace Kontecg.Organizations
{
    public class WorkPlacePaymentRemover :
        IEventHandler<EntityDeletedEventData<WorkPlacePayment>>,
        ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<WorkPlaceUnit, long> _workPlaceUnitRepository;

        public WorkPlacePaymentRemover(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<WorkPlaceUnit, long> workPlaceUnitRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _workPlaceUnitRepository = workPlaceUnitRepository;
        }

        public virtual void HandleEvent(EntityDeletedEventData<WorkPlacePayment> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var placeUnits = _workPlaceUnitRepository
                    .GetAllIncluding(wp => wp.WorkPlacePayment)
                    .Where(p => p.WorkPlacePayment == eventData.Entity).ToList();

                placeUnits.ForEach(p => p.WorkPlacePayment = null);

                _unitOfWorkManager.Current.SaveChanges();
            });
        }
    }
}
