using System;
using DevExpress.Mvvm;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Dto;
using System.Collections.ObjectModel;
using System.Threading;
using Kontecg.Threading;
using System.Collections.Generic;
using DevExpress.Mvvm.DataAnnotations;
using System.Threading.Tasks;
using DevExpress.Mvvm.POCO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Kontecg.ViewModels
{
    #region With CRUD Service

    public abstract class EntitiesViewModel<TEntityDto, TService>
        : EntitiesViewModel<TEntityDto, int, TService>
        where TEntityDto : EntityDto
        where TService : ICrudAppService<TEntityDto, int, PagedSortedAndFilteredInputDto, TEntityDto, TEntityDto, EntityDto<int>, EntityDto<int>>
    {
        /// <inheritdoc />
        protected EntitiesViewModel(TService service, 
            ViewModelConfiguration configuration)
            : base(service, configuration)
        {
        }
    }

    public abstract class EntitiesViewModel<TEntityDto, TPrimaryKey, TService>
        : EntitiesViewModel<TEntityDto, TPrimaryKey, PagedSortedAndFilteredInputDto, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>, TService>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TService : ICrudAppService<TEntityDto, TPrimaryKey, PagedSortedAndFilteredInputDto, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>>
    {
        /// <inheritdoc />
        protected EntitiesViewModel(TService service, 
            ViewModelConfiguration configuration)
            : base(service, configuration)
        {
        }
    }

    public abstract class EntitiesViewModel<TEntityDto, TPrimaryKey, TGetAllInput, TService>
        : EntitiesViewModel<TEntityDto, TPrimaryKey, TGetAllInput, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>, TService>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedResultRequest, ISortedResultRequest, new()
        where TService : ICrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>>
    {
        /// <inheritdoc />
        protected EntitiesViewModel(TService service, 
            ViewModelConfiguration configuration) 
            : base(service, configuration)
        {
        }
    }

    public abstract class EntitiesViewModel<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput, TService>
        : EntitiesViewModel<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedResultRequest, ISortedResultRequest, new()
        where TCreateInput : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>, new()
        where TDeleteInput : IEntityDto<TPrimaryKey>
        where TService : ICrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
    {
        /// <inheritdoc />
        protected EntitiesViewModel(TService service, 
            ViewModelConfiguration configuration)
            :base(configuration)
        {
            Service = service;
        }

        protected TService Service { get; private set; }

        /// <inheritdoc />
        protected override bool HasService => Service != null;
    }

    #endregion

    #region With Aync CRUD Service

    public abstract class AsyncEntitiesViewModel<TEntityDto, TService>
        : AsyncEntitiesViewModel<TEntityDto, int, TService>
        where TEntityDto : EntityDto
        where TService : IAsyncCrudAppService<TEntityDto, int, PagedSortedAndFilteredInputDto, TEntityDto, TEntityDto, EntityDto<int>, EntityDto<int>>
    {
        /// <inheritdoc />
        protected AsyncEntitiesViewModel(TService service,
            ViewModelConfiguration configuration)
            : base(service, configuration)
        {
        }
    }

    public abstract class AsyncEntitiesViewModel<TEntityDto, TPrimaryKey, TService>
        : AsyncEntitiesViewModel<TEntityDto, TPrimaryKey, PagedSortedAndFilteredInputDto, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>, TService>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TService : IAsyncCrudAppService<TEntityDto, TPrimaryKey, PagedSortedAndFilteredInputDto, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>>
    {
        /// <inheritdoc />
        protected AsyncEntitiesViewModel(TService service, 
            ViewModelConfiguration configuration)
            : base(service, configuration)
        {
        }
    }

    public abstract class AsyncEntitiesViewModel<TEntityDto, TPrimaryKey, TGetAllInput, TService>
        : AsyncEntitiesViewModel<TEntityDto, TPrimaryKey, TGetAllInput, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>, TService>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedResultRequest, ISortedResultRequest, new()
        where TService : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>>
    {
        /// <inheritdoc />
        protected AsyncEntitiesViewModel(TService service, 
            ViewModelConfiguration configuration)
            : base(service, configuration)
        {
        }
    }

    public abstract class AsyncEntitiesViewModel<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput, TService>
        : EntitiesViewModel<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedResultRequest, ISortedResultRequest, new()
        where TCreateInput : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>, new()
        where TDeleteInput : IEntityDto<TPrimaryKey>
        where TService : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
    {
        /// <inheritdoc />
        protected AsyncEntitiesViewModel(TService service, 
            ViewModelConfiguration configuration)
            : base(configuration)
        {
            Service = service;
        }

        protected TService Service { get; private set; }

        /// <inheritdoc />
        protected override bool HasService => Service != null;
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntityDto">The DTO type for the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    /// <typeparam name="TGetAllInput">The type of the input for getting all entities.</typeparam>
    /// <typeparam name="TCreateInput">The type of the input for creating an entity.</typeparam>
    /// <typeparam name="TUpdateInput">The type of the input for updating an entity.</typeparam>
    /// <typeparam name="TGetInput">The type of the input for getting a single entity.</typeparam>
    /// <typeparam name="TDeleteInput">The type of the input for deleting an entity.</typeparam>
    [POCOViewModel]
    public abstract class EntitiesViewModel<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
        : KontecgViewModelBase, IEntitiesViewModel<TEntityDto, TPrimaryKey>, IDocumentContent, ISupportParentViewModel
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedResultRequest, ISortedResultRequest, new()
        where TCreateInput : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>, new()
        where TDeleteInput : IEntityDto<TPrimaryKey>
    {
        private readonly ICancellationTokenProvider _cancellationTokenProvider;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ObservableCollection<TEntityDto> _entities;
        private bool _isInitialized;

        /// <inheritdoc />
        protected EntitiesViewModel(ViewModelConfiguration configuration)
        {
            Configuration = configuration ?? new ViewModelConfiguration();
            Statistics = new ViewModelStatistics();
            _entities = new ObservableCollection<TEntityDto>();
            _isInitialized = false;
            _cancellationTokenSource = new CancellationTokenSource(Configuration.OperationTimeoutMs);
            LocalizationSourceName = Configuration.LocalizationSourceName;
        }

        protected override void OnInitializeInRuntime()
        {
            if (_isInitialized) return;

            try
            {
                Logger.Debug($"Initializing {this.GetUnproxiedType().Name}");

                CurrentState = ViewModelState.Initializing;

                // Configurar recursos de localización específicos
                //ConfigureLocalization();

                // Inicializar colecciones y eventos
                //InitializeCollections();
                //SubscribeToEvents();

                // Configurar servicios específicos del ViewModel
                //InitializeServices();

                // Configurar filtros por defecto
                //InitializeDefaultFilters();
                if (Configuration.AutoApplyFilters)
                {

                }

                // Carga inicial de datos (asíncrona, no bloquea UI)
                if (Configuration.AutoLoadOnInitialize)
                {

                }

                CurrentState = ViewModelState.Ready;
                _isInitialized = true;

                Logger.Debug($"{this.GetUnproxiedType().Name} setup successfully");
            }
            catch (Exception ex)
            {
                CurrentState = ViewModelState.Error;
                Logger.Error($"An error has occurred initializing {this.GetUnproxiedType().Name}", ex);
                throw;
            }
        }

        protected ViewModelConfiguration Configuration { get; private set; }

        /// <summary>
        /// For internal use only
        /// </summary>
        protected virtual bool HasService => false;

        protected Dictionary<string, bool> PermissionCache = new();

        protected DateTime LastPermissionRefresh = DateTime.MinValue;

        protected bool IsLoaded => _entities.Count > 0;

        protected virtual int PermissionCacheLifetimeMinutes => 5;

        public virtual ViewModelStatistics Statistics { get; private set; }

        public virtual ViewModelState CurrentState { get; protected set; } = ViewModelState.Ready;

        public virtual bool IsLoading { get; protected set; }

        public virtual bool IsSaving { get; protected set; }

        public virtual bool HasPendingChanges { get; protected set; }

        public virtual bool IsEditing { get; protected set; }

        public virtual bool IsCheckingPermissions { get; protected set; }

        public virtual ObservableCollection<TEntityDto> Entities => _entities;

        public virtual ObservableCollection<TEntityDto> FilteredEntities { get; protected set; } = new();

        public virtual bool HasEntities => FilteredEntities?.Count > 0;

        public virtual int TotalCount { get; protected set; }

        public virtual TEntityDto SelectedEntity { get; set; }

        public virtual bool HasSelection => SelectedEntity != null;

        public virtual Func<TEntityDto> DefaultEntitySelector { get; protected set; }

        public virtual IList<TEntityDto> SelectedEntities { get; set; } = new List<TEntityDto>();

        public virtual bool HasMultipleSelection => SelectedEntities?.Count > 1;

        public virtual TGetAllInput Filter { get; set; }

        public virtual bool HasActiveFilters => false;

        public virtual ObservableCollection<string> GrantedPermissions { get; protected set; } = new();

        public virtual bool AllowViewEntities { get; protected set; }

        public virtual bool AllowCreateEntities { get; protected set; }

        public virtual bool AllowUpdateEntities { get; protected set; }

        public virtual bool AllowDeleteEntities { get; protected set; }

        public virtual bool AllowPrintEntities { get; protected set; }

        public virtual bool AllowExportEntities { get; protected set; }

        protected virtual bool AllowExecuteOperations =>
            CurrentState == ViewModelState.Ready && !IsLoading && !IsSaving;

        #region Estados del ViewModel

        public virtual string StatusMessage => GetStatusMessage();

        private async Task CancelCurrentOperationAsync()
        {
            if (_cancellationTokenSource != null)
            {
                try
                {
                    await _cancellationTokenSource.CancelAsync();
                }
                catch (ObjectDisposedException)
                {
                    // Token ya fue disposed, ignorar
                }
                finally
                {
                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSource = null;
                }
            }
        }

        /// <summary>
        /// Cambia el estado del ViewModel y notifica a la UI.
        /// </summary>
        /// <param name="newState">Nuevo estado</param>
        /// <param name="reason">Razón del cambio (para logging)</param>
        protected virtual void ChangeState(ViewModelState newState, string reason = null)
        {
            var previousState = CurrentState;
            CurrentState = newState;

            Logger.Debug($"Changing state from {previousState} to {newState}. Reason: {reason ?? "Undefined"}");

            // Notificar cambio de estado
            OnStateChanged(previousState, newState);

            // Actualizar comandos que dependen del estado
            RefreshCommandStates();
        }

        /// <summary>
        /// Llamado cuando cambia el estado del ViewModel.
        /// Virtual para reacciones específicas en clases derivadas.
        /// </summary>
        protected virtual void OnStateChanged(ViewModelState previousState, ViewModelState newState)
        {
        }

        /// <summary>
        /// Actualiza el estado CanExecute de todos los comandos.
        /// Se llama automáticamente cuando cambia el estado.
        /// </summary>
        protected virtual void RefreshCommandStates()
        {
        }

        protected virtual string GetStatusMessage()
        {
            return CurrentState switch
                   {
                       ViewModelState.Loading => L("LoadingData"),
                       ViewModelState.Saving => L("SavingData"),
                       ViewModelState.Editing => L("EditingEntity"),
                       ViewModelState.Error => L("ExceptionMessage"),
                       ViewModelState.Ready when !HasEntities => L("NoDataAvailable"),
                       ViewModelState.Ready when HasActiveFilters => L("FilteredResults", FilteredEntities.Count, TotalCount),
                       ViewModelState.Ready => L("TotalOfRecords", FilteredEntities.Count),
                       _ => string.Empty
                   };
        }

        #endregion

        #region Interfaz IEntitiesViewModel

        ObservableCollection<TEntityDto> IEntitiesViewModel<TEntityDto, TPrimaryKey>.Entities => Entities;

        bool IEntitiesViewModel<TEntityDto, TPrimaryKey>.IsLoading => IsLoading;

        #endregion

        #region Interfaz ISupportParentViewModel

        public virtual object ParentViewModel { get; set; }

        protected void OnParentViewModelChanged()
        {
            AllowSaveReset = true;
            Logger.Debug($"Setup parent viewmodel for {this.GetUnproxiedType().Name} with {ParentViewModel?.GetUnproxiedType().Name}");
        }

        public bool AllowSaveReset { get; protected set; }

        /// <summary>
        /// Comando de guardado genérico.
        /// Las clases derivadas deben sobrescribir para implementar lógica específica.
        /// </summary>
        [Display(AutoGenerateField = false)]
        public virtual void Save()
        {
            Logger.Debug("Save command executed - base implementation");
        }

        /// <summary>
        /// Comando de reset genérico.
        /// Las clases derivadas deben sobrescribir para implementar lógica específica.
        /// </summary>
        [Display(AutoGenerateField = false)]
        public virtual void Reset()
        {
            HasPendingChanges = false;
            IsEditing = false;
            //EditingEntity = default;
            //NewEntity = default;

            Logger.Debug("Reset command executed - base implementation");
        }

        #endregion

        #region Interfaz IDocumentContent

        protected IDocumentOwner DocumentOwner { get; private set; }

        IDocumentOwner IDocumentContent.DocumentOwner
        {
            get => DocumentOwner;
            set => DocumentOwner = value;
        }

        object IDocumentContent.Title => GetDocumentTitle();

        protected virtual object GetDocumentTitle()
        {
            var entityName = typeof(TEntityDto).Name.Replace("Dto", "");
            var count = HasEntities ? FilteredEntities.Count : 0;

            return HasActiveFilters ?
                L("FilteredEntitiesTitle", entityName, count) :
                L("EntitiesTitle", entityName, count);
        }

        void IDocumentContent.OnClose(CancelEventArgs e) => OnClose(e);

        void IDocumentContent.OnDestroy() => OnDestroy();

        protected virtual void OnClose(CancelEventArgs e)
        {
            if (HasPendingChanges)
            {
                var result = MessageBoxService.ShowMessage(
                    L("UnsavedChangesMessage"),
                    L("UnsavedChanges"),
                    MessageButton.YesNoCancel,
                    MessageIcon.Question);

                switch (result)
                {
                    case MessageResult.Cancel:
                        e.Cancel = true;
                        return;
                    case MessageResult.Yes:
                        Save();
                        break;
                    case MessageResult.No:
                        // Continuar sin guardar
                        break;
                }
            }
        }

        /// <summary>
        /// Comando para cerrar la vista/documento actual.
        /// </summary>
        [Command]
        public virtual void Close()
        {
            Logger.Debug($"Closing {this.GetUnproxiedType().Name} document");
            DocumentOwner?.Close(this);
        }

        #endregion

        #region Notificaciones

        /// <summary>
        /// Comando para actualizar manualmente la entidad seleccionada.
        /// Útil para refrescar binding después de cambios externos.
        /// </summary>
        [Command]
        public virtual void NotifySelectionChanged()
        {
            this.RaisePropertyChanged(x => x.SelectedEntity);
            this.RaisePropertyChanged(x => x.SelectedEntities);
        }

        /// <summary>
        /// Comando para actualizar manualmente la entidad seleccionada.
        /// Útil para refrescar binding después de cambios externos.
        /// </summary>
        [Command]
        public virtual async Task NotifyAsync()
        {
            INotification notification = NotificationService.CreatePredefinedNotification("Texto 1", "Texto 2", "Texto 3");
            NotificationResult result = await notification.ShowAsync();
        }

        #endregion

        #region Cleanup y Disposable

        protected virtual void OnDestroy()
        {
            try
            {
                Logger.Debug($"Disposing {this.GetUnproxiedType().Name}");
                AsyncHelper.RunSync(CancelCurrentOperationAsync);
                //UnsubscribeFromEvents();

                // Limpiar referencias
                SelectedEntity = null;
                if(HasMultipleSelection)
                    SelectedEntities?.Clear();
                //EditingEntity = default;
                //NewEntity = default;

                Logger.Debug($"{this.GetUnproxiedType().Name} disposed successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error disposing {this.GetUnproxiedType().Name}", ex);
            }
        }

        #endregion

        #region Servicios DevExpress

        protected IMessageBoxService MessageBoxService =>
            this.GetService<IMessageBoxService>();

        protected IDocumentManagerService DocumentManagerService =>
            this.GetService<IDocumentManagerService>();

        protected IDispatcherService DispatcherService =>
            this.GetService<IDispatcherService>();

        protected IDialogService DialogService =>
            this.GetService<IDialogService>();

        protected INotificationService NotificationService =>
            this.GetService<INotificationService>();

        #endregion
    }
}