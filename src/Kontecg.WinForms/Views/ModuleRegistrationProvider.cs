using Kontecg.Dependency;

namespace Kontecg.Views
{
    /// <summary>
    ///     This class should be implemented by classes which register
    ///     modules and views of the application.
    /// </summary>
    public abstract class ModuleRegistrationProvider : ITransientDependency
    {
        /// <summary>
        ///     Used to register modules.
        /// </summary>
        /// <param name="context">Module registration context</param>
        public abstract void Register(IModuleRegistrationContext context);
    }
}