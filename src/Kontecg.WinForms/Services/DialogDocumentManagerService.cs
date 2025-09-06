using System.Windows.Forms;
using System;
using DevExpress.Mvvm;

namespace Kontecg.Services
{
    public abstract class DialogDocumentManagerService : DocumentManagerServiceBase
    {
        #region Document

        protected class DialogDocument : IDocument, IDocumentInfo
        {
            private readonly object _contentCore;
            private readonly Form _formCore;
            private readonly DialogDocumentManagerService _owner;
            private DocumentState _docState = DocumentState.Hidden;
            private DialogResult _dialogResult = DialogResult.None;

            public DialogDocument(DialogDocumentManagerService owner, Form dialogForm, object content)
            {
                _owner = owner;
                _formCore = dialogForm;
                _contentCore = content;

                dialogForm.AutoValidate = AutoValidate.EnableAllowFocusChange;
                dialogForm.Closed += Form_Closed;
            }

            private void Form_Closed(object sender, EventArgs e)
            {
                _owner.RemoveDocument(this);
                _formCore.Closed -= Form_Closed;
            }

            void IDocument.Show()
            {
                using (_formCore)
                {
                    _dialogResult = _formCore.ShowDialog();
                }
                _docState = DocumentState.Visible;
            }

            void IDocument.Hide()
            {
                _formCore.Close();
                _docState = DocumentState.Hidden;
            }

            void IDocument.Close(bool force)
            {
                _formCore.Close();
                _docState = DocumentState.Hidden;
            }

            bool IDocument.DestroyOnClose
            {
                get => true;
                set { }
            }

            object IDocument.Id { get; set; }

            object IDocument.Title
            {
                get => _formCore.Text;
                set => _formCore.Text = Convert.ToString(value) ?? string.Empty;
            }

            object IDocument.Content => _contentCore;

            DocumentState IDocumentInfo.State => _docState;

            string IDocumentInfo.DocumentType => null;

            DialogResult DialogResult => _dialogResult;
        }

        #endregion Document
    }
}