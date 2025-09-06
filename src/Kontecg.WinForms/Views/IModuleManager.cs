using System.Collections.Generic;

namespace Kontecg.Views
{
    public interface IModuleManager
    {
        IList<ModuleDefinition> Modules { get; }
    }
}