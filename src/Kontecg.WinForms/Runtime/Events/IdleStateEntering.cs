using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using Kontecg.Events.Bus;

namespace Kontecg.Runtime.Events
{
    public class IdleStateEntering : EventData
    {
        public IdleStateEntering(Control control, OverlayWindowOptions options)
        {
            Owner = control;
            Options = options;
        }

        public Control Owner { get; }

        public OverlayWindowOptions Options { get; }
    }
}
