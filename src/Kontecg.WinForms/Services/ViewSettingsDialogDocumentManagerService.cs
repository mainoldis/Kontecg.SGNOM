using DevExpress.Mvvm;
using Kontecg.Dependency;
using Kontecg.ViewModels;
using System;
using System.Windows.Forms;
using Kontecg.Services.Forms;
using Kontecg.ViewModels.Shared;
using Kontecg.Views.Shared;

namespace Kontecg.Services
{
    public class ViewSettingsDialogDocumentManagerService : DialogDocumentManagerService, ITransientDependency
    {
        private readonly Func<CollectionUiViewModel> _collectionUiViewModelAccessor;

        /// <inheritdoc />
        public ViewSettingsDialogDocumentManagerService(Func<CollectionUiViewModel> collectionUiViewModelAccessor)
        {
            _collectionUiViewModelAccessor = collectionUiViewModelAccessor;
        }

        protected override IDocument CreateDocumentCore(string documentType, object viewModel, object parentViewModel, object parameter)
        {
            object view = new ViewSettingsControl(_collectionUiViewModelAccessor());
            viewModel = EnsureViewModel(viewModel, parameter, parentViewModel, view);
            return RegisterDocument(view,
                (form) => new ViewSettingsDialogDocument(this, form, viewModel),
                () => new FilterForm { Text = documentType });
        }

        #region Document

        private class ViewSettingsDialogDocument : DialogDocument
        {
            public ViewSettingsDialogDocument(ViewSettingsDialogDocumentManagerService owner, Form form, object content)
                : base(owner, form, content)
            {
                if (content is ViewSettingsViewModel viewModel) viewModel.Document = this;
            }
        }

        #endregion Document
    }
}