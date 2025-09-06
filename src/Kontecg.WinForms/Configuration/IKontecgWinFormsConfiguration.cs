using System;
using Kontecg.Collections;
using Kontecg.Views;

namespace Kontecg.Configuration
{
    public interface IKontecgWinFormsConfiguration
    {
        bool UseSingleProcess { get; set; }

        bool UseDirectX { get; set; }

        Type MainType { get; set; }

        /// <summary>
        ///     List of module providers.
        /// </summary>
        ITypeList<ModuleRegistrationProvider> Providers { get; }
    }
}
