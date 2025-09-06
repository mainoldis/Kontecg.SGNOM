using Kontecg.Domain.Uow;

namespace Kontecg.EFCore
{
    public class SGNOMDatabaseCheckHelper : DatabaseCheckHelper<SGNOMDbContext>
    {
        public SGNOMDatabaseCheckHelper(
            IDbContextProvider<SGNOMDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager)
            : base(dbContextProvider, unitOfWorkManager)
        {
        }
    }
}
