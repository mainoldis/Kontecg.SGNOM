using System;
using Castle.Core.Logging;
using Kontecg.Data;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore
{
    public abstract class DatabaseCheckHelper<TDbContext> : IDatabaseCheckHelper where TDbContext : DbContext
    {
        protected readonly IDbContextProvider<TDbContext> DbContextProvider;
        protected readonly IUnitOfWorkManager UnitOfWorkManager;

        public ILogger Logger { get; set; }

        public DatabaseCheckHelper(
            IDbContextProvider<TDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager
        )
        {
            DbContextProvider = dbContextProvider;
            UnitOfWorkManager = unitOfWorkManager;
            Logger = NullLogger.Instance;
        }

        public virtual bool CanConnect(string connectionString)
        {
            if (connectionString.IsNullOrEmpty())
                //connectionString is null for unit tests
                return true;

            try
            {
                using var uow = UnitOfWorkManager.Begin();
                // Switching to host is necessary for single company mode.
                using (UnitOfWorkManager.Current.SetCompanyId(null))
                {
                    DbContextProvider.GetDbContext().Database.OpenConnection();
                    uow.Complete();
                }
            }
            catch(Exception ex)
            {
                string message = "The application cannot connect to the specified database, " +
                                 "because the database doesn't exist, its version is older " +
                                 "than that of the application or its schema does not match " +
                                 "the ORM data model structure.";

                Logger.Warn(message, ex);
                return false;
            }

            return true;
        }
    }
}
