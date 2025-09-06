using DevExpress.Mvvm;
using Kontecg.Dependency;
using Kontecg.Services;
using Kontecg.ViewModels.Account;
using System.Windows.Forms;
using Kontecg.Localization;
using Kontecg.Services.Forms;

namespace Kontecg.Views.Account
{
    public class LoginDialogDocumentManagerService : DialogDocumentManagerService, ITransientDependency
    {
        /// <inheritdoc />
        protected override IDocument CreateDocumentCore(string documentType, object viewModel, object parentViewModel, object parameter)
        {
            var view = IocManager.Instance.Resolve<LoginView>();
            viewModel = EnsureViewModel(view.ViewModel, parameter, parentViewModel, view);

            return RegisterDocument(view, form => new AccountDialogDocument(this, form, viewModel),
                () => new DialogForm {Text = $@"KONTECG - {LocalizationHelper.GetString(KontecgWinFormsConsts.LocalizationSourceName, documentType + "_Title")}"});
        }

        #region Document

        private class AccountDialogDocument : DialogDocument
        {
            public AccountDialogDocument(LoginDialogDocumentManagerService owner, Form form, object content)
                : base(owner, form, content)
            {
                if (content is LoginViewModel viewModel) viewModel.Document = this;
            }
        }

        #endregion Document
    }
}