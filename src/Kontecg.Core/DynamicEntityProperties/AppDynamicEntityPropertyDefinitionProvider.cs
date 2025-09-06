using Kontecg.CustomInputTypes;
using Kontecg.HumanResources;
using Kontecg.UI.Inputs;
using Kontecg.Workflows;

namespace Kontecg.DynamicEntityProperties
{
    public class AppDynamicEntityPropertyDefinitionProvider : DynamicEntityPropertyDefinitionProvider
    {
        public override void SetDynamicEntityProperties(IDynamicEntityPropertyDefinitionContext context)
        {
            context.Manager.AddAllowedInputType<SingleLineStringInputType>();
            context.Manager.AddAllowedInputType<ComboboxInputType>();
            context.Manager.AddAllowedInputType<CheckboxInputType>();
            context.Manager.AddAllowedInputType<MultiSelectComboboxInputType>();

            //Add entities here 
            context.Manager.AddEntity<Person, long>();
            context.Manager.AddEntity<DocumentDefinition>();
        }
    }
}
