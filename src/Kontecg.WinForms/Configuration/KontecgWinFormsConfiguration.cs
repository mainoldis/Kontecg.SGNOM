using System;
using Kontecg.Collections;
using Kontecg.Domain;
using Kontecg.Views;

namespace Kontecg.Configuration
{
    internal class KontecgWinFormsConfiguration : IKontecgWinFormsConfiguration
    {
        private Type _main;

        /// <inheritdoc />
        public bool UseSingleProcess { get; set; } = true;

        /// <inheritdoc />
        public bool UseDirectX { get; set; } = false;

        /// <inheritdoc />
        public Type MainType
        {
            get => _main;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));

                if (!typeof(BaseForm).IsAssignableFrom(value)
                    && !typeof(BaseRibbonForm).IsAssignableFrom(value)
                    && !typeof(BaseDirectXForm).IsAssignableFrom(value))
                    throw new KontecgException(value.AssemblyQualifiedName + " should be derived from " +
                                               typeof(IKontecgForm).AssemblyQualifiedName);

                _main = value;
            }
        }

        /// <inheritdoc />
        public ITypeList<ModuleRegistrationProvider> Providers { get; } = new TypeList<ModuleRegistrationProvider>();
    }
}
