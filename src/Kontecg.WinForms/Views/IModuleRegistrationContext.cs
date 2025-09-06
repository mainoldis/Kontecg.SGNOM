namespace Kontecg.Views
{
    /// <summary>
    ///     Provides infrastructure to set component registration.
    /// </summary>
    public interface IModuleRegistrationContext
    {
        /// <summary>
        ///     Gets a reference to the module manager.
        /// </summary>
        IModuleManager Manager { get; }
    }
}