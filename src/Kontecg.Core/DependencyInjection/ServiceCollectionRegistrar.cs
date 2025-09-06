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


            //var coreOptions = iocManager.Resolve<IKontecgCoreConfiguration>();

            //services.AddMassTransit(x => x.UsingRabbitMq((ctx, c) =>
            //{
            //    x.AddConsumers();

            //    c.Host(new Uri($"amqp://{coreOptions.MassTransitOptions.Host}:{coreOptions.MassTransitOptions.Port}{coreOptions.MassTransitOptions.VirtualHost}"),
            //        configurator =>
            //        {
            //            configurator.Username(coreOptions.MassTransitOptions.Username);
            //            configurator.Password(coreOptions.MassTransitOptions.Password);
            //            configurator.Heartbeat(coreOptions.MassTransitOptions.Heartbeat);
            //        });

            //    c.ReceiveEndpoint(coreOptions.MassTransitOptions.QueueName, endpoint =>
            //    {
            //        endpoint.ConfigureConsumers(ctx);
            //    });

            //    ctx.ConfigureEndpoints();
            //}));

            //iocManager.Release(coreOptions);

            return WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);
        }
    }
}
