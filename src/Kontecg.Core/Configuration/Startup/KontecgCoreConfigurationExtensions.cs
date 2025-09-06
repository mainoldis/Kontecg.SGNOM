using System;

namespace Kontecg.Configuration.Startup
{
    public static class KontecgCoreConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure core module.
        /// </summary>
        /// <param name="moduleConfigurations"></param>
        /// <returns></returns>
        public static void UseCore(this IModuleConfigurations moduleConfigurations)
        {
            UseCore(moduleConfigurations, optionsAction => {});
        }

        /// <summary>
        ///     Used to configure core module.
        /// </summary>
        /// <param name="moduleConfigurations"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static void UseCore(this IModuleConfigurations moduleConfigurations, Action<IKontecgCoreConfiguration> optionsAction)
        {
            var iocManager = moduleConfigurations.KontecgConfiguration.IocManager;
            optionsAction(iocManager.Resolve<IKontecgCoreConfiguration>());
        }
    }
}
