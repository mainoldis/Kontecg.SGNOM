using Kontecg.Domain.Entities;

namespace Kontecg.EFCore.Repositories
{
    /// <summary>
    ///     Base class for custom repositories of the application.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public abstract class
        KontecgRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<KontecgCoreDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected KontecgRepositoryBase(IDbContextProvider<KontecgCoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        //add your common methods for all repositories
    }

    /// <summary>
    ///     Base class for custom repositories of the application.
    ///     This is a shortcut of <see cref="KontecgRepositoryBase{TEntity,TPrimaryKey}" /> for <see cref="int" /> primary key.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class KontecgRepositoryBase<TEntity> : KontecgRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected KontecgRepositoryBase(IDbContextProvider<KontecgCoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        //do not add any method here, add to the class above (since this inherits it)!!!
    }
}
