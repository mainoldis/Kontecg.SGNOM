using DevExpress.Mvvm;
using Kontecg.Dependency;
using Kontecg.Services.Forms;
using Kontecg.Views;

namespace Kontecg.Services
{
    public class FilterDialogDocumentManagerService : DialogDocumentManagerService, ITransientDependency
    {
        private readonly Module _moduleType;
        private readonly ViewCategory _viewCategory;

        /// <inheritdoc />
        public FilterDialogDocumentManagerService(Module moduleType, ViewCategory viewCategory)
        {
            _moduleType = moduleType;
            _viewCategory = viewCategory;
        }

        /// <inheritdoc />
        protected override IDocument CreateDocumentCore(string documentType, object viewModel, object parentViewModel, object parameter)
        {
            var moduleLocator = GetService<IModuleLocator>(parentViewModel);
            object view = moduleLocator.GetModuleControl(_moduleType, viewModel, parameter, _viewCategory);
            return RegisterDocument(view,
                (form) => new DialogDocument(this, form, viewModel),
                () => new FilterForm() { Text = documentType });
        }
    }
}