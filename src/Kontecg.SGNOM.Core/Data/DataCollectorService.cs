using System.Linq;
using System.Threading.Tasks;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Repositories;
using Kontecg.HumanResources;
using Kontecg.MultiCompany;
using Kontecg.Organizations;
using Kontecg.Timing;
using Kontecg.WorkRelations;

namespace Kontecg.Data
{
    public class DataCollectorService : SGNOMDomainServiceBase, IDataCollectorService
    {
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IPersonOrganizationUnitRepository _personOrganizationUnitRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IRepository<PersonScholarship> _personScholarshipRepository;   
        private readonly IRepository<Company> _companyRepository;

        public DataCollectorService(
            IEmploymentRepository employmentRepository,
            IPersonOrganizationUnitRepository personOrganizationUnitRepository,
            IPersonRepository personRepository,
            IRepository<Company> companyRepository, 
            IRepository<PersonScholarship> personScholarshipRepository)
        {
            _personOrganizationUnitRepository = personOrganizationUnitRepository;
            _companyRepository = companyRepository;
            _personScholarshipRepository = personScholarshipRepository;
            _personRepository = personRepository;
            _employmentRepository = employmentRepository;
        }

        public async Task ForcePersonToChangeTheirOrganizationUnitAsync(int companyId)
        {
            using (CurrentUnitOfWork.SetCompanyId(companyId))
            {
                var creationTime = Clock.Now.AddDays(-1).ToUniversalTime();

                var relationship = (await _employmentRepository.CurrentRelationshipAsync(new FilterRequest { MaxResultCount = int.MaxValue }))
                    .Where(c => c.CreationTime >= creationTime || c.LastModificationTime >= creationTime).ToList();

                var separationCount = 500;
                var separationLoopCount = relationship.Count / separationCount + 1;

                for (int i = 0; i < separationLoopCount; i++)
                {
                    var personIdsToUpdate = relationship.Skip(i * separationCount).Take(separationCount)
                        .Select(e => new {e.CompanyId, e.PersonId, e.OrganizationUnitId}).ToList();

                    if (personIdsToUpdate.Count > 0)
                    {
                        for (int j = 0; j < personIdsToUpdate.Count; j++)
                        {
                            var personOrganizationUnit = await _personOrganizationUnitRepository.FirstOrDefaultAsync(e =>
                                e.PersonId == personIdsToUpdate[j].PersonId);

                            if (personOrganizationUnit != null && personOrganizationUnit.OrganizationUnitId != personIdsToUpdate[j].OrganizationUnitId)
                            {
                                personOrganizationUnit.OrganizationUnitId = personIdsToUpdate[j].OrganizationUnitId;
                                await _personOrganizationUnitRepository.UpdateAsync(personOrganizationUnit);
                            }
                            else if (personOrganizationUnit == null)
                            {
                                await _personOrganizationUnitRepository.InsertAsync(
                                    new PersonOrganizationUnit(personIdsToUpdate[j].CompanyId, personIdsToUpdate[j].PersonId,
                                        personIdsToUpdate[j].OrganizationUnitId));
                            }
                        }
                    }
                }
            }
        }

        public async Task ForcePersonToChangeTheirScholarshipDataAsync(int companyId)
        {
            using (CurrentUnitOfWork.SetCompanyId(companyId))
            {
                var creationTime = Clock.Now.AddDays(-1).ToUniversalTime();

                var relationship = (await _employmentRepository.CurrentRelationshipAsync(new FilterRequest { MaxResultCount = int.MaxValue }))
                    .Where(c => c.CreationTime >= creationTime || c.LastModificationTime >= creationTime).ToList();

                var separationCount = 500;
                var separationLoopCount = relationship.Count / separationCount + 1;

                for (int i = 0; i < separationLoopCount; i++)
                {
                    var personIdsToUpdate = relationship.Skip(i * separationCount).Take(separationCount)
                        .Select(e => new { e.CompanyId, e.PersonId, e.Exp })
                        .ToList();

                    if (personIdsToUpdate.Count > 0)
                    {
                        for (int j = 0; j < personIdsToUpdate.Count; j++)
                        {
                            var person = await _personRepository.FirstOrDefaultAsync(e =>
                                e.Id == personIdsToUpdate[j].PersonId);

                            if (person != null)
                            {
                                var scholarship = (await _personScholarshipRepository.GetAllIncludingAsync(
                                        s => s.ScholarshipLevel,
                                        s => s.Degree)).OrderByDescending(o => o.ScholarshipLevel.Weight)
                                                       .FirstOrDefault(p => p.PersonId == person.Id);

                                person.ScholarshipLevel = scholarship?.ScholarshipLevel.DisplayName;
                                person.Scholarship = scholarship?.Degree.DisplayName;
                                await _personRepository.UpdateAsync(person);
                            }
                        }
                    }
                }
            }
        }

        public async Task ForcePersonToChangeTheirExpNumberAsync(int companyId)
        {
            using (CurrentUnitOfWork.SetCompanyId(companyId))
            {
                var creationTime = Clock.Now.AddDays(-1).ToUniversalTime();

                var relationship = (await _employmentRepository.CurrentRelationshipAsync(new FilterRequest { MaxResultCount = int.MaxValue }))
                                   .Where(c => c.CreationTime >= creationTime || c.LastModificationTime >= creationTime).ToList();

                var separationCount = 500;
                var separationLoopCount = relationship.Count / separationCount + 1;

                for (int i = 0; i < separationLoopCount; i++)
                {
                    var personIdsToUpdate = relationship.Skip(i * separationCount).Take(separationCount)
                                                        .Select(e => new { e.CompanyId, e.PersonId, e.Exp })
                                                        .ToList();

                    if (personIdsToUpdate.Count > 0)
                    {
                        for (int j = 0; j < personIdsToUpdate.Count; j++)
                        {
                            var person = await _personRepository.FirstOrDefaultAsync(e =>
                                e.Id == personIdsToUpdate[j].PersonId);

                            if (person != null)
                            {
                                person.SetData("E", new { personIdsToUpdate[j].Exp, personIdsToUpdate[j].CompanyId });
                                await _personRepository.UpdateAsync(person);
                            }
                        }
                    }
                }
            }
        }

        public async Task ForcePersonToChangeTheirDataAsync()
        {
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                // check companies
                var companyIds = (await _companyRepository.GetAllListAsync()).Select(company => company.Id).ToList();
                foreach (var companyId in companyIds)
                {
                    await ForcePersonToChangeTheirExpNumberAsync(companyId);
                    await ForcePersonToChangeTheirOrganizationUnitAsync(companyId);
                    await ForcePersonToChangeTheirScholarshipDataAsync(companyId);
                }
            }
        }
    }
}
