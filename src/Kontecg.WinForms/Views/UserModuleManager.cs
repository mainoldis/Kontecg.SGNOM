using System.Collections.Generic;
using Kontecg.Application.Features;
using Kontecg.Authorization;
using Kontecg.Dependency;
using Kontecg.Localization;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Session;

namespace Kontecg.Views
{
    internal class UserModuleManager(
        IIocResolver iocResolver,
        ILocalizationContext localizationContext,
        IModuleManager moduleManager)
        : IUserModuleManager, ITransientDependency
    {
        private Module _defaultModule;

        public IKontecgSession KontecgSession { get; set; } = NullKontecgSession.Instance;

        /// <inheritdoc />
        public Module Default {
            get => _defaultModule ??= new Module(ModuleDefinition.Unknown, localizationContext);
            private set => _defaultModule = value;
        }

        /// <inheritdoc />
        public IList<Module> GetModules(UserIdentifier user)
        {
            var modules = new List<Module> ();

            FillUserModules(user, moduleManager.Modules, modules);

            return modules;
        }

        private int FillUserModules(UserIdentifier user, IList<ModuleDefinition> moduleDefinitions, IList<Module> modules)
        {
            var addedModuleCount = 0;

            using IScopedIocResolver scope = iocResolver.CreateScope();
            PermissionDependencyContext permissionDependencyContext = scope.Resolve<PermissionDependencyContext>();
            permissionDependencyContext.User = user;

            FeatureDependencyContext featureDependencyContext = scope.Resolve<FeatureDependencyContext>();
            featureDependencyContext.CompanyId = user == null ? null : user.CompanyId;

            foreach (var moduleDefinition in moduleDefinitions)
            {
                if (moduleDefinition.RequiresAuthentication && user == null)
                {
                    continue;
                }

                if (moduleDefinition.PermissionDependency != null 
                    && (user == null || !moduleDefinition.PermissionDependency
                                                         .IsSatisfied(permissionDependencyContext)))
                {
                    continue;
                }

                if (moduleDefinition.FeatureDependency != null &&
                    (KontecgSession.MultiCompanySide == MultiCompanySides.Company ||
                     (user != null && user.CompanyId != null)) &&
                    !moduleDefinition.FeatureDependency.IsSatisfied(featureDependencyContext))
                {
                    continue;
                }

                Module moduleItem = new Module(moduleDefinition, localizationContext);
                moduleItem.FillUserViews(user, KontecgSession.MultiCompanySide, permissionDependencyContext, featureDependencyContext);

                //if ((Default == null || (Default != null && Default.Name == ModuleDefinition.Unknown.Name) &&
                //        moduleDefinition.IsDefault)) 
                //    Default = moduleItem;

                if (moduleDefinition.IsLeaf ||
                    FillUserModules(user, moduleDefinition.SubModules, moduleItem.SubModules) > 0)
                {
                    modules.Add(moduleItem);
                    ++addedModuleCount;
                }
            }

            return addedModuleCount;
        }
    }
}