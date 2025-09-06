using Kontecg.Baseline;
using Kontecg.Domain.Uow;
using Kontecg.Linq;
using System.Threading.Tasks;

namespace Kontecg.WorkRelations
{
    public class EmploymentDocumentManager : SGNOMDomainServiceBase
    {
        /// <inheritdoc />
        public EmploymentDocumentManager(IEmploymentRepository employmentRepository)
        {
            EmploymentRepository = employmentRepository;
            LocalizationSourceName = KontecgBaselineConsts.LocalizationSourceName;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        protected IEmploymentRepository EmploymentRepository { get; }

        public virtual async Task<long> CreateAsync(EmploymentDocument employmentDocument)
        {
            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin();
            await ValidateEmploymentDocumentAsync(employmentDocument);
            long ouId = await EmploymentRepository.InsertAndGetIdAsync(employmentDocument);
            await uow.CompleteAsync();
            return await Task.FromResult(ouId);
        }

        protected virtual async Task ValidateEmploymentDocumentAsync(EmploymentDocument employmentDocument)
        {
            //List<EmploymentDocument> siblings = (await FindChildrenAsync(organizationUnit.ParentId))
            //                                    .Where(ou => ou.Id != organizationUnit.Id)
            //                                    .ToList();

            //if (siblings.Any(ou => ou.DisplayName == organizationUnit.DisplayName))
            //{
            //    throw new UserFriendlyException(L("OrganizationUnitDuplicateDisplayNameWarning", organizationUnit.DisplayName));
            //}
        }
    }
}