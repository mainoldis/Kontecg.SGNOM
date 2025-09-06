using System.Threading.Tasks;
using Elsa.Workflows;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Parameters;

namespace Kontecg.Workflows
{
    public interface IWorkflowManager
    {
        Task ProcessAsync(string definitionId, StartWorkflowRuntimeParams options);

        IWorkflowRuntime Runtime { get; }

        IWorkflowRunner Runner { get; }
    }
}
