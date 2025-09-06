using System;
using System.Threading.Tasks;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Session;
using Microsoft.Extensions.Configuration;

namespace Kontecg.EFCore
{
    /// <summary>
    ///     Implements <see cref="IDbPerContextConnectionStringResolver" /> to dynamically resolve
    ///     connection string for a multi context application.
    /// </summary>
    public class DbPerContextConnectionStringResolver : DefaultConnectionStringResolver,
        IDbPerContextConnectionStringResolver
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly IConfigurationRoot _appConfiguration;
        private readonly ICompanyCache _companyCache;

        public DbPerContextConnectionStringResolver(
            IKontecgStartupConfiguration configuration,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IAppConfigurationAccessor configurationAccessor,
            ICompanyCache companyCache)
            : base(configuration)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _appConfiguration = configurationAccessor.Configuration;
            _companyCache = companyCache;

            KontecgSession = NullKontecgSession.Instance;
        }

        /// <summary>
        ///     Reference to the session.
        /// </summary>
        public IKontecgSession KontecgSession { get; set; }

        public override string GetNameOrConnectionString(ConnectionStringResolveArgs args)
        {
            return GetNameOrConnectionString(args.MultiCompanySide == MultiCompanySides.Host ?
                new DbPerContextConnectionStringResolverArgs(null, args) :
                new DbPerContextConnectionStringResolverArgs(GetCurrentCompanyId(), args));
        }

        public virtual string GetNameOrConnectionString(DbPerContextConnectionStringResolverArgs args)
        {
            if (args["DbContextConcreteType"] is Type &&
                args["DbContextConcreteType"] as Type == typeof(KontecgCoreDbContext))
            {
                if (args.CompanyId == null)
                    //Requested for host
                    return base.GetNameOrConnectionString(args);

                var companyCacheItem = _companyCache.Get(args.CompanyId.Value);
                return companyCacheItem.ConnectionString.IsNullOrEmpty()
                    ? base.GetNameOrConnectionString(args)
                    : companyCacheItem.ConnectionString;
            }

            var typeName = (args["DbContextConcreteType"] as Type)?.Name;
            if (typeName == null) return base.GetNameOrConnectionString(args);
            var result = _appConfiguration.GetConnectionString(typeName.ToUpperInvariant().RemovePostFix("DBCONTEXT"));
            return !result.IsNullOrEmpty() ? result : base.GetNameOrConnectionString(args);
        }

        public override async Task<string> GetNameOrConnectionStringAsync(ConnectionStringResolveArgs args)
        {
            return await GetNameOrConnectionStringAsync(args.MultiCompanySide == MultiCompanySides.Host ?
                new DbPerContextConnectionStringResolverArgs(null, args) :
                new DbPerContextConnectionStringResolverArgs(GetCurrentCompanyId(), args));
        }

        public virtual async Task<string> GetNameOrConnectionStringAsync(DbPerContextConnectionStringResolverArgs args)
        {
            if (args["DbContextConcreteType"] is Type &&
                args["DbContextConcreteType"] as Type == typeof(KontecgCoreDbContext))
            {
                if (args.CompanyId == null)
                    //Requested for host
                    return await base.GetNameOrConnectionStringAsync(args);

                var companyCacheItem = _companyCache.Get(args.CompanyId.Value);
                return companyCacheItem.ConnectionString.IsNullOrEmpty()
                    ? await base.GetNameOrConnectionStringAsync(args)
                    : companyCacheItem.ConnectionString;
            }

            var typeName = (args["DbContextConcreteType"] as Type)?.Name;
            if (typeName == null) return await base.GetNameOrConnectionStringAsync(args);
            var result = _appConfiguration.GetConnectionString(typeName.ToUpperInvariant().RemovePostFix("DBCONTEXT"));
            return !result.IsNullOrEmpty() ? result : await base.GetNameOrConnectionStringAsync(args);
        }

        protected virtual int? GetCurrentCompanyId()
        {
            return _currentUnitOfWorkProvider.Current != null
                ? _currentUnitOfWorkProvider.Current.GetCompanyId()
                : KontecgSession.CompanyId;
        }
    }
}
