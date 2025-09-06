using AutoMapper;
using Kontecg.Authorization;
using Kontecg.AutoMapper;
using Kontecg.Calendar.Dto;
using Kontecg.EFCore;
using Kontecg.Modules;
using Kontecg.Organizations;
using Kontecg.Organizations.Dto;
using Kontecg.Reflection.Extensions;
using Kontecg.Salary;
using Kontecg.Timing;
using Kontecg.Timing.Dto;
using Kontecg.WorkRelations;
using Kontecg.WorkRelations.Dto;

namespace Kontecg
{
    [DependsOn(
        typeof(SGNOMCoreModule),
        typeof(SGNOMDataModule),
        typeof(KontecgAppModule))]
    public class SGNOMServicesModule : KontecgModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<SGNOMAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.UseAutoMapper().Configurators.Add(CreateMaps);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SGNOMServicesModule).GetAssembly());
        }

        private void CreateMaps(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<WorkRegime, RTDDto>();
            configuration.CreateMap<WorkShift, WorkShiftDto>();

            configuration.CreateMap<Occupation, OccupationListDto>();

            //WorkRelationship
            configuration.CreateMap<PlusDefinition, PlusDefinitionInfoDto>();
            configuration.CreateMap<EmploymentPlus, EmploymentPlusInfoDto>();

            configuration.CreateMap<EmploymentDocument, EmploymentDocumentInfoDto>();
            configuration.CreateMap<EmploymentDocument, LegalEmploymentDocumentDto>();
            configuration.CreateMap<EmploymentDocument, InnerPartEmploymentDocumentDto>();

            configuration.CreateMap<TemplateDocument, TemplateDocumentOutputDto>();
            configuration.CreateMap<Template, TemplateListDto>().ReverseMap();

            configuration.CreateMap<TemplateJobPosition, JobPositionListDto>().ReverseMap();
        }
    }
}
