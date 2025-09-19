using DevExpress.Mvvm;
using Kontecg.Domain;
using Kontecg.Runtime;
using Kontecg.Services.Forms;
using Kontecg.ViewModels;
using Kontecg.Views;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kontecg.Services
{
    public abstract class DetailFormDocumentManagerServiceBase : DocumentManagerServiceBase
    {
        private readonly string _moduleType;
        private readonly ViewCategory _viewCategory;

        public DetailFormDocumentManagerServiceBase(string moduleType, ViewCategory viewCategory)
        {
            _moduleType = moduleType;
            _viewCategory = viewCategory;
        }

        #region Document

        protected class DetailFormDocument : IDocument, IDocumentInfo
        {
            private readonly object _contentCore;
            private readonly Form _formCore;
            private readonly DetailFormDocumentManagerServiceBase _owner;
            private bool _destroyOnCloseCore = true;
            private DocumentState _docState = DocumentState.Hidden;

            public DetailFormDocument(DetailFormDocumentManagerServiceBase owner, Form form, object content)
            {
                _owner = owner;
                _formCore = form;
                _contentCore = content;
                form.AutoValidate = AutoValidate.EnableAllowFocusChange;
                form.Closing += Form_Closing;
                form.Closed += Form_Closed;
            }

            private void Form_Closing(object sender, CancelEventArgs e)
            {
                IDocumentContent documentContent = GetContent() as IDocumentContent;
                
                documentContent?.OnClose(e);

                if (!_destroyOnCloseCore)
                {
                    bool cancel = e.Cancel;
                    e.Cancel = true;
                    if (!cancel)
                        _formCore.Hide();
                }
            }

            private void Form_Closed(object sender, EventArgs e)
            {
                var formTitle = _formCore.Text;
                var parentViewModel = ViewModelHelper.GetParentViewModel<ISupportNewParent>(_contentCore);
                _owner.RemoveDocument(this);
                _formCore.Closing -= Form_Closing;
                _formCore.Closed -= Form_Closed;
                IDocumentContent documentContent = GetContent() as IDocumentContent;
                documentContent?.OnDestroy();
                TryUpdateNew(GetContent(), formTitle, parentViewModel);
            }

            void IDocument.Show()
            {
                if (!_formCore.Visible)
                    _formCore.Show(AppHelper.MainForm);
                else
                    _formCore.Activate();
                _docState = DocumentState.Visible;
                var parentViewModel = ViewModelHelper.GetParentViewModel<ISupportNewParent>(_contentCore);
                if (_contentCore is ISupportNewChild && parentViewModel != null)
                    UpdateNew(parentViewModel);
            }

            void IDocument.Hide()
            {
                _formCore.Hide();
                _docState = DocumentState.Hidden;
            }

            void IDocument.Close(bool force)
            {
                if (force)
                {
                    _formCore.Closing -= Form_Closing;
                    DevExpress.XtraEditors.Container.ContainerHelper.ClearUnvalidatedControl(_formCore.ActiveControl, _formCore);
                }
                _formCore.Close();
                _docState = DocumentState.Hidden;
            }

            private void TryUpdateNew(object content, string title, ISupportNewParent parentViewModel)
            {
                if (content is ISupportNewChild && parentViewModel != null)
                {
                    if (AppHelper.MainForm != null && title.EndsWith("(New)"))
                        AppHelper.MainForm.BeginInvoke(new Action<object>(UpdateNew), parentViewModel);
                }
            }

            private void UpdateNew(object parameter)
            {
                if (parameter is ISupportNewParent parentViewModel)
                    ViewModelHelper.RaiseCanExecuteChanged(parentViewModel, "New");
            }

            bool IDocument.DestroyOnClose
            {
                get => _destroyOnCloseCore;
                set => _destroyOnCloseCore = value;
            }

            object IDocument.Id { get; set; }

            object IDocument.Title
            {
                get => _formCore.Text;
                set => _formCore.Text = Convert.ToString(value) ?? string.Empty;
            }

            object IDocument.Content => GetContent();

            object GetContent()
            {
                return _contentCore;
            }

            DocumentState IDocumentInfo.State => _docState;

            string IDocumentInfo.DocumentType => null;
        }

        #endregion

        protected bool IsDefaultViewModuleType(string actualViewModuleType, ViewCategory viewCategory)
        {
            return _moduleType == actualViewModuleType && _viewCategory == viewCategory;
        }

        protected virtual Module GetActualViewModuleType(string documentType, object parentViewModel)
        {
            var moduleLocator = GetService<IModuleLocator>(parentViewModel);
            return moduleLocator.GetModuleType(documentType, _viewCategory);
        }

        protected IDocument RegisterDetailFormDocumentForModule(object viewModel, object parentViewModel, object parameter, Module actualModuleType)
        {
            var waitingService = GetService<IWaitingViewService>(parentViewModel);
            var container = new DetailForm();
            waitingService.BeginWaiting(container, parameter);
            var moduleLocator = GetService<IModuleLocator>(parentViewModel);
            object view = moduleLocator.GetModuleControl(actualModuleType, viewModel, parameter, _viewCategory);
            viewModel = EnsureViewModel(viewModel, parameter, parentViewModel, view);
            IDocument document = RegisterDocument(view, form => new DetailFormDocument(this, form, viewModel), () => container, parameter);
            waitingService.EndWaiting();
            return document;
        }
    }

    public class DetailFormDocumentManagerService : DetailFormDocumentManagerServiceBase, IDocumentManagerService
    {
        /// <inheritdoc />
        public DetailFormDocumentManagerService(string moduleType, ViewCategory viewCategory) 
            : base(moduleType, viewCategory)
        {
        }

        /// <inheritdoc />
        protected override IDocument CreateDocumentCore(string documentType, object viewModel, object parentViewModel, object parameter)
        {
            var actualModuleType = GetActualViewModuleType(documentType, parentViewModel);
            return RegisterDetailFormDocumentForModule(viewModel, parentViewModel, parameter, actualModuleType);
        }
    }
}