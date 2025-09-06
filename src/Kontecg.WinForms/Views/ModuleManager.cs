using System.Collections.Generic;
using Kontecg.Configuration;
using Kontecg.Dependency;

namespace Kontecg.Views
{
    internal class ModuleManager(IIocResolver iocResolver, IKontecgWinFormsConfiguration configuration)
        : IModuleManager, ISingletonDependency
    {
        /// <inheritdoc />
        public IList<ModuleDefinition> Modules { get; } = new List<ModuleDefinition>();

        public void Initialize()
        {
            var context = new ModuleRegistrationContext(this);

            foreach (var providerType in configuration.Providers)
            {
                using var provider = iocResolver.ResolveAsDisposable<ModuleRegistrationProvider>(providerType);
                provider.Object.Register(context);
            }
        }
    }
}