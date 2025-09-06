using Kontecg.Authorization.Roles;
using Kontecg.Baseline.Configuration;
using Kontecg.BlobStoring;
using Kontecg.BlobStoring.FileSystem;
using Kontecg.Configuration;
using Kontecg.Localization;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using Kontecg.Storage.Blobs;

namespace Kontecg
{
    [DependsOn(typeof(KontecgCoreModule))]
    public class SGNOMCoreModule : KontecgModule
    {
        public override void PreInitialize()
        {
            //Configuro los idiomas del módulo
            SGNOMLocalizationConfigurer.Configure(Configuration.Localization);

            //Proveedor de configuración
            Configuration.Settings.Providers.Add<SGNOMSettingProvider>();

            //Configuro roles propios del módulo y que son estáticos
            SGNOMRoleConfig.Configure(Configuration.Modules.UseBaseline().RoleManagement);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SGNOMCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            SetContentFolder();
        }

        private void SetContentFolder()
        {
            var contentFolders = IocManager.Resolve<IContentFolders>();
            Configuration.Modules.UseBlobStoring().Containers.Configure<HumanResourcesContainer>(option => option.UseFileSystem(fileSystem =>
            {
                fileSystem.BasePath = contentFolders.DataFolder;
            }));
        }
    }
}
