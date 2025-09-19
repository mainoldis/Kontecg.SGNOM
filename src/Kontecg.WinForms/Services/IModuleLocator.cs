using System.Collections.Generic;
using Kontecg.Views;

namespace Kontecg.Services
{
    public interface IModuleLocator
    {
        IList<Module> Modules { get; }

        Module GetModule(string moduleType);

        Module GetModule(Module moduleType);

        Module GetModuleType(string moduleType, ViewCategory? category = null);

        Module GetModuleType(Module moduleType, ViewCategory? category = null);

        object GetModuleControl(Module moduleType, object viewModel, object parameter = null,
            ViewCategory category = ViewCategory.MainView);

        void ReleaseModuleControl(object control);

        bool IsModuleLoaded(Module moduleType);
    }
}