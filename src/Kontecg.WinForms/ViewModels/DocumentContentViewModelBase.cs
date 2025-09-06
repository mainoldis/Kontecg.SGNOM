using System.ComponentModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

namespace Kontecg.ViewModels
{
    public abstract class DocumentContentViewModelBase : KontecgViewModelBase, IDocumentContent
    {
        [Command]
        public void Close()
        {
            ((IDocumentContent)this).DocumentOwner.Close(this);
        }

        /// <inheritdoc />
        public virtual void OnClose(CancelEventArgs e)
        {
        }

        /// <inheritdoc />
        public virtual void OnDestroy()
        {
        }

        /// <inheritdoc />
        public IDocumentOwner DocumentOwner { get; set; }

        /// <inheritdoc />
        public object Title => GetTitle();

        protected virtual string GetTitle()
        {
            return null;
        }
    }
}