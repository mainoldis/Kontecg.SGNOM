using Kontecg.Castle.MsAdapter;
using Kontecg.Dependency;
using Kontecg.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Kontecg.Migrator.DependencyInjection
{
    public static class ServiceCollectionRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            var services = new ServiceCollection();

            IdentityRegistrar.Register(services);

            services.UseCastleLoggerFactory();

            WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);
        }
    }
}
