using System;
using System.Transactions;
using Castle.Core.Logging;
using Kontecg.Baseline.EFCore;
using Kontecg.Data;
using Kontecg.Domain.Uow;
using Kontecg.Migrations.Seed;
using Kontecg.MultiCompany;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore
{
    public class SGNOMDbMigrator : KontecgDbMigrator<SGNOMDbContext>
    {
        private readonly DbPerContextConnectionStringResolver _connectionStringResolver;
        private readonly IContentFolders _contentFolders;

        public SGNOMDbMigrator(
            IUnitOfWorkManager unitOfWorkManager,
            IDbPerCompanyConnectionStringResolver dbPerCompanyConnectionStringResolver,
            DbPerContextConnectionStringResolver dbPerContextConnectionStringResolver,
            IDbContextResolver dbContextResolver,
            IContentFolders contentFolders)
            : base(unitOfWorkManager, dbPerCompanyConnectionStringResolver, dbContextResolver)
        {
            _connectionStringResolver = dbPerContextConnectionStringResolver;
            _contentFolders = contentFolders;
        }

        public override void CreateOrMigrateForHost()
        {
            CreateOrMigrateForHost((context, logger) =>
            {
                context.SuppressAutoSetCompanyId = true;

                new DefaultSalaryDefinitionsBuilder(context, logger).Create();
                new DefaultWorkRegimenBuilder(context, logger).Create();
                new DefaultGeneralDataBuilder(context, _contentFolders, logger).Create();
                new DefaultTemplateDataBuilder(context, _contentFolders, logger).Create();
            });
        }

        protected override void CreateOrMigrate(KontecgCompanyBase company, Action<SGNOMDbContext, ILogger> seedAction)
        {
            var args = new DbPerContextConnectionStringResolverArgs(
                    company?.Id,
                    MultiCompanySides.Company
                )
                { ["DbContextType"] = typeof(SGNOMDbContext), ["DbContextConcreteType"] = typeof(SGNOMDbContext) };

            var nameOrConnectionString = ConnectionStringHelper.GetConnectionString(
                _connectionStringResolver.GetNameOrConnectionString(args)
            );

            using var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using var dbContext = DbContextResolver.Resolve<SGNOMDbContext>(nameOrConnectionString, null);
            Logger.Info("Creating database");
            dbContext.Database.Migrate();
            Logger.Info("Database created");
            Logger.Info("Seeding with initial data");
            seedAction?.Invoke(dbContext, Logger);
            Logger.Info("Seed was saved");
            UnitOfWorkManager.Current.SaveChanges();
            uow.Complete();
        }
    }
}
