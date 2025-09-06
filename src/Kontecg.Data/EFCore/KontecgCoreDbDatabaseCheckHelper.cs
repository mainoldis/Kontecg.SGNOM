using Kontecg.Domain.Uow;

namespace Kontecg.EFCore
{
    public class KontecgCoreDbDatabaseCheckHelper : DatabaseCheckHelper<KontecgCoreDbContext>
    {
        public KontecgCoreDbDatabaseCheckHelper(
            IDbContextProvider<KontecgCoreDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager)
            : base(dbContextProvider, unitOfWorkManager)
        {
        }
    }
}
