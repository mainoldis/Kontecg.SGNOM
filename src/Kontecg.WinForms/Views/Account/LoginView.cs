using System;
using DevExpress.Utils.MVVM.Services;
using DevExpress.XtraEditors.Controls;
using Kontecg.Authorization.Users;
using Kontecg.Extensions;
using Kontecg.ViewModels.Account;

namespace Kontecg.Views.Account
{
    public partial class LoginView : BaseUserControl
    {
        public LoginView() 
            : base(typeof(LoginViewModel))
        {
            InitializeComponent();

            if (Context.IsDesignMode) return;
            BindCommands();
        }

        /// <inheritdoc />
        protected override void OnInitServices()
        {
            Context.RegisterService(this);
            Context.RegisterService(MessageBoxService.CreateXtraMessageBoxService(GuessOwner() ?? this.ParentForm));
        }

        private void BindCommands()
        {
            var bindings = Context.OfType<LoginViewModel>();
            bindings.SetBinding(txtUsernameOrEmail, edit => edit.Text, x => x.UsernameOrEmail);
            bindings.SetBinding(txtPassword, edit => edit.Text, x => x.Password);
            bindings.SetBinding(cboDomainName, edit => edit.Text, x => x.Company);
            bindings.SetBinding(chkRememberMe, edit => edit.EditValue, x => x.RememberMe);
            
            bindings
                .WithCommand(x => x.LoginAsync)
                .Bind(btnSignIn)
                .BindCancel(btnSignIn);

            bindings.SetTrigger(x => x.IsLogin, InitializeUI);
            InitializeUI(bindings.ViewModel.IsLogin);
            Remember();
        }

        protected override void LocalizeIsolatedItems()
        {
            base.LocalizeIsolatedItems();
            lciUsernameOrEmail.Text = L(lciUsernameOrEmail.Text);
            lciUsernameOrEmail.CustomizationFormText = L(lciUsernameOrEmail.CustomizationFormText);
            lciPassword.Text = L(lciPassword.Text);
            lciPassword.CustomizationFormText = L(lciPassword.CustomizationFormText);
            lciDomainName.Text = L(lciDomainName.Text);
            lciDomainName.CustomizationFormText = L(lciDomainName.CustomizationFormText);
            lciRememberMe.Text = L(lciRememberMe.Text);
            lciRememberMe.CustomizationFormText = L(lciRememberMe.CustomizationFormText);
            btnSignIn.Text = L(btnSignIn.Text);

            var itemForEdit = moduleLayout.GetItemByControl(txtUsernameOrEmail);
            itemForEdit.AllowHtmlStringInCaption = true;
            itemForEdit.Text += @" <color=red>*</color>";

            itemForEdit = moduleLayout.GetItemByControl(txtPassword);
            itemForEdit.AllowHtmlStringInCaption = true;
            itemForEdit.Text += @" <color=red>*</color>";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var companies = ViewModel.Companies;
            foreach (var t in companies.Items) cboDomainName.Properties.Items.Add(new ComboBoxItem(t.DisplayText));

            moduleLayout.BeginInvoke(new Action(() =>
            {
                if (txtUsernameOrEmail.Text.IsNullOrWhiteSpace()) txtUsernameOrEmail.Focus();
                else txtPassword.Focus();
            }));
        }

        private void InitializeUI(bool isLogin)
        {
            txtUsernameOrEmail.Enabled = !isLogin;
            txtPassword.Enabled = !isLogin;
            cboDomainName.Enabled = !isLogin;
            chkRememberMe.Enabled = !isLogin;
            btnSignIn.Enabled = !isLogin;
        }

        private void Remember()
        {
            if (ViewModel.CheckRememberUsernameOrEmail() && ViewModel.RememberMe)
            {
                ViewModel.UsernameOrEmail = KontecgUserBase.AdminUserName;
            }
        }

        public LoginViewModel ViewModel => Context.GetViewModel<LoginViewModel>();
    }
}
