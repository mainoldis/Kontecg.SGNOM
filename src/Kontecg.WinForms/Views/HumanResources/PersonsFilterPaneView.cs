using Kontecg.Domain;

namespace Kontecg.Views.HumanResources
{
    public partial class PersonsFilterPaneView : BaseUserControl, ISupportCompactLayout
    {
        public PersonsFilterPaneView()
        {
            InitializeComponent();
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
        void UpdateCompactLayout()
        {
            //btnNewCustomerLayoutControlItem.Visibility = _compactLayout ? LayoutVisibility.Never : LayoutVisibility.Always;
        }

        #endregion
    }
}
