using System;
using Kontecg.Castle.MsAdapter;
using Kontecg.Dependency;
using Kontecg.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Kontecg.DependencyInjection
{
    public static class ServiceCollectionRegistrar
    {
        public static IServiceProvider Register(IIocManager iocManager, IServiceCollection services = null)
        {
            services ??= new ServiceCollection();

            IdentityRegistrar.Register(services);

            WorkflowRegistrar.Register(services);

            services.UseCastleLoggerFactory();
            
            return WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);
        }
    }
}
