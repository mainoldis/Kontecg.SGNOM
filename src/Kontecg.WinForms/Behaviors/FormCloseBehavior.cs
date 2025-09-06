using System.Windows.Forms;
using DevExpress.Utils.MVVM;
using Kontecg.Dependency;
using Kontecg.Localization;

namespace Kontecg.Behaviors
{
    public class FormCloseBehavior : ConfirmationBehavior<FormClosingEventArgs>, ITransientDependency
    {
        /// <inheritdoc />
        public FormCloseBehavior() 
            : base("FormClosing")
        {
        }

        /// <inheritdoc />
        protected override string GetConfirmationCaption()
        {
            return LocalizationHelper.GetString(KontecgWinFormsConsts.LocalizationSourceName, "ConfirmExitCaption");
        }

        /// <inheritdoc />
        protected override string GetConfirmationText()
        {
            return LocalizationHelper.GetString(KontecgWinFormsConsts.LocalizationSourceName, "ConfirmExitMessage");
        }
    }
}