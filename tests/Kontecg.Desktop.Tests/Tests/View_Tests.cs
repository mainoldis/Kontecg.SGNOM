using Kontecg.ExceptionHandling;
using Kontecg.Presenters;
using Kontecg.Services;
using Shouldly;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class View_Tests : DesktopTestModuleTestBase
    {
        [Fact]
        public void Grab_snapshots_Test()
        {
            KontecgSession.Use(1, 2);
            var snapshooter = Resolve<SnapshotManager>();
            snapshooter.GrabSnapshot();
        }

        [Fact]
        public void Get_one_view_Test()
        {
            KontecgSession.Use(null, 1);
            var viewLocator = Resolve<IModuleLocator>();

        }

        [Fact]
        public void Get_screen_Test()
        {
            KontecgSession.Use(null, 1);
            
        }
    }
}