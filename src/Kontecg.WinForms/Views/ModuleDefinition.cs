using System;
using System.Collections.Generic;
using Kontecg.Application.Features;
using Kontecg.Authorization;
using Kontecg.Collections.Extensions;
using Kontecg.Localization;

namespace Kontecg.Views
{
    public sealed class ModuleDefinition(
        string name,
        Guid id,
        ILocalizableString displayName = null,
        int order = 0,
        string imageName = null,
        string smallImageUri = null,
        string largeImageUri = null,
        Type resourceType = null,
        bool isDefault = false,
        IPermissionDependency permissionDependency = null,
        IFeatureDependency featureDependency = null,
        bool requiresAuthentication = false) : IEquatable<ModuleDefinition>
    {
        public static readonly ModuleDefinition Unknown = new("Unknown", Guid.Empty, new FixedLocalizableString("Unknown"));

        public string Name { get; } = name;

        public ILocalizableString DisplayName { get; } = displayName;

        public Guid Id { get; } = id;

        public int Order { get; } = order;

        /// <summary>
        ///     Icon of the view if exists.
        /// </summary>
        public string ImageName { get; } = imageName;

        /// <summary>
        ///     Icon of the view if exists.
        /// </summary>
        public string SmallImageUri { get; } = smallImageUri;

        /// <summary>
        ///     Icon of the view if exists.
        /// </summary>
        public string LargeImageUri { get; } = largeImageUri;

        /// <summary>
        ///     Type of the resource.
        /// </summary>
        public Type ResourceType { get; } = resourceType;

        public bool IsDefault { get; } = isDefault;

        /// <summary>
        ///     A permission dependency. Only users that can satisfy this permission dependency can access to this module item.
        ///     Optional.
        /// </summary>
        public IPermissionDependency PermissionDependency { get; } = permissionDependency;

        /// <summary>
        ///     A feature dependency.
        ///     Optional.
        /// </summary>
        public IFeatureDependency FeatureDependency { get; } = featureDependency;

        /// <summary>
        ///     This can be set to true if only authenticated users should access to this module item.
        /// </summary>
        public bool RequiresAuthentication { get; } = requiresAuthentication;

        #region SubModules

        /// <summary>
        ///     Module items (first level).
        /// </summary>
        public List<ModuleDefinition> SubModules { get; } = [];

        /// <summary>
        ///     Adds a <see cref="ModuleDefinition" /> to <see cref="SubModules" />.
        /// </summary>
        /// <param name="moduleItem"><see cref="ModuleDefinition" /> to be added</param>
        /// <returns>This <see cref="ModuleDefinition" /> object</returns>
        public ModuleDefinition AddSubModule(ModuleDefinition moduleItem)
        {
            SubModules.Add(moduleItem);
            return this;
        }

        /// <summary>
        ///     Remove module item with given name
        /// </summary>
        /// <param name="name"></param>
        public void RemoveSubModule(string name)
        {
            SubModules.RemoveAll(m => m.Name == name);
        }

        /// <summary>
        ///     Returns true if this menu item has no child <see cref="SubModules" />.
        /// </summary>
        public bool IsLeaf => SubModules.IsNullOrEmpty();

        #endregion

        #region Views

        /// <summary>
        ///     View items on this level.
        /// </summary>
        public List<ViewDefinition> Views { get; } = [];

        /// <summary>
        ///     Adds a <see cref="ViewDefinition" /> to <see cref="Views" />.
        /// </summary>
        /// <param name="viewItem"><see cref="ViewDefinition" /> to be added</param>
        /// <returns>This <see cref="ViewDefinition" /> object</returns>
        public ModuleDefinition AddView(ViewDefinition viewItem)
        {
            Views.Add(viewItem);
            return this;
        }

        /// <summary>
        ///     Remove view item with given name
        /// </summary>
        /// <param name="name"></param>
        public void RemoveView(string name)
        {
            Views.RemoveAll(m => m.Name == name);
        }

        #endregion

        #region IEquatable

        public bool Equals(ModuleDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Name == other.Name && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is ModuleDefinition otherDefinition && Equals(otherDefinition);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Id);
        }

        public static bool operator ==(ModuleDefinition p1, ModuleDefinition p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(ModuleDefinition p1, ModuleDefinition p2)
        {
            return !(p1 == p2);
        }

        #endregion
    }
}