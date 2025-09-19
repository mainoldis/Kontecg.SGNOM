using System;
using Kontecg.HumanResources.Dto;

namespace Kontecg.ViewModels.Persons
{
    public class PersonViewModel : SingleObjectViewModel<PersonDto, long>
    {
        /// <inheritdoc />
        public PersonViewModel() : base(null)
        {
        }

        public event EventHandler EntityChanged;

        /// <inheritdoc />
        protected override void OnEntityChanged()
        {
            base.OnEntityChanged();
            EntityChanged?.Invoke(this, EventArgs.Empty);
        }

        public PersonsCollectionViewModel ParentViewModel => ViewModelHelper.GetParentViewModel<PersonsCollectionViewModel>(this);
    }
}