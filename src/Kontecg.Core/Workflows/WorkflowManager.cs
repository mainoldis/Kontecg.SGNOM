using System;
using System.Threading.Tasks;
using Elsa.Workflows;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Parameters;
using Kontecg.Events.Bus;
using Kontecg.Events.Bus.Exceptions;

namespace Kontecg.Workflows
{
    public class WorkflowManager : KontecgCoreDomainServiceBase, IWorkflowManager
    {
        public WorkflowManager(
            IWorkflowRuntime workflowRuntime,
            IWorkflowRunner workflowRunner)
        {
            Runtime = workflowRuntime;
            Runner = workflowRunner;

            EventBus = NullEventBus.Instance;
        }

        public IEventBus EventBus { get; set; }

        public async Task ProcessAsync(string definitionId, StartWorkflowRuntimeParams options)
        {
            try
            {
                var result = await Runtime.TryStartWorkflowAsync(definitionId, options);


            }
            catch (KontecgException ex)
            {
                await EventBus.TriggerAsync(this, new KontecgHandledExceptionData(ex));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex.InnerException  ?? ex);
            }
        }

        public IWorkflowRuntime Runtime { get; }

        public IWorkflowRunner Runner { get; }
    }
}
