namespace Kontecg.Views
{
    internal class ModuleRegistrationContext(IModuleManager manager) : IModuleRegistrationContext
    {
        public IModuleManager Manager { get; } = manager;
    }
}