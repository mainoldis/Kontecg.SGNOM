using Elsa.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Kontecg.DependencyInjection
{
    public static class WorkflowRegistrar
    {
        public static void Register(IServiceCollection services)
        {
            services.AddElsa(builder =>
            {
                builder.UseWorkflowManagement();
                builder.UseWorkflowRuntime();
                builder.UseWorkflows();
                builder.UseWorkflowsApi();
            });
        }
    }
}
