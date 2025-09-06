using Kontecg.Authorization;
using Kontecg.AutoMapper;
using Kontecg.EFCore;
using Kontecg.Localization;
using Kontecg.Modules;
using Kontecg.Reflection.Extensions;
using Kontecg.Dependency;
using System;

namespace Kontecg
{
    [DependsOn(
        typeof(KontecgCoreModule),
        typeof(KontecgDataModule))]
    public class KontecgAppModule : KontecgModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<CoreAuthorizationProvider>();

            Configuration.Modules.UseAutoMapper().Configurators.Add(configuration =>
            {
                configuration.ConstructServicesUsing(type => IocManager.Resolve(type));
                configuration.CreateMap<Enum, string>().ConvertUsing<LocalizedEnumConverter>();
            });

            //Adding custom AutoMapper configuration
            Configuration.Modules.UseAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.Register<LocalizedEnumConverter>(DependencyLifeStyle.Transient);
            IocManager.RegisterAssemblyByConvention(typeof(KontecgAppModule).GetAssembly());
        }
    }
}
