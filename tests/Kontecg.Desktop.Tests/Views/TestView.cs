using DevExpress.Utils.MVVM;
using Kontecg.Domain;
using Kontecg.Views;

namespace Kontecg.Desktop.Views
{
    public partial class TestView : BaseUserControl, IMustHaveContext
    {
        public TestView()
        {
            InitializeComponent();
            Context = new MVVMContext(components);
            Context.ViewModelType = typeof(TestViewModel);

            Context.BindCommand<TestViewModel>(simpleButton1, model => model.Click());
        }

        /// <inheritdoc />
        public MVVMContext Context { get; set; }
    }
}
