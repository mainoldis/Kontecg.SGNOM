using DevExpress.Mvvm;

namespace Kontecg.ViewModels.Persons
{
    public class PersonViewModel : KontecgViewModelBase, ISupportParentViewModel, ISupportParameter
    {

        public PersonsCollectionViewModel ParentViewModel => ViewModelHelper.GetParentViewModel<PersonsCollectionViewModel>(this);

        /// <inheritdoc />
        object ISupportParentViewModel.ParentViewModel { get; set; }

        /// <inheritdoc />
        public virtual object Parameter { get; set; }
    }
}