using Kontecg.Authorization.Users;
using Kontecg.MultiCompany;
using Kontecg.TestBase;
using Kontecg.Timing;
using System;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.EFCore;
using Castle.Facilities.Logging;
using Kontecg.Castle.Logging.Log4Net;
using Kontecg.Threading;
using NMoneys;

namespace Kontecg.SGNOM.Tests
{
    public abstract class SGNOMModuleTestBase : KontecgIntegratedTestBase<SGNOMTestModule>
    {
        private static IIocManager GetIocManager()
        {
            var iocManager = new IocManager();
            iocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseKontecgLog4Net().WithConfig("log4net.config"));
            return iocManager;
        }

        protected SGNOMModuleTestBase()
            :base(true, GetIocManager())
        {
            Clock.Provider = ClockProviders.Local;
            //Fix cultureInfo
            ThreadCultureSanitizer.Sanitize();
            Currency.InitializeAllCurrencies();
        }

        public void UsingDbContext(Action<SGNOMDbContext> action)
        {
            using var context = LocalIocManager.Resolve<SGNOMDbContext>();
            action(context);
            context.SaveChanges();
        }

        public T UsingDbContext<T>(Func<SGNOMDbContext, T> func)
        {
            using var context = LocalIocManager.Resolve<SGNOMDbContext>();
            var result = func(context);
            context.SaveChanges();

            return result;
        }

        public async Task UsingDbContextAsync(Func<SGNOMDbContext, Task> action)
        {
            await using var context = LocalIocManager.Resolve<SGNOMDbContext>();
            await action(context);
            await context.SaveChangesAsync(true);
        }

        public async Task<T> UsingDbContextAsync<T>(Func<SGNOMDbContext, Task<T>> func)
        {
            await using var context = LocalIocManager.Resolve<SGNOMDbContext>();
            var result = await func(context);
            await context.SaveChangesAsync();

            return result;
        }

        protected Company GetDefaultCompany()
        {
            return GetCompany(KontecgCompanyBase.DefaultCompanyName);
        }

        protected Company GetCompany(string companyName)
        {
            using var context = LocalIocManager.Resolve<KontecgCoreDbContext>();
            return context.Companies.Single(t => t.CompanyName == companyName);
        }

        protected User GetDefaultCompanyAdmin()
        {
            var defaultCompany = GetDefaultCompany();
            using var context = LocalIocManager.Resolve<KontecgCoreDbContext>();
            return context.Users.Single(u =>
                u.UserName == KontecgUserBase.AdminUserName && u.CompanyId == defaultCompany.Id);
        }
    }
}
