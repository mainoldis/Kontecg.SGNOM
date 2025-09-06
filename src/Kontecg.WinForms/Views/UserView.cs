using System;
using JetBrains.Annotations;
using Kontecg.Localization;

namespace Kontecg.Views
{
    /// <summary>
    ///     Represents a view shown to the user.
    /// </summary>
    public class UserView
    {
        /// <summary>
        ///     Creates a new <see cref="UserView" /> object from given <see cref="ViewDefinition" />.
        /// </summary>
        public UserView(Module module, ViewDefinition viewDefinition, ILocalizationContext localizationContext)
        {
            Owner = module;
            Name = viewDefinition.Name;
            Icon = viewDefinition.Icon;
            ResourceType = viewDefinition.ResourceType;
            DisplayName = viewDefinition.DisplayName.Localize(localizationContext);
            Category = viewDefinition.Category;
            Type = viewDefinition.Type;
        }

        /// <summary>
        ///     Unique name of the view item in the application.
        ///     Can be used to find this view item later.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Display name of the view item. Required.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        ///     Category related to this view
        /// </summary>
        public ViewCategory Category { get; }

        public Module Owner { get; }

        /// <summary>
        ///     Icon of the view if exists.
        /// </summary>
        public string Icon { get; }

        /// <summary>
        ///     Type of the resource.
        /// </summary>
        public Type ResourceType { get; set; }

        /// <summary>
        ///     Type of the view.
        /// </summary>
        public Type Type { get; }
        
        [CanBeNull] 
        public object Control { get; set; }

        public bool IsForm => Control is BaseForm || Control is BaseDirectXForm || Control is BaseRibbonForm;
    }
}
