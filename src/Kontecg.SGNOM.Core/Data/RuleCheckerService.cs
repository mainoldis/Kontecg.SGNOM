using Kontecg.Domain.Repositories;
using Kontecg.HumanResources;
using Kontecg.WorkRelations;

namespace Kontecg.Data
{
    public class RuleCheckerService : SGNOMDomainServiceBase, IRuleChecker
    {
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IRepository<EmploymentPlus, long> _employmentPlusRepository;
        private readonly IRepository<PersonAccount, long> _personAccountRepository;
        private readonly IRepository<PersonBackgroundTime> _personBackgroundTimeRepository;
        private readonly IRepository<PersonDriverLicense> _personDriverLicenseRepository;
        private readonly IRepository<PersonFamilyRelationship> _personFamilyRelationshipRepository;
        private readonly IRepository<PersonHolidaySchedule> _personHolidayScheduleRepository;
        private readonly IRepository<PersonLawPenalty> _personLawPenaltyRepository;
        private readonly IRepository<PersonMilitaryRecord> _personMilitaryRecordRepository;
        private readonly IRepository<PersonScholarship> _personScholarshipRepository;

        public RuleCheckerService()
        {
        }
    }
}
