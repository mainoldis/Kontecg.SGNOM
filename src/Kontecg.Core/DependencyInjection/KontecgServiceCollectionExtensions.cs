using System;
using JetBrains.Annotations;
using Kontecg.Castle.MsAdapter;
using Kontecg.Modules;
using Kontecg.Domain.Uow;
using System.Collections.Generic;
using Kontecg;
using Kontecg.Auditing;
using Kontecg.Runtime.Validation;
using Kontecg.Dependency;
using Kontecg.Castle.Logging.MsLogging;
using ICastleLoggerFactory = Castle.Core.Logging.ILoggerFactory;

// ReSharper disable once CheckNamespace - This is done to add extension methods to Microsoft.Extensions.DependencyInjection namespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class KontecgServiceCollectionExtensions
    {
        /// <summary>
        /// Integrates Kontecg to Microsoft DI.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="KontecgModule"/>.</typeparam>
        /// <param name="services">Services.</param>
        /// <param name="optionsAction">An action to get/modify options</param>
        /// <param name="removeConventionalInterceptors">Removes the conventional interceptors</param>
        public static IServiceProvider AddKontecg<TStartupModule>(this IServiceCollection services,
            [CanBeNull] Action<KontecgBootstrapperOptions> optionsAction = null,
            bool removeConventionalInterceptors = true)
            where TStartupModule : KontecgModule
        {
            if (removeConventionalInterceptors)
            {
                RemoveConventionalInterceptionSelectors();
            }

            var kontecgBootstrapper = AddKontecgBootstrapper<TStartupModule>(services, optionsAction);
            ConfigureHostedServices(services, kontecgBootstrapper.IocManager);

            return WindsorRegistrationHelper.CreateServiceProvider(kontecgBootstrapper.IocManager.IocContainer, services);
        }

        /// <summary>
        /// Integrates Kontecg to Microsoft DI without creating a IServiceProvider.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="KontecgModule"/>.</typeparam>
        /// <param name="services">Services.</param>
        /// <param name="optionsAction">An action to get/modify options</param>
        /// <param name="removeConventionalInterceptors">Removes the conventional interceptors</param>
        public static void AddKontecgWithoutCreatingServiceProvider<TStartupModule>(this IServiceCollection services,
            [CanBeNull] Action<KontecgBootstrapperOptions> optionsAction = null,
            bool removeConventionalInterceptors = true)
            where TStartupModule : KontecgModule
        {
            if (removeConventionalInterceptors)
            {
                RemoveConventionalInterceptionSelectors();
            }

            var kontecgBootstrapper = AddKontecgBootstrapper<TStartupModule>(services, optionsAction);
            ConfigureHostedServices(services, kontecgBootstrapper.IocManager);
        }

        private static void RemoveConventionalInterceptionSelectors()
        {
            UnitOfWorkDefaultOptions.ConventionalUowSelectorList = new List<Func<Type, bool>>();
            KontecgAuditingDefaultOptions.ConventionalAuditingSelectorList = new List<Func<Type, bool>>();
            KontecgValidationDefaultOptions.ConventionalValidationSelectorList = new List<Func<Type, bool>>();
        }

        private static void ConfigureHostedServices(IServiceCollection services, IIocResolver iocResolver)
        {
        }

        private static KontecgBootstrapper AddKontecgBootstrapper<TStartupModule>(IServiceCollection services,
            Action<KontecgBootstrapperOptions> optionsAction)
            where TStartupModule : KontecgModule
        {
            var kontecgBootstrapper = KontecgBootstrapper.Create<TStartupModule>(optionsAction);

            services.AddSingleton(kontecgBootstrapper);

            return kontecgBootstrapper;
        }

        public static void UseCastleLoggerFactory(this IServiceCollection services)
        {
            var castleLoggerFactory = services.GetSingletonServiceOrNull<ICastleLoggerFactory>();
            if (castleLoggerFactory == null)
                return;

            services
                .GetSingletonService<Microsoft.Extensions.Logging.ILoggerFactory>()
                .AddCastleLogger(castleLoggerFactory);
        }
    }
}
