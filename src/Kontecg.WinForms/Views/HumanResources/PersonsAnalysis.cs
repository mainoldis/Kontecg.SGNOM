using DevExpress.XtraBars.Ribbon;
using Kontecg.Domain;
using Kontecg.ViewModels.Persons;

namespace Kontecg.Views.HumanResources
{
    public partial class PersonsAnalysis : BaseUserControl, IRibbonOwner
    {
        public PersonsAnalysis()
            :base(typeof(HumanResourcesAnalysisViewModel))
        {
            InitializeComponent();
            BindCommands();
            LoadTemplate();
        }

        public HumanResourcesAnalysisViewModel ViewModel => GetViewModel<HumanResourcesAnalysisViewModel>();

        public PersonsCollectionViewModel CollectionViewModel => GetParentViewModel<PersonsCollectionViewModel>();

        /// <inheritdoc />
        protected override void OnParentViewModelAttached()
        {
            base.OnParentViewModelAttached();
            LoadAnalysisData();
        }
        private void BindCommands()
        {
            biClose.BindCommand(() => ViewModel.Close(), ViewModel);
        }

        private void LoadTemplate()
        {

        }

        private void LoadAnalysisData()
        {
            spreadsheetControl.Document.BeginUpdate();

            var sheet = spreadsheetControl.Document.Worksheets.Contains("Etnia");

            var items = ViewModel.GetOldPeople();

            spreadsheetControl.Document.EndUpdate();
        }

        /// <inheritdoc />
        public RibbonControl Ribbon => ribbonControl;
    }
}
