using Kontecg.Application.Features;
using Kontecg.Authorization;
using Kontecg.Localization;
using System;

namespace Kontecg.Views
{
    /// <summary>
    ///     Represents a view for an application.
    /// </summary>
    public class ViewDefinition
    {
        /// <summary>
        ///     Creates a new <see cref="ViewDefinition" /> object.
        /// </summary>
        /// <param name="name">Unique name of the view</param>
        /// <param name="displayName">Display name of the view</param>
        /// <param name="type">Type of view used to resolve instances on DI or resources</param>
        /// <param name="category">Category related to this view.</param>
        /// <param name="icon">Icon related to the view</param>
        /// <param name="requiresAuthentication">This can be set to true if only authenticated users should access to this view item.</param>
        /// <param name="featureDependency">A feature dependency</param>
        /// <param name="permissionDependency">A permission dependency</param>
        public ViewDefinition(
            string name,
            ILocalizableString displayName,
            Type type,
            ViewCategory category = ViewCategory.MainView,
            string icon = null,
            bool requiresAuthentication = false,
            IFeatureDependency featureDependency = null,
            IPermissionDependency permissionDependency = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), @"View name can not be empty or null.");

            Name = name;
            DisplayName = displayName ??
                          throw new ArgumentNullException(nameof(displayName),
                              @"Display name of the view can not be null.");
            Type = type ?? throw new ArgumentNullException(nameof(type),
                @"Type of the view can not be null.");
            Category = category;
            Icon = icon;
            RequiresAuthentication = requiresAuthentication;
            FeatureDependency = featureDependency;
            PermissionDependency = permissionDependency;
        }

        /// <summary>
        ///     Unique name of the view item in the application.
        ///     Can be used to find this view item later.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Icon of the view if exists.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     Type of the resource.
        /// </summary>
        public Type ResourceType { get; set; }

        /// <summary>
        ///     Type of the view.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        ///     Display name of the view item. Required.
        /// </summary>
        public ILocalizableString DisplayName { get; set; }

        public ViewCategory Category { get; }

        /// <summary>
        ///     A permission dependency. Only users that can satisfy this permission dependency can access to this view item.
        ///     Optional.
        /// </summary>
        public IPermissionDependency PermissionDependency { get; set; }

        /// <summary>
        ///     A feature dependency.
        ///     Optional.
        /// </summary>
        public IFeatureDependency FeatureDependency { get; set; }

        /// <summary>
        ///     This can be set to true if only authenticated users should access to this view item.
        /// </summary>
        public bool RequiresAuthentication { get; set; }
    }
}
