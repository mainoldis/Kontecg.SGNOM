using Kontecg.ViewModels.Persons;

namespace Kontecg.Views.HumanResources
{
    public partial class PersonDetailView : BaseUserControl
    {
        public PersonDetailView()
        {
            InitializeComponent();

            var vm = ViewModel;
        }

        public PersonViewModel ViewModel => GetViewModel<PersonViewModel>();

        //public bool IsHorizontalLayout
        //{
        //    get { return winExplorerView.OptionsView.Style == WinExplorerViewStyle.Large; }
        //    set
        //    {
        //        winExplorerView.OptionsView.Style = value ?
        //            WinExplorerViewStyle.Large : WinExplorerViewStyle.Medium;
        //    }
        //}

        public bool IsHorizontalLayout { get; set; }
    }
}
