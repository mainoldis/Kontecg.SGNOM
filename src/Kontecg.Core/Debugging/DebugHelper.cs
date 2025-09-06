using System.Diagnostics;
using Castle.MicroKernel;
using Castle.Windsor.Diagnostics;
using Kontecg.Dependency;

namespace Kontecg.Debugging
{
    public static class DebugHelper
    {
        public static bool IsDebug
        {
            get
            {
#pragma warning disable
#if DEBUG
                return Debugger.IsAttached;
#endif
                return false;
#pragma warning restore
            }
        }

        public static void DebugInfo()
        {
            var host = (IDiagnosticsHost)IocManager.Instance.IocContainer.Kernel.GetSubSystem(SubSystemConstants.DiagnosticsKey);
            var diagnostics = host.GetDiagnostic<IPotentialLifestyleMismatchesDiagnostic>();
            var misconfiguredHandlers = diagnostics.Inspect();
        }
    }
}
