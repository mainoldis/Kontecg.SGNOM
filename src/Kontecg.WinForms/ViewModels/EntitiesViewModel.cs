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
using System.Collections.Specialized;
using System.Linq;
using Kontecg.Authorization;
using Kontecg.Logging;
using Kontecg.Runtime.Validation;
using Kontecg.UI;
using Kontecg.Presenters;

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

    public abstract class EntitiesViewModel<TEntityDto, TPrimaryKey, TFilter, TService>
        : EntitiesViewModel<TEntityDto, TPrimaryKey, TFilter, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>, TService>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TFilter : IPagedResultRequest, ISortedResultRequest, new()
        where TService : ICrudAppService<TEntityDto, TPrimaryKey, TFilter, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>>
    {
        /// <inheritdoc />
        protected EntitiesViewModel(TService service, 
            ViewModelConfiguration configuration) 
            : base(service, configuration)
        {
        }
    }

    public abstract class EntitiesViewModel<TEntityDto, TPrimaryKey, TFilter, TCreateInput, TUpdateInput, TGetInput, TDeleteInput, TService>
        : EntitiesViewModel<TEntityDto, TPrimaryKey, TFilter, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TFilter : IPagedResultRequest, ISortedResultRequest, new()
        where TCreateInput : class, IEntityDto<TPrimaryKey>
        where TUpdateInput : class, IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>, new()
        where TDeleteInput : class, IEntityDto<TPrimaryKey>
        where TService : ICrudAppService<TEntityDto, TPrimaryKey, TFilter, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
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

    public abstract class AsyncEntitiesViewModel<TEntityDto, TPrimaryKey, TFilter, TService>
        : AsyncEntitiesViewModel<TEntityDto, TPrimaryKey, TFilter, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>, TService>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TFilter : IPagedResultRequest, ISortedResultRequest, new()
        where TService : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TFilter, TEntityDto, TEntityDto, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>>
    {
        /// <inheritdoc />
        protected AsyncEntitiesViewModel(TService service, 
            ViewModelConfiguration configuration)
            : base(service, configuration)
        {
        }
    }

    public abstract class AsyncEntitiesViewModel<TEntityDto, TPrimaryKey, TFilter, TCreateInput, TUpdateInput, TGetInput, TDeleteInput, TService>
        : EntitiesViewModel<TEntityDto, TPrimaryKey, TFilter, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TFilter : IPagedResultRequest, ISortedResultRequest, new()
        where TCreateInput : class, IEntityDto<TPrimaryKey>
        where TUpdateInput : class, IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>, new()
        where TDeleteInput : class, IEntityDto<TPrimaryKey>
        where TService : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TFilter, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
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
    /// <typeparam name="TFilter">The type of the input for getting all entities.</typeparam>
    /// <typeparam name="TCreateInput">The type of the input for creating an entity.</typeparam>
    /// <typeparam name="TUpdateInput">The type of the input for updating an entity.</typeparam>
    /// <typeparam name="TGetInput">The type of the input for getting a single entity.</typeparam>
    /// <typeparam name="TDeleteInput">The type of the input for deleting an entity.</typeparam>
    [POCOViewModel]
    public abstract class EntitiesViewModel<TEntityDto, TPrimaryKey, TFilter, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
        : KontecgViewModelBase, IEntitiesViewModel<TEntityDto, TPrimaryKey>, IDocumentContent, ISupportParentViewModel, ISupportParameter
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TFilter : IPagedResultRequest, ISortedResultRequest, new()
        where TCreateInput : class, IEntityDto<TPrimaryKey>
        where TUpdateInput : class, IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>, new()
        where TDeleteInput : class, IEntityDto<TPrimaryKey>
    {
        private readonly ICancellationTokenProvider _cancellationTokenProvider;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ObservableCollection<TEntityDto> _entities;
        private readonly TFilter _defaultFilter;
        private bool _isInitialized;

        /// <inheritdoc />
        protected EntitiesViewModel(ViewModelConfiguration configuration)
        {
            Configuration = configuration ?? new ViewModelConfiguration();
            Statistics = new ViewModelStatistics();
            _entities = new ObservableCollection<TEntityDto>();
            _defaultFilter = new TFilter {MaxResultCount = int.MaxValue};
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

                // Inicializar colecciones y eventos
                InitializeCollections();
                SubscribeToEvents();

                // Configurar servicios específicos del ViewModel
                InitializeServices();

                InitializeDefaultFilters();
                // Configurar filtros por defecto
                if (Configuration.AutoApplyFilters)
                {
                    ApplyFilters();
                }

                // Carga inicial de datos (asíncrona, no bloquea UI)
                if (Configuration.AutoLoadOnInitialize)
                {
                    _ = LoadAsync();
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

        public virtual int TotalCount { get; protected set; }

        public virtual TEntityDto SelectedEntity { get; set; }

        public virtual bool HasSelection => SelectedEntity != null;

        public virtual Func<TEntityDto> DefaultEntitySelector { get; protected set; }

        public virtual IList<TEntityDto> SelectedEntities { get; set; } = new List<TEntityDto>();

        public virtual bool HasMultipleSelection => SelectedEntities?.Count > 1;

        public virtual TFilter Filter { get; set; }

        public virtual bool HasActiveFilters => HasCustomFilters();

        public virtual TUpdateInput EditingEntity { get; set; }

        public virtual TCreateInput NewEntity { get; set; }

        public virtual ObservableCollection<string> GrantedPermissions { get; protected set; } = new();

        public virtual bool AllowViewEntities { get; protected set; }

        public virtual bool AllowCreateEntities { get; protected set; }

        public virtual bool AllowUpdateEntities { get; protected set; }

        public virtual bool AllowDeleteEntities { get; protected set; }

        public virtual bool AllowPrintEntities { get; protected set; }

        public virtual bool AllowExportEntities { get; protected set; }

        protected virtual bool AllowExecuteOperations =>
            CurrentState == ViewModelState.Ready && !IsLoading && !IsSaving;

        #region Lifecycle

        /// <summary>
        /// Inicializa las colecciones y sus configuraciones.
        /// </summary>
        private void InitializeCollections()
        {
            // Inicializar listas de selección
            SelectedEntities ??= new List<TEntityDto>();
        }

        /// <summary>
        /// Inicializa servicios específicos del ViewModel.
        /// Virtual para permitir personalización en clases derivadas.
        /// </summary>
        protected virtual void InitializeServices()
        {
        }

        /// <summary>
        /// Inicializa los filtros por defecto del ViewModel.
        /// Virtual para permitir personalización.
        /// </summary>
        protected virtual void InitializeDefaultFilters()
        {
            Filter ??= _defaultFilter;
        }

        #endregion

        #region Events

        public event EventHandler<NotifyCollectionChangedEventArgs> EntitiesCollectionChanged;

        public event EventHandler<PagedResultDto<TEntityDto>> DataLoadedSuccessfully;

        public event EventHandler DataLoadCancelled;

        public event EventHandler EntitiesLoaded;

        public event EventHandler EntityCreated;

        public event EventHandler EntityUpdated;

        public event EventHandler EntityDeleted;

        public event EventHandler<EntitiesCountEventArgs> TotalCountChanged;

        public event EventHandler<EntityEventArgs<TPrimaryKey>> SelectedEntityChanged;

        public event EventHandler FilterApplied;

        public event EventHandler StateChanged;

        public event EventHandler HasActiveFiltersChanged; 


        /// <summary>
        /// Suscribe a eventos de las colecciones.
        /// </summary>
        private void SubscribeToEvents()
        {
            Entities.CollectionChanged += OnEntitiesCollectionChanged;
        }

        /// <summary>
        /// Desuscribe de eventos de las colecciones.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            Entities.CollectionChanged -= OnEntitiesCollectionChanged;
        }

        /// <summary>
        /// Maneja cambios en la colección principal de entidades.
        /// </summary>
        private void OnEntitiesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_isInitialized || CurrentState == ViewModelState.Loading)
                return;

            HasPendingChanges = true;
            OnItemsCollectionChanged(e);

            // Actualizar colección filtrada si no estamos cargando
            if (CurrentState == ViewModelState.Ready && Configuration.AutoApplyFilters)
            {
                ApplyFilters();
            }
        }

        /// <summary>
        /// Llamado cuando cambia la colección principal.
        /// Virtual para permitir personalización.
        /// </summary>
        protected virtual void OnItemsCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            EntitiesCollectionChanged?.Invoke(this, e);
        }

        protected virtual void OnDataLoadedSuccessfully(PagedResultDto<TEntityDto> result)
        {
            DataLoadedSuccessfully?.Invoke(this, result);
        }

        protected virtual void OnDataLoadCancelled()
        {
            DataLoadCancelled?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnEntitiesLoaded(IEnumerable<TEntityDto> entities)
        {
            EntitiesLoaded?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnEntityCreated(TCreateInput input)
        {
            EntityCreated?.Invoke(this, EventArgs.Empty);
            EventBus.Trigger(new SynchronizationMessage<TCreateInput, TPrimaryKey>(input, ViewModelEntityState.New));
        }

        protected virtual void OnEntityUpdated(TUpdateInput input)
        {
            EntityUpdated?.Invoke(this, EventArgs.Empty);
            EventBus.Trigger(new SynchronizationMessage<TUpdateInput, TPrimaryKey>(input, ViewModelEntityState.Changed));
        }

        protected virtual void OnEntityDeleted(TDeleteInput input)
        {
            EntityDeleted?.Invoke(this, EventArgs.Empty);
            EventBus.Trigger(new SynchronizationMessage<TDeleteInput, TPrimaryKey>(input, ViewModelEntityState.Changed));
        }

        protected virtual void OnSelectedEntityChanged()
        {
            var key = SelectedEntity != null ? SelectedEntity.Id : default;
            SelectedEntityChanged?.Invoke(this, new EntityEventArgs<TPrimaryKey>(key));
            //this.RaiseCanExecuteChanged(x => x.UpdateAsync(null));
            //this.RaiseCanExecuteChanged(x => x.DeleteAsync(null));
        }

        protected virtual void OnTotalCountChanged()
        {
            TotalCountChanged?.Invoke(this, new EntitiesCountEventArgs(TotalCount));
        }

        protected virtual void OnHasActiveFiltersChanged()
        {
            HasActiveFiltersChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Filters

        private void ApplyFilters()
        {
            try
            {
                Logger.Debug($"Setting filters. Entities: {Entities.Count}");

                var total = Entities.Count;
                var filtered = new List<TEntityDto>(FilterEntities(Entities, Filter));
                var sorted = SortEntities(filtered);

                // Actualizar colección filtrada de forma eficiente
                _entities.Clear();
                foreach (var entity in sorted) 
                    _entities.Add(entity);

                Logger.Debug($"Filters applied. Result: {_entities.Count}/{total}");

                OnFilterApplied();
            }
            catch (Exception ex)
            {
                Logger.Error("Has occurred an error applying filters", ex);
            }
        }

        protected virtual IEnumerable<TEntityDto> FilterEntities(
            IEnumerable<TEntityDto> entities,
            TFilter customFilter)
        {
            var result = entities;

            // Filtros estructurados personalizados
            if (customFilter != null)
            {
                result = ApplyCustomFilters(result, customFilter);
            }

            return result;
        }

        protected virtual IEnumerable<TEntityDto> ApplyCustomFilters(
            IEnumerable<TEntityDto> entities,
            TFilter filter)
        {
            return entities;
        }

        protected virtual IEnumerable<TEntityDto> SortEntities(IEnumerable<TEntityDto> entities)
        {
            return entities.OrderBy(GetSortKeySelector());
        }

        protected virtual Func<TEntityDto, object> GetSortKeySelector()
        {
            return entity => entity.Id;
        }

        protected virtual bool HasCustomFilters()
        {
            return Filter != null && object.ReferenceEquals(Filter, _defaultFilter);
        }

        [Command]
        public virtual void ApplyFilter()
        {
            Logger.Debug("Setting filters manually");
            ApplyFilters();
        }

        /// <summary>
        /// Comando para limpiar todos los filtros.
        /// </summary>
        [Command]
        public virtual void ClearFilter()
        {
            Logger.Debug("Clearing filters manually");

            Filter = _defaultFilter;

            ApplyFilters();

            Logger.Debug("Filters cleared successfully");
        }

        public virtual bool CanApplyFilter() => AllowExecuteOperations;

        /// <summary>
        /// Determina si se pueden limpiar filtros.
        /// VIRTUAL para personalización.
        /// </summary>
        public virtual bool CanClearFilter() => AllowExecuteOperations && HasActiveFilters;

        protected virtual void OnFilterApplied()
        {
            FilterApplied?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Datasource

        [AsyncCommand(CanExecuteMethodName = "CanLoadAsync", AllowMultipleExecution = false)]
        public virtual async Task LoadAsync()
        {
            await CancelCurrentOperationAsync();

            ChangeState(ViewModelState.Loading, "Loading data from DataSource");

            IsLoading = true;
            HasPendingChanges = false;

            try
            {
                Logger.Debug($"Loading data for {typeof(TEntityDto).Name}");
                var result = await LoadDataSourceAsync();

                await DispatcherService.BeginInvoke(() => UpdateEntities(result));

                OnDataLoadedSuccessfully(result);

                Logger.Debug($"Loaded {result.Items.Count} items of {typeof(TEntityDto).Name} Successfully");

                ChangeState(ViewModelState.Ready, "Data loaded successfully");
            }
            catch (OperationCanceledException ex)
            {
                Logger.Debug("Operation cancelled by user", ex);
                OnDataLoadCancelled();
                ChangeState(ViewModelState.Ready, "Loading data cancelled by user");
            }
            catch (Exception ex)
            {
                Logger.Error($"Has occurred an error loading data for {typeof(TEntityDto).Name}", ex);
                await HandleLoadErrorAsync(ex);
                ChangeState(ViewModelState.Error, $"Error on loading data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected abstract ValueTask<PagedResultDto<TEntityDto>> LoadDataSourceAsync();

        public virtual bool CanLoadAsync()
        {
            return CurrentState != ViewModelState.Loading &&
                   CurrentState != ViewModelState.Saving &&
                   CurrentState != ViewModelState.Initializing;
        }

        private void UpdateEntities(PagedResultDto<TEntityDto> result)
        {
            try
            {
                UnsubscribeFromEvents();

                _entities.Clear();

                // Agregar nuevas entidades
                foreach (var item in result.Items) _entities.Add(item);

                TotalCount = result.TotalCount;

                if(Configuration.AutoApplyFilters)
                    ApplyFilters();

                SubscribeToEvents();

                this.RaisePropertyChanged(x => x.Entities);
                this.RaisePropertyChanged(x => x.TotalCount);
                this.RaisePropertyChanged(x => x.HasActiveFilters);
                this.RaisePropertyChanged(x => x.StatusMessage);

                // Restablecer estados
                HasPendingChanges = false;

                // Callback para lógica específica
                OnEntitiesLoaded(_entities);

                Logger.Debug($"Updating UI successfully. {_entities.Count} showed after filters applied");
            }
            catch (Exception ex)
            {
                Logger.Error("An critical error has occurred updating user interface", ex);
                throw;
            }
        }

        #endregion

        #region Estados del ViewModel

        protected virtual string ViewName => typeof(TEntityDto).Name + "EntitiesView";

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

            Logger.Debug(previousState != newState
                ? $"Changing state from {previousState} to {newState}. Reason: {reason ?? "Undefined"}"
                : $"Current state {newState}. Reason: {reason ?? "Undefined"}");

            this.RaisePropertyChanged(x => x.StatusMessage);
            
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
            StateChanged?.Invoke(this, EventArgs.Empty);
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
                       ViewModelState.Ready when !IsLoaded => L("NoDataAvailable"),
                       ViewModelState.Ready when HasActiveFilters => L("FilteredResults", Entities.Count, TotalCount),
                       ViewModelState.Ready => L("TotalOfRecords", TotalCount),
                       _ => string.Empty
                   };
        }

        #endregion

        #region Handling errors

        protected virtual async Task HandleLoadErrorAsync(Exception ex)
        {
            await HandleGenericErrorAsync(ex, "ErrorLoadingData");
        }

        private async Task HandleGenericErrorAsync(Exception ex, string fallbackMessageKey)
        {
            string message;
            LogSeverity severity;

            // Determinar tipo de excepción y mensaje apropiado
            switch (ex)
            {
                case KontecgValidationException validationEx:
                    // Errores de validación - mostrar detalles
                    message = FormatValidationErrors(validationEx);
                    severity = validationEx.Severity;
                    break;

                case UserFriendlyException userFriendlyEx:
                    message = userFriendlyEx.Message;
                    severity = userFriendlyEx.Severity;
                    break;

                case KontecgAuthorizationException authEx:
                    // Errores de autorización
                    message = L("NotAuthorizedForThisOperation");
                    severity = authEx.Severity;
                    break;

                case TimeoutException:
                    // Timeout
                    message = L("OperationTimedOut");
                    severity = LogSeverity.Warn;
                    break;

                default:
                    message = L(fallbackMessageKey);
                    severity = LogSeverity.Error;
                    break;
            }

            await ShowErrorMessageAsync(message, severity);
        }

        private string FormatValidationErrors(KontecgValidationException validationEx)
        {
            if (validationEx.ValidationErrors?.Any() == true)
            {
                var errors = validationEx.ValidationErrors
                                         .Select(e => $"• {e.MemberNames.FirstOrDefault()}: {e.ErrorMessage}")
                                         .Take(5);

                return $"{L("ValidationErrors")}:\n{string.Join("\n", errors)}";
            }

            return validationEx.Message;
        }

        private string GetMessageCaption(LogSeverity logSeverity)
        {
            return logSeverity switch
                   {
                       LogSeverity.Debug or LogSeverity.Info => L("Information"),
                       LogSeverity.Warn => L("Warning"),
                       LogSeverity.Error or LogSeverity.Fatal => L("Error"),
                       _ => L("Message")
                   };
        }

        private MessageIcon GetMessageIcon(LogSeverity logSeverity)
        {
            return logSeverity switch
                   {
                       LogSeverity.Debug or LogSeverity.Info => MessageIcon.Information,
                       LogSeverity.Warn => MessageIcon.Warning,
                       LogSeverity.Error or LogSeverity.Fatal => MessageIcon.Error,
                       _ => MessageIcon.Information
                   };
        }

        protected async Task ShowErrorMessageAsync(string message, LogSeverity severity)
        {
            await DispatcherService.BeginInvoke(() =>
            {
                MessageBoxService.ShowMessage(
                    message,
                    GetMessageCaption(severity),
                    MessageButton.OK,
                    GetMessageIcon(severity));
            });
        }

        protected async Task ShowSuccessMessageAsync(string message)
        {
            await DispatcherService.BeginInvoke(() =>
            {
                MessageBoxService.ShowMessage(
                    message,
                    L("Success"),
                    MessageButton.OK,
                    MessageIcon.Information);
            });
        }

        protected async Task<MessageResult> ShowConfirmationAsync(string message)
        {
            MessageResult result = MessageResult.None;

            await DispatcherService.BeginInvoke(() =>
                result = MessageBoxService.ShowMessage(
                    message,
                    L("Confirmation"),
                    MessageButton.YesNo,
                    MessageIcon.Question));

            return result;
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
            AllowSaveReset = Configuration.AllowSaveReset;
            Logger.Debug($"Setup parent viewmodel for {this.GetUnproxiedType().Name} with {ParentViewModel?.GetUnproxiedType().Name}");
        }

        public bool AllowSaveReset { get; protected set; }

        /// <summary>
        /// Comando de guardado genérico.
        /// Las clases derivadas deben sobrescribir para implementar lógica específica.
        /// </summary>
        [Display(AutoGenerateField = false)]
        [Command]
        public virtual void Save()
        {
            Logger.Debug("Save command executed - base implementation");
        }

        public virtual bool CanSave() =>
            AllowExecuteOperations && HasPendingChanges && !IsSaving && AllowSaveReset;

        /// <summary>
        /// Comando de reset genérico.
        /// Las clases derivadas deben sobrescribir para implementar lógica específica.
        /// </summary>
        [Display(AutoGenerateField = false)]
        [Command]
        public virtual void Reset()
        {
            HasPendingChanges = false;
            IsEditing = false;
            //EditingEntity = default;
            //NewEntity = default;

            Logger.Debug("Reset command executed - base implementation");
        }

        public virtual bool CanReset() =>
            AllowExecuteOperations && (HasPendingChanges || IsEditing) && AllowSaveReset;

        #endregion

        #region ISupportParameter

        public event EventHandler ParameterChanged;

        private object _parameterCore;

        protected object Parameter
        {
            get => _parameterCore;
            private set
            {
                _parameterCore = value;
                ParameterChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        object ISupportParameter.Parameter
        {
            get => Parameter;
            set => Parameter = value;
        }

        #endregion

        #region Interfaz IDocumentContent

        protected IDocument FindDocument<TViewModel>()
        {
            if (DocumentManagerService == null) return null;
            return DocumentManagerService.Documents.FirstOrDefault(d => d.Content is TViewModel);
        }

        protected IDocument FindDocument<TViewModel>(TPrimaryKey key)
        {
            if (DocumentManagerService == null) return null;
            foreach (IDocument document in DocumentManagerService.Documents)
            {
                if (document.Content is ISingleObjectViewModel<TEntityDto, TPrimaryKey> entityViewModel and TViewModel && object.Equals(entityViewModel.Id, key))
                    return document;
            }
            return null;
        }

        protected IDocumentOwner DocumentOwner { get; private set; }

        IDocumentOwner IDocumentContent.DocumentOwner
        {
            get => DocumentOwner;
            set => DocumentOwner = value;
        }

        protected virtual object GetTitle()
        {
            var entityName = typeof(TEntityDto).Name.Replace("Dto", "");
            var count = IsLoaded ? Entities.Count : 0;

            return HasActiveFilters ?
                L("FilteredEntitiesTitle", entityName, count) :
                L("EntitiesTitle", entityName, count);
        }

        object IDocumentContent.Title => GetTitle();

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
            var result = await ShowConfirmationAsync("Prueba de confirmación");
        }

        #endregion

        #region Cleanup y Disposable

        protected virtual void OnDestroy()
        {
            try
            {
                Logger.Debug($"Disposing {this.GetUnproxiedType().Name}");
                AsyncHelper.RunSync(CancelCurrentOperationAsync);
                UnsubscribeFromEvents();

                // Limpiar referencias
                SelectedEntity = null;
                if(HasMultipleSelection)
                    SelectedEntities?.Clear();
                EditingEntity = default;
                NewEntity = default;

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

        #endregion
    }
}