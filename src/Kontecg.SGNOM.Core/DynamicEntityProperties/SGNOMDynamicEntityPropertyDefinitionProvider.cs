using Kontecg.Salary;

namespace Kontecg.DynamicEntityProperties
{
    public class SGNOMDynamicEntityPropertyDefinitionProvider : DynamicEntityPropertyDefinitionProvider
    {
        public override void SetDynamicEntityProperties(IDynamicEntityPropertyDefinitionContext context)
        {
            //Add entities here 
            context.Manager.AddEntity<PaymentDefinition>();
        }
    }
}
