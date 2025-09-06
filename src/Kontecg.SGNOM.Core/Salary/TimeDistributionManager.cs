using Kontecg.Domain.Repositories;
using Kontecg.WorkRelations;

namespace Kontecg.Salary
{
    public class TimeDistributionManager : SGNOMDomainServiceBase
    {
        private readonly IRepository<TimeDistributionDocument> _documentRepository;
        private readonly IEmploymentRepository _employmentRepository;

        /// <inheritdoc />
        public TimeDistributionManager(
            IRepository<TimeDistributionDocument> documentRepository, 
            IEmploymentRepository employmentRepository)
        {
            _documentRepository = documentRepository;
            _employmentRepository = employmentRepository;
        }
    }
}