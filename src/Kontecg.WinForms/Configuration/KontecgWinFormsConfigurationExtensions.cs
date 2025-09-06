using Kontecg.Configuration.Startup;

namespace Kontecg.Configuration
{
    public static class KontecgWinFormsConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure winforms module.
        /// </summary>
        /// <param name="moduleConfigurations"></param>
        /// <returns></returns>
        public static IKontecgWinFormsConfiguration UseWinForms(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.KontecgConfiguration.Get<IKontecgWinFormsConfiguration>();
        }
    }
}
