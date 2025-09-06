using Elsa.Extensions;
using Elsa.Workflows;
using Kontecg.Workflows;

namespace Kontecg.Accounting.Workflows
{
    public class SummarizeDocumentWorkflow : WorkflowBase
    {
        public SummarizeDocumentWorkflow()
        {
        }

        protected override void Build(IWorkflowBuilder builder)
        {
            var docType = builder.WithVariable<DocumentDefinition>("DOCTYPE", null).WithMemoryStorage();
        }
    }
}
