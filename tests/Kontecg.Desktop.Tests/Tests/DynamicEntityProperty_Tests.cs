using System.Collections.Generic;
using System.Linq;
using Kontecg.DynamicEntityProperties;
using Kontecg.DynamicEntityProperties.Dto;
using Kontecg.Workflows;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class DynamicEntityProperty_Tests : DesktopTestModuleTestBase
    {
        [Fact]
        public async void Get_dynamic_entity_property_Test()
        {
            KontecgSession.Use(1, 2);

            var dynamicPropertyAppService = LocalIocManager.Resolve<IDynamicPropertyAppService>();
            var dynamicEntityPropertyAppService = LocalIocManager.Resolve<IDynamicEntityPropertyAppService>();
            var dynamicEntityPropertyDefinitionManager =
                LocalIocManager.Resolve<IDynamicEntityPropertyDefinitionManager>();

            var model = new CreateEntityDynamicPropertyViewModel()
            {
                EntityFullName = typeof(DocumentDefinition).FullName
            };

            var allDynamicProperties = (await dynamicPropertyAppService.GetAllAsync()).Items.ToList();
            var definedPropertyIds =
                (await dynamicEntityPropertyAppService.GetAllPropertiesOfAnEntityAsync(
                    new DynamicEntityPropertyGetAllInput() {EntityFullName = model.EntityFullName }))
                .Items.Select(x => x.DynamicPropertyId).ToList();

            model.DynamicProperties = allDynamicProperties.Where(x => !definedPropertyIds.Contains(x.Id)).ToList();

            foreach (DynamicPropertyDto modelDynamicProperty in model.DynamicProperties)
            {
                var dnp = new DynamicEntityPropertyDto
                {
                    EntityFullName = model.EntityFullName,
                    CompanyId = modelDynamicProperty.CompanyId,
                    DynamicPropertyName = modelDynamicProperty.PropertyName,
                    DynamicPropertyId = modelDynamicProperty.Id
                };

                await dynamicEntityPropertyAppService.AddAsync(dnp);
            }
        }

        private class CreateEntityDynamicPropertyViewModel
        {
            public string EntityFullName { get; set; }

            public List<string> AllEntities { get; set; }

            public List<DynamicPropertyDto> DynamicProperties { get; set; }
        }

        private class DynamicEntityPropertyValueManageAllViewModel
        {
            public string EntityFullName { get; set; }

            public string EntityId { get; set; }
        }
    }
}
