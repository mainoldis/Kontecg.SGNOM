using System.ComponentModel;
using DevExpress.Mvvm;

namespace Kontecg.ViewModels.OrganizationUnits
{
    public class WorkPlaceEditViewModel : KontecgViewModelBase, ISupportParentViewModel, IDocumentContent
    {
        /// <inheritdoc />
        public object ParentViewModel { get; set; }

        /// <inheritdoc />
        public void OnClose(CancelEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void OnDestroy()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDocumentOwner DocumentOwner { get; set; }

        /// <inheritdoc />
        public object Title { get; }
    }
}