using System.Windows.Forms;
using DevExpress.Utils.MVVM;
using Kontecg.Dependency;
using Kontecg.Services;

namespace Kontecg.Behaviors
{
    public class ControlCBehavior : EventTriggerBase<KeyEventArgs>, ITransientDependency
    {
        /// <inheritdoc />
        public ControlCBehavior() 
            : base("KeyDown")
        {
        }

        /// <inheritdoc />
        protected override void OnEvent()
        {
            if (Args.Control && Args.KeyCode == Keys.C)
            {
                var service = this.GetService<IClipboardService>();

            }
        }
    }
}