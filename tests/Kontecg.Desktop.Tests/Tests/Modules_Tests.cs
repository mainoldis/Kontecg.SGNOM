using System.Collections.Generic;
using System.Linq;
using Kontecg.Modules;
using Xunit;
using Xunit.Abstractions;

namespace Kontecg.Desktop.Tests
{
    public class Modules_Tests : DesktopTestModuleTestBase
    {
        private readonly ITestOutputHelper _output;

        public Modules_Tests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Get_all_loaded_modules_Test()
        {
            var manager = LocalIocManager.Resolve<IKontecgModuleManager>();
            var moduleInfos = manager.Modules;

            WritetoOuput(moduleInfos.ToList(), 1);
        }

        private void WritetoOuput(IList<KontecgModuleInfo> modules, int level)
        {
            foreach (var moduleInfo in modules)
            {
                string separartor = "-";
                var fill = string.Empty;
                for (int i = 0; i < level; i++) fill += separartor;
                _output.WriteLine("|{0}>{1}, {2}", fill, moduleInfo.Name, moduleInfo.Version);
                WritetoOuput(moduleInfo.Dependencies, level + 1);
            }
        }
    }
}
