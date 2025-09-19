using System;
using Kontecg.HumanResources.Dto;
using Kontecg.ViewModels.Persons;

namespace Kontecg.Views.HumanResources
{
    public partial class PersonDetailView : BaseUserControl
    {
        public PersonDetailView()
            :base(typeof(PersonViewModel))
        {
            InitializeComponent();
            ViewModel.EntityChanged += ViewModelOnEntityChanged;
            //ItemForHomeOffice.AppearanceItemCaption.ForeColor = ColorHelper.DisabledTextColor;
            //ItemForHomeOffice.AppearanceItemCaption.Options.UseForeColor = true;
        }

        /// <inheritdoc />
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateUi(ViewModel.Entity);
        }

        public PersonViewModel ViewModel => GetViewModel<PersonViewModel>();

        protected override void OnMVVMContextReleasing()
        {
            ViewModel.EntityChanged -= ViewModelOnEntityChanged;
        }

        public bool IsHorizontalLayout { get; set; }

        private void ViewModelOnEntityChanged(object sender, EventArgs e)
        {
            UpdateUi(ViewModel.Entity);
        }

        private void UpdateUi(PersonDto person)
        {
            if (person != null)
            {
                if (!object.Equals(bindingSource.DataSource, person))
                    bindingSource.DataSource = person;
                else
                    bindingSource.ResetBindings(false);

                //Update other controls here
            }
            moduleLayout.Visible = (person != null);
        }
    }
}
