using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using Kontecg.Data;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.EFCore;
using Kontecg.Extensions;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Security;

namespace Kontecg.Migrator
{
    public class MultiCompanyMigrateExecuter : ITransientDependency
    {
        private readonly IDbPerCompanyConnectionStringResolver _connectionStringResolver;

        private readonly IKontecgDbMigrator _hostMigrator;
        private readonly IKontecgDbMigrator _sgnomMigrator;
        private readonly IRepository<Company> _companyRepository;

        public MultiCompanyMigrateExecuter(
            KontecgDbMigrator hostMigrator,
            SGNOMDbMigrator sgnomMigrator,
            IRepository<Company> companyRepository,
            IDbPerCompanyConnectionStringResolver connectionStringResolver)
        {
            _hostMigrator = hostMigrator;
            _sgnomMigrator = sgnomMigrator;
            _companyRepository = companyRepository;
            _connectionStringResolver = connectionStringResolver;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Run(bool skipConnVerification)
        {
            var hostConnStr = _connectionStringResolver.GetNameOrConnectionString(new ConnectionStringResolveArgs(MultiCompanySides.Host));
            if (hostConnStr.IsNullOrWhiteSpace())
            {
                Logger.Warn("Configuration file should contain a connection string named 'Default'");
                return;
            }

            Logger.Info("Host database: " + ConnectionStringHelper.GetConnectionString(hostConnStr));

            if (!skipConnVerification)
            {
                Logger.Info("Continue to migration for this host database and all companies..? (Y/N): ");
                var command = Console.ReadLine();
                if (!command.IsIn("Y", "y"))
                {
                    Logger.Info("Migration canceled.");
                    return;
                }
            }

            Logger.Info("HOST database migration started...");

            try
            {
                _hostMigrator.CreateOrMigrateForHost();
            }
            catch (Exception ex)
            {
                Logger.Info("An error occurred during migration of host database:");
                Logger.Info(ex.ToString());
                Logger.Info("Canceled migrations.");
                return;
            }

            Logger.Info("HOST database migration completed.");
            Logger.Info("--------------------------------------------------------");

            var migratedDatabases = new HashSet<string>();
            var companies =
                _companyRepository.GetAllList(t => t.ConnectionString != null && t.ConnectionString != string.Empty);

            for (var i = 0; i < companies.Count; i++)
            {
                var company = companies[i];
                Logger.Info($"Company database migration started... ({i + 1} / {companies.Count})");
                Logger.Info("Name              : " + company.Name);
                Logger.Info("CompanyName       : " + company.CompanyName);
                Logger.Info("Company Id         : " + company.Id);
                Logger.Info("Connection string : " + SimpleStringCipher.Instance.Decrypt(company.ConnectionString));

                if (!migratedDatabases.Contains(company.ConnectionString))
                {
                    try
                    {
                        _hostMigrator.CreateOrMigrateForCompany(company);
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("An error occurred during migration of company database:");
                        Logger.Info(ex.ToString());
                        Logger.Info("Skipped this company and will continue for others...");
                    }

                    migratedDatabases.Add(company.ConnectionString);
                }
                else
                {
                    Logger.Info(
                        "This database has already migrated before (you have more than one company in same database). Skipping it....");
                }

                Logger.Info($"Company database migration completed. ({i + 1} / {companies.Count})");
                Logger.Info("--------------------------------------------------------");
            }

            if (!skipConnVerification)
            {
                Logger.Info("Continue to migration for SGNOM database and all companies..? (Y/N): ");
                var command = Console.ReadLine();
                if (!command.IsIn("Y", "y"))
                {
                    Logger.Info("Migration canceled.");
                    return;
                }
            }

            Logger.Info("SGNOM database migration started...");

            try
            {
                _sgnomMigrator.CreateOrMigrateForHost();
            }
            catch (Exception ex)
            {
                Logger.Info("An error occurred during migration of SGNOM database:");
                Logger.Info(ex.ToString());
                Logger.Info("Canceled migrations.");
                return;
            }

            Logger.Info("SGNOM database migration completed.");
            Logger.Info("--------------------------------------------------------");

            Logger.Info("All databases have been migrated.");
        }
    }
}
