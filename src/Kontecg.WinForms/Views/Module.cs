using Kontecg.Application.Features;
using Kontecg.Authorization;
using Kontecg.Localization;
using System;
using System.Collections.Generic;
using Kontecg.MultiCompany;

namespace Kontecg.Views
{
    public class Module : IEquatable<Module>
    {
        private readonly ModuleDefinition _moduleDefinition;
        private readonly ILocalizationContext _localizationContext;

        public Module(ModuleDefinition moduleDefinition, ILocalizationContext localizationContext)
        {
            _moduleDefinition = moduleDefinition;
            _localizationContext = localizationContext;

            Name = moduleDefinition.Name;
            DisplayName = moduleDefinition.DisplayName.Localize(localizationContext);
            Id = moduleDefinition.Id;
            Order = moduleDefinition.Order;
            ImageName = moduleDefinition.ImageName;
            SmallImageUri = moduleDefinition.SmallImageUri;
            LargeImageUri = moduleDefinition.LargeImageUri;
            ResourceType = moduleDefinition.ResourceType;

            SubModules = new List<Module>();
            OtherViews = new List<UserView>();
        }

        internal void FillUserViews(UserIdentifier user, MultiCompanySides multiCompanySide,
            PermissionDependencyContext permissionDependencyContext,
            FeatureDependencyContext featureDependencyContext)
        {
            foreach (var viewDefinition in _moduleDefinition.Views)
            {
                if (viewDefinition.RequiresAuthentication && user == null)
                {
                    continue;
                }

                if (viewDefinition.PermissionDependency != null
                    && (user == null || !viewDefinition.PermissionDependency
                                                       .IsSatisfied(permissionDependencyContext)))
                {
                    continue;
                }

                if (viewDefinition.FeatureDependency != null &&
                    (multiCompanySide == MultiCompanySides.Company ||
                     (user != null && user.CompanyId != null)) &&
                    !viewDefinition.FeatureDependency.IsSatisfied(featureDependencyContext))
                {
                    continue;
                }

                UserView userViewItem = new UserView(this, viewDefinition, _localizationContext);

                switch (userViewItem.Category)
                {
                    case ViewCategory.MainView:
                        MainView = userViewItem;
                        break;
                    case ViewCategory.DetailView:
                        DetailView = userViewItem;
                        break;
                    case ViewCategory.FilterPaneView:
                        FilterPaneView = userViewItem;
                        break;
                    case ViewCategory.FilterPaneCollapsedView:
                        FilterPaneCollapsedView = userViewItem;
                        break;
                    case ViewCategory.CustomFilterView:
                        CustomFilterView = userViewItem;
                        break;
                    case ViewCategory.GroupFilterView:
                        GroupFilterView = userViewItem;
                        break;
                    case ViewCategory.EditView:
                        EditView = userViewItem;
                        break;
                    case ViewCategory.PeekView:
                        PeekView = userViewItem;
                        break;
                    case ViewCategory.ExportView:
                        ExportView = userViewItem;
                        break;
                    case ViewCategory.PrintView:
                        PrintView = userViewItem;
                        break;
                    case ViewCategory.AnalysisView:
                        AnalysisView = userViewItem;
                        break;
                    case ViewCategory.SettingsView:
                        SettingsView = userViewItem;
                        break;
                    default:
                        OtherViews.Add(userViewItem);
                        break;
                }
            }
        }

        public string Name { get; }

        public string DisplayName { get; }

        public Guid Id { get; }

        public int Order { get; }

        /// <summary>
        ///     Icon of the view if exists.
        /// </summary>
        public string ImageName { get; }

        /// <summary>
        ///     Icon of the view if exists.
        /// </summary>
        public string SmallImageUri { get; }

        /// <summary>
        ///     Icon of the view if exists.
        /// </summary>
        public string LargeImageUri { get; }

        /// <summary>
        ///     Type of the resource.
        /// </summary>
        public Type ResourceType { get; }

        public IList<Module> SubModules { get; }

        public UserView MainView { get; private set; }

        public UserView DetailView { get; private set; }

        public UserView FilterPaneView { get; private set; }

        public UserView FilterPaneCollapsedView { get; private set; }

        public UserView CustomFilterView { get; private set; }

        public UserView GroupFilterView { get; private set; }

        public UserView EditView { get; private set; }

        public UserView PeekView { get; private set; }

        public UserView ExportView { get; private set; }

        public UserView PrintView { get; private set; }

        public UserView AnalysisView { get; private set; }

        public UserView SettingsView { get; private set; }

        public IList<UserView> OtherViews { get; }

        public bool CanShowPeekView => PeekView != null;

        #region IEquatable

        public bool Equals(Module other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Name == other.Name && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Module otherDefinition && Equals(otherDefinition);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Id);
        }

        #endregion
    }
}