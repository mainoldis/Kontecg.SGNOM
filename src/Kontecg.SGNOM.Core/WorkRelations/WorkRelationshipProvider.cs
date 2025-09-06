using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.HumanResources;
using Kontecg.Linq;
using Kontecg.MultiCompany;
using Kontecg.Organizations;
using Kontecg.Runtime.Session;
using NMoneys;

namespace Kontecg.WorkRelations
{
    public class WorkRelationshipProvider : IWorkRelationshipProvider, ITransientDependency
    {
        private readonly IPersonRepository _personRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<WorkPlaceUnit, long> _workPlaceUnitRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly IEmploymentRepository _employmentRepository;

        public WorkRelationshipProvider(
            IPersonRepository personRepository,
            IEmploymentRepository employmentRepository,
            IRepository<Company> companyRepository,
            IRepository<WorkPlaceUnit, long> workPlaceUnitRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _personRepository = personRepository;
            _employmentRepository = employmentRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _companyRepository = companyRepository;
            _workPlaceUnitRepository = workPlaceUnitRepository;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            KontecgSession = NullKontecgSession.Instance;
        }

        public IKontecgSession KontecgSession { get; set; }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public WorkRelationship[] GetWorkRelationshipInformation()
        {
            WorkRelationship[] workRelationshipInformation;
            using var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using (_currentUnitOfWorkProvider.Current.SetCompanyId(KontecgSession.GetCompanyId()))
            {
                var persons = _personRepository.GetAllList();
                var companies = _companyRepository.GetAllList();

                var workplaces = _workPlaceUnitRepository.GetAllIncluding(
                        w => w.Classification,
                        w => w.WorkPlacePayment,
                        w => w.Parent)
                    .Where(w => w.Classification.Level == 3 ||
                               w.DisplayName == KontecgCompanyBase.DefaultCompanyName)
                    .ToList();

                var relationships = _employmentRepository.CurrentRelationship();

                workRelationshipInformation = (
                    from r in relationships
                    join w in workplaces on r.OrganizationUnitId equals w.Id
                    join c in companies on r.CompanyId equals c.Id
                    join p in persons on r.PersonId equals p.Id into personsGroup
                    from p in personsGroup.DefaultIfEmpty()
                    select new WorkRelationship
                    {
                        CompanyId = r.CompanyId,
                        Company = c,
                        PersonId = r.PersonId,
                        Person = p,
                        Exp = r.Exp,
                        OrganizationUnitId = r.OrganizationUnitId,
                        WorkPlaceUnit = w,
                        CenterCost = r.CenterCost,
                        OccupationCode = r.OccupationCode,
                        OccupationDescription = r.FullOccupationDescription,
                        OccupationCategory = r.OccupationCategory,
                        ComplexityGroup = r.ComplexityGroup,
                        LastDocumentId = r.Id,
                        OccupationResponsibility = r.Responsibility,
                        Code = r.Code,
                        MadeOn = r.MadeOn,
                        EffectiveSince = r.EffectiveSince,
                        EffectiveUntil = r.EffectiveUntil,
                        Contract = r.Contract.ToString(),
                        Type = r.Type.ToString(),
                        SubType = r.SubType.ToString(),
                        FirstLevelDisplayName = r.FirstLevelDisplayName,
                        SecondLevelDisplayName = r.SecondLevelDisplayName,
                        ThirdLevelDisplayName = r.ThirdLevelDisplayName,
                        WorkShiftDisplayName = r.WorkShift.DisplayName,
                        WorkRegimenDisplayName = r.WorkShift.Regime.LegalName,
                        Salary = r.Salary,
                        Plus = r.Plus.Count > 0 ? Money.Total(r.Plus.Select(pl => pl.Amount)) : Money.Zero(),
                        TotalSalary = r.TotalSalary,
                        RatePerHour = r.RatePerHour,
                        EmployeeSalaryForm = r.EmployeeSalaryForm.ToString(),
                    }).ToArray();
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
                var companies = await _companyRepository.GetAllListAsync();

                var workplaces = (await _workPlaceUnitRepository.GetAllIncludingAsync(
                        w => w.Classification,
                        w => w.WorkPlacePayment,
                        w => w.Parent))
                    .Where(w => w.Classification.Level == 3 ||
                               w.DisplayName == KontecgCompanyBase.DefaultCompanyName)
                    .ToList();

                var relationships = await _employmentRepository.CurrentRelationshipAsync();

                workRelationshipInformation = (
                    from r in relationships
                    join w in workplaces on r.OrganizationUnitId equals w.Id
                    join c in companies on r.CompanyId equals c.Id
                    join p in persons on r.PersonId equals p.Id into personsGroup
                    from p in personsGroup.DefaultIfEmpty()
                    select new WorkRelationship
                    {
                        CompanyId = r.CompanyId,
                        Company = c,
                        PersonId = r.PersonId,
                        Person = p,
                        Exp = r.Exp,
                        OrganizationUnitId = r.OrganizationUnitId,
                        WorkPlaceUnit = w,
                        CenterCost = r.CenterCost,
                        OccupationCode = r.OccupationCode,
                        OccupationDescription = r.FullOccupationDescription,
                        OccupationCategory = r.OccupationCategory,
                        ComplexityGroup = r.ComplexityGroup,
                        LastDocumentId = r.Id,
                        OccupationResponsibility = r.Responsibility,
                        Code = r.Code,
                        MadeOn = r.MadeOn,
                        EffectiveSince = r.EffectiveSince,
                        EffectiveUntil = r.EffectiveUntil,
                        Contract = r.Contract.ToString(),
                        Type = r.Type.ToString(),
                        SubType = r.SubType.ToString(),
                        FirstLevelDisplayName = r.FirstLevelDisplayName,
                        SecondLevelDisplayName = r.SecondLevelDisplayName,
                        ThirdLevelDisplayName = r.ThirdLevelDisplayName,
                        WorkShiftDisplayName = r.WorkShift.DisplayName,
                        WorkRegimenDisplayName = r.WorkShift.Regime.LegalName,
                        Salary = r.Salary,
                        Plus = r.Plus.Count > 0 ? Money.Total(r.Plus.Select(pl => pl.Amount)) : Money.Zero(),
                        TotalSalary = r.TotalSalary,
                        RatePerHour = r.RatePerHour,
                        EmployeeSalaryForm = r.EmployeeSalaryForm.ToString(),
                    }).ToArray();
            }
            await uow.CompleteAsync();
            return workRelationshipInformation;
        }
    }
}
