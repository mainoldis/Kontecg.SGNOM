using Kontecg.Baseline.EFCore;
using Kontecg.Domain.Uow;
using Kontecg.Migrations.Seed.Companies;
using Kontecg.Migrations.Seed.Host;
using Kontecg.MultiCompany;

namespace Kontecg.EFCore
{
    public class KontecgDbMigrator : KontecgDbMigrator<KontecgCoreDbContext>
    {
        private readonly IContentFolders _contentFolders;

        public KontecgDbMigrator(
            IUnitOfWorkManager unitOfWorkManager,
            IConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver,
            IContentFolders contentFolders)
            : base(
                unitOfWorkManager,
                connectionStringResolver,
                dbContextResolver)
        {
            _contentFolders = contentFolders;
        }

        public override void CreateOrMigrateForHost()
        {
            CreateOrMigrateForHost((context, logger) =>
            {
                context.SuppressAutoSetCompanyId = true;

                //Host seed
                new InitialHostDbBuilder(context,logger).Create();

                new DefaultDemographicsBuilder(context, _contentFolders, logger).Create();
                new DefaultAccountingInfoBuilder(context, logger).Create();

                //Default company seed(in host database).
                new DefaultCompanyBuilder(context, logger).Create();
                new CompanyRoleAndUserBuilder(context, 1, logger).Create();
                new CompanyAccountingInfoBuilder(context, 1, logger).Create();
                new OrganizationUnitBuilder(context, 1, logger).Create();
            });
        }

        public override void CreateOrMigrateForCompany(KontecgCompanyBase company)
        {
            CreateOrMigrateForCompany(company, (context, logger) =>
            {
                new CompanyRoleAndUserBuilder(context, company.Id, logger).Create();
                new CompanyAccountingInfoBuilder(context, company.Id, logger).Create();
                new OrganizationUnitBuilder(context, company.Id, logger).Create();
            });
        }
    }
}
