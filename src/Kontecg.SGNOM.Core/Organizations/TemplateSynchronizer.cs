using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Timing;

namespace Kontecg.Organizations
{
    public class TemplateSynchronizer :
        IEventHandler<EntityCreatedEventData<Template>>,
        IEventHandler<EntityUpdatedEventData<Template>>,
        IEventHandler<EntityDeletingEventData<Template>>,
        IEventHandler<EntityDeletedEventData<Template>>,
        IEventHandler<EntityUpdatedEventData<WorkPlaceUnit>>,
        ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ITemplateJobPositionRepository _templateJobPositionRepository;
        private readonly IWorkShiftRepository _workShiftRepository;

        public TemplateSynchronizer(
            IUnitOfWorkManager unitOfWorkManager,
            ITemplateJobPositionRepository templateJobPositionRepository,
            IWorkShiftRepository workShiftRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _templateJobPositionRepository = templateJobPositionRepository;
            _workShiftRepository = workShiftRepository;
        }

        public void HandleEvent(EntityCreatedEventData<Template> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                    if (eventData.Entity is {Approved: > 0})
                    {
                        for (int i = 1; i <= eventData.Entity.Approved; i++)
                        {
                            var newJobPosition = TemplateJobPosition.CreateFromTemplate(eventData.Entity);
                            newJobPosition.Code = i.ToString(new string('0', TemplateJobPosition.MaxCodeLength));
                            var workShift = _workShiftRepository.GetWorkShiftByName("N");
                            newJobPosition.WorkShift = workShift;
                            _templateJobPositionRepository.Insert(newJobPosition);
                        }
                        _unitOfWorkManager.Current.SaveChanges();
                    }
                    _templateJobPositionRepository.UpdateCodes();
                }
            });
        }

        public void HandleEvent(EntityUpdatedEventData<Template> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                    var approvedSaved = _templateJobPositionRepository.Count(jp => jp.TemplateId == eventData.Entity.Id);

                    if (eventData.Entity is { Approved: >= 0 })
                    {
                        if (eventData.Entity.Approved < approvedSaved)
                        {
                            //RULE: Eliminar las sobrantes que no estén ocupadas, si el número de ocupadas es mayor no podemos hacer nada
                            var removedCount = _templateJobPositionRepository.TryToRemoveOrphans(eventData.Entity.Id,
                                approvedSaved - eventData.Entity.Approved.Value);
                        }

                        if (eventData.Entity.Approved > approvedSaved)
                        {
                            //RULE: Agregar las nuevas plazas las sobrantes el turno se debe corregir por el sistema
                            for (int i = approvedSaved + 1; i <= eventData.Entity.Approved; i++)
                            {
                                var newJobPosition = TemplateJobPosition.CreateFromTemplate(eventData.Entity);
                                newJobPosition.Code = i.ToString(new string('0', TemplateJobPosition.MaxCodeLength));
                                var workShift = _workShiftRepository.GetWorkShiftByName("N");
                                newJobPosition.WorkShift = workShift;
                                _templateJobPositionRepository.Insert(newJobPosition);
                            }
                        }

                        _unitOfWorkManager.Current.SaveChanges();
                    }

                    _templateJobPositionRepository.UpdateCodes();
                }
            });
        }

        public void HandleEvent(EntityDeletingEventData<Template> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                    _templateJobPositionRepository.Delete(template => template.TemplateId == eventData.Entity.Id);
                }
            });
        }

        public void HandleEvent(EntityDeletedEventData<Template> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                    _templateJobPositionRepository.UpdateCodes();
                }
            });
        }

        public void HandleEvent(EntityUpdatedEventData<WorkPlaceUnit> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {

                }
            });
        }
    }
}
