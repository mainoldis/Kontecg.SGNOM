using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Kontecg.Runtime;
using Kontecg.ViewModels;
using DevExpress.Mvvm.POCO;
using Kontecg.Dependency;

namespace Kontecg.Services
{
    public abstract class DocumentManagerServiceBase : IDocumentManagerService, IDocumentOwner
    {
        private readonly IList<IDocument> _documentsCore = new List<IDocument>();

        protected IDocument RegisterDocument(object view, Func<Form, IDocument> createDocument, Func<Form> createContainer, object id = null)
        {
            Form container = null;
            if (createContainer != null)
            {
                container = createContainer();
                if (container != null)
                    container.Owner = AppHelper.MainForm;
            }

            IDocument document = createDocument(container);

            document.Id = id;
            _documentsCore.Add(document);

            if (view != null && container != null)
            {
                container.ClientSize = ((Control)view).Size;
                ((Control)view).Dock = DockStyle.Fill;
                ((Control)view).Parent = container;
                ((Control)view).BringToFront();
            }

            return document;
        }

        protected object EnsureViewModel(object viewModel, object parameter, object parentViewModel, object view)
        {
            if (viewModel == null)
            {
                if (view is ISupportViewModel supportViewModel)
                    viewModel = supportViewModel.ViewModel;
                ViewModelHelper.EnsureModuleViewModel(view, parentViewModel, parameter);
            }

            if (viewModel is IDocumentContent documentContent)
                documentContent.DocumentOwner = this;

            return viewModel;
        }

        protected void RemoveDocument(IDocument document)
        {
            _documentsCore.Remove(document);
        }

        protected TService GetService<TService>(object viewModel) where TService : class
        {
            var serviceContainer = GetServiceContainer(viewModel);
            if (serviceContainer != null && serviceContainer.IsRegistered<TService>())
                return serviceContainer.Resolve<TService>();

            var secondServiceContainer = GetServiceContainerFromAnother(viewModel);
            return secondServiceContainer != null
                ? serviceContainer.GetService<TService>()
                : null;
        }

        private IIocManager GetServiceContainer(object viewModel)
        {
            return viewModel is not IIocManagerAccessor accessor ? null : accessor.IocManager;
        }

        private IServiceContainer GetServiceContainerFromAnother(object viewModel)
        {
            return viewModel is not ISupportServices services ? null : services.ServiceContainer;
        }

        protected abstract IDocument CreateDocumentCore(string documentType, object viewModel, object parentViewModel, object parameter);

        #region IDocumentManagerService

        IDocument IDocumentManagerService.CreateDocument(string documentType, object viewModel, object parameter, object parentViewModel)
        {
            return CreateDocumentCore(documentType, viewModel, parentViewModel, parameter);
        }

        IEnumerable<IDocument> IDocumentManagerService.Documents => _documentsCore;

        IDocument IDocumentManagerService.ActiveDocument
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        event ActiveDocumentChangedEventHandler IDocumentManagerService.ActiveDocumentChanged
        {
            add { }
            remove { }
        }

        void IDocumentOwner.Close(IDocumentContent documentContent, bool force)
        {
            var document = _documentsCore.FirstOrDefault((d) => Equals(d.Content, documentContent));
            document?.Close(force);
        }

        #endregion IDocumentManagerService
    }
}