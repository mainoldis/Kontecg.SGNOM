using System.Collections.Generic;
using System.Linq;
using Kontecg.Domain.Entities;
using Kontecg.Organizations;
using Kontecg.WorkRelations;

namespace Kontecg.EFCore.Repositories
{
    /// <summary>
    ///     Base class for custom repositories of the application.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public abstract class SGNOMRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<SGNOMDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected SGNOMRepositoryBase(IDbContextProvider<SGNOMDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public IReadOnlyList<OrganizationUnitAncestor> GetOrganizationUnitAncestor(long id, int level = -1)
        {
            var ancestors = GetContext().GetOrganizationUnitAncestor(id, level).ToList();
            return ancestors;
        }
    }

    /// <summary>
    ///     Base class for custom repositories of the application.
    ///     This is a shortcut of <see cref="SGNOMRepositoryBase{TEntity,TPrimaryKey}" /> for <see cref="int" /> primary key.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class SGNOMRepositoryBase<TEntity> : SGNOMRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected SGNOMRepositoryBase(IDbContextProvider<SGNOMDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        //do not add any method here, add to the class above (since this inherits it)!!!
    }
}
