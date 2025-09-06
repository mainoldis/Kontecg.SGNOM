using DevExpress.Mvvm;
using System;

namespace Kontecg.ViewModels
{
#if false
    public abstract class ReportViewModelBase
    {
        protected internal abstract bool IsLoaded { get; }

        protected internal abstract void OnReload();
    }

    public abstract class ReportViewModelBase<TReportType> : ReportViewModelBase, ISupportParameter
        where TReportType : struct
    {
        public virtual TReportType ReportType { get; set; }

        protected virtual void OnReportTypeChanged()
        {
            if (IsLoaded) RaiseReportTypeChanged();
        }

        public event EventHandler ReportTypeChanged;

        private void RaiseReportTypeChanged()
        {
            ReportTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        #region ISupportParameter

        object ISupportParameter.Parameter
        {
            get => ReportType;
            set
            {
                ReportType = (TReportType)value;
                OnParameterChanged();
            }
        }

        protected virtual void OnParameterChanged() { }

        #endregion
    }

    public class ReportViewModelBase<TReportType, TEntity, TPrimaryKey> : ReportViewModelBase<TReportType>
        where TReportType : struct
        where TEntity : class
    {
        protected override void OnParameterChanged()
        {
            base.OnParameterChanged();
            CheckReportEntityKey();
        }

        public virtual object ReportEntityKey { get; set; }

        protected virtual void OnReportEntityKeyChanged()
        {
            RaiseReportEntityKeyChanged();
        }

        public event EventHandler ReportEntityKeyChanged;

        protected internal override void OnReload()
        {
            if (!IsLoaded) return;
            CheckReportEntityKey();
            RaiseReload();
        }

        public event EventHandler Reload;

        private bool _isLoadedCore;

        protected internal override bool IsLoaded => _isLoadedCore;

        public void OnLoad()
        {
            CheckReportEntityKey();
            _isLoadedCore = true;
        }

        private void CheckReportEntityKey()
        {
            var viewModel = GetCollectionViewModel();
            if (viewModel != null)
                ReportEntityKey = viewModel.SelectedEntityKey;
        }

        protected CollectionViewModel<TEntity, TPrimaryKey> GetCollectionViewModel()
        {
            ISupportParentViewModel supportParent = this as ISupportParentViewModel;
            if (supportParent != null)
                return supportParent.ParentViewModel as CollectionViewModel<TEntity, TPrimaryKey>;
            return null;
        }

        private void RaiseReportEntityKeyChanged()
        {
            ReportEntityKeyChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseReload()
        {
            Reload?.Invoke(this, EventArgs.Empty);
        }
    }
#endif
}