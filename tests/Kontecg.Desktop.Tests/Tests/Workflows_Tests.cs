using System.Threading.Tasks;
using Elsa.Workflows;
using Elsa.Workflows.Builders;
using Elsa.Workflows.Options;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class Workflows_Tests : DesktopTestModuleTestBase
    {
        public Workflows_Tests()
        {
            var admin = GetDefaultCompanyAdmin();
            KontecgSession.UserId = admin.Id;
            KontecgSession.CompanyId = admin.CompanyId;
        }

        [Fact]
        public async Task Simple_execution_activity_Test()
        {
            var runner = Resolve<IWorkflowRunner>();
        }
    }
}
