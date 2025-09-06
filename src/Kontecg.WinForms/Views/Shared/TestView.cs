using DevExpress.Utils.MVVM;
using Kontecg.Domain;
using Kontecg.ViewModels.Shared;

namespace Kontecg.Views.Shared
{
    public partial class TestView : BaseUserControl, IMustHaveContext
    {
        public TestView()
        {
            InitializeComponent();

            Context = new MVVMContext(components);
            Context.ContainerControl = this;

            Context.ViewModelType = typeof(TestViewModel);

            var bindings = Context.OfType<TestViewModel>();
            bindings.WithCommand(x => x.ClickAsync)
                    .Bind(okButton)
                    .BindCancel(cancelButton);

            bindings.WithCommand(x => x.Grab).Bind(grabButton);

            // One-way binding for displaying progress
            bindings.SetBinding(progressBar, p => p.EditValue, x => x.Progress);
            bindings.SetBinding(picture, p => p.EditValue, x => x.Snapshot);
        }

        /// <inheritdoc />
        public MVVMContext Context { get; set; }

        private void OnDisposing()
        {
            var context = MVVMContext.FromControl(this);
            if (context != null) context.Dispose();
        }
    }
}
