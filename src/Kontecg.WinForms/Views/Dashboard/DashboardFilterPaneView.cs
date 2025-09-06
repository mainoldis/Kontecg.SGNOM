using Kontecg.Domain;
using Kontecg.Services;

namespace Kontecg.Views.Dashboard
{
    public partial class DashboardFilterPaneView : BaseUserControl, ISupportCompactLayout
    {
        public DashboardFilterPaneView()
        {
            InitializeComponent();
        }

        protected override void OnDisposing()
        {
            //Presenter.Dispose();
            base.OnDisposing();
        }

        /// <inheritdoc />
        protected override void OnInitServices()
        {
            //Context.RegisterService("Custom Filter", new FilterDialogDocumentManagerService(ModuleType.CustomersCustomFilter));
            //Context.RegisterService("Group Filter", new FilterDialogDocumentManagerService(ModuleType.CustomersGroupFilter));
        }

        #region ISupportCompactLayout Members

        private bool _compactLayout = true;

        bool ISupportCompactLayout.Compact
        {
            get => _compactLayout;
            set
            {
                if (_compactLayout == value) return;
                _compactLayout = value;
                UpdateCompactLayout();
            }
        }

        private void UpdateCompactLayout()
        {
            //btnNewCustomerLayoutControlItem.Visibility = _compactLayout ? LayoutVisibility.Never : LayoutVisibility.Always;
        }

        #endregion
    }
}
