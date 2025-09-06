using System.Linq;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Events.Bus.Entities;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Identity;
using Kontecg.Organizations;
using Kontecg.Workflows;
using LinqKit;

namespace Kontecg.WorkRelations
{
    public class EmploymentSynchronizer :
        IEventHandler<EntityCreatedEventData<EmploymentDocument>>,
        IEventHandler<EntityChangedEventData<EmploymentDocument>>,
        IEventHandler<EntityDeletedEventData<EmploymentDocument>>,
        IEventHandler<EntityChangingEventData<EmploymentDocumentToGenerate>>,
        ITransientDependency
    {
        private readonly ITemplateJobPositionRepository _jobPositionRepository;
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IEmploymentIndexRepository _employmentIndexRepository;
        private readonly IEmploymentDocumentGenerator _employmentDocumentGenerator;
        private readonly IRepository<SignOnDocument> _signOnRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public EmploymentSynchronizer(
            IUnitOfWorkManager unitOfWorkManager,
            IEmploymentIndexRepository employmentIndexRepository,
            ITemplateJobPositionRepository jobPositionRepository, 
            IEmploymentRepository employmentRepository, 
            IEmploymentDocumentGenerator employmentDocumentGenerator)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _jobPositionRepository = jobPositionRepository;
            _employmentRepository = employmentRepository;
            _employmentDocumentGenerator = employmentDocumentGenerator;
            _employmentIndexRepository = employmentIndexRepository;
        }

        public void HandleEvent(EntityCreatedEventData<EmploymentDocument> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                }
            });
        }

        public void HandleEvent(EntityChangedEventData<EmploymentDocument> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                }
            });
        }

        public void HandleEvent(EntityDeletedEventData<EmploymentDocument> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                    var jobPositions = _jobPositionRepository.GetAll()
                        .Where(position => position.DocumentId == eventData.Entity.Id);

                    jobPositions.ForEach(p => p.Document = null);
                    
                }
            });
        }

        /// <inheritdoc />
        public void HandleEvent(EntityChangingEventData<EmploymentDocumentToGenerate> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(eventData.Entity.CompanyId))
                {
                    if (eventData.Entity.Confirmed && eventData.Entity.NextEmploymentDocumentId == null)
                    {
                        //Crea una copia del movimiento de nómina
                        var idLastMov = eventData.Entity.EmploymentDocumentId;
                        var documentToClone = _employmentRepository.GetQueryableEmploymentDocument().Single(d => d.Id == idLastMov);
                        var documentNew = _employmentDocumentGenerator.Clone(documentToClone, eventData.Entity);
                        var documentCloned = _employmentRepository.Insert(documentNew);
                        eventData.Entity.NextEmploymentDocument = documentCloned;
                    }

                    if (!eventData.Entity.Confirmed && eventData.Entity.NextEmploymentDocumentId != null)
                    {
                        var idLastMov = eventData.Entity.NextEmploymentDocumentId;
                        var documentToDelete = _employmentRepository.GetQueryableEmploymentDocument().FirstOrDefault(d => d.Id == idLastMov && d.Review != ReviewStatus.Confirmed);
                        if (documentToDelete != null) _employmentRepository.Delete(documentToDelete);
                    }
                }
            });
        }
    }
}
