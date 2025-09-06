#if false
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.Specialized;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using Kontecg.Application.Services.Dto;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using Kontecg.Threading;
using Kontecg.Application.Services;
using Kontecg.Logging;
using System.Linq;
using Kontecg.UI.Inputs;
using Kontecg.Authorization;
using Kontecg.Runtime.Validation;
using Kontecg.UI;

namespace Kontecg.ViewModels
{
    /// <summary>
    /// Clase base abstracta para ViewModels que manejan colecciones de entidades.
    /// Implementa el patrón MVVM usando DevExpress POCO ViewModels con integración completa ABP.
    /// 
    /// CARACTERÍSTICAS ABP INTEGRADAS:
    /// - Localización automática con ILocalizationManager
    /// - Validación mediante interceptores ABP (sin duplicación)
    /// - Logging usando sistema ABP
    /// - Manejo de excepciones ABP (UserFriendlyException, etc.)
    /// - Permisos y características ABP
    /// - UnitOfWork automático
    /// 
    /// CARACTERÍSTICAS DEVEXPRESS:
    /// - POCO ViewModels con generación automática de propiedades
    /// - Comandos con CanExecute automático y customizable
    /// - Servicios integrados (MessageBox, Dispatcher, etc.)
    /// - Binding optimizado para controles DevExpress
    /// </summary>
    /// <typeparam name="TEntityDto">Tipo de entidad DTO</typeparam>
    /// <typeparam name="TPrimaryKey">Tipo de clave primaria</typeparam>
    /// <typeparam name="TGetAllInput">Tipo para parámetros de consulta (filtros, paginación)</typeparam>
    /// <typeparam name="TCreateInput">Tipo para creación de entidades</typeparam>
    /// <typeparam name="TUpdateInput">Tipo para actualización de entidades</typeparam>
    /// <typeparam name="TGetInput">Tipo para obtener una entidad específica</typeparam>
    /// <typeparam name="TDeleteInput">Tipo para eliminación de entidades</typeparam>
    /// <typeparam name="TService">Tipo del servicio de aplicación</typeparam>
    [POCOViewModel]
    public abstract partial class CollectionViewModelBase<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput, TService> :
        KontecgViewModelBase,
        IEntitiesViewModel<TEntityDto, TPrimaryKey>,
        IDocumentContent,
        ISupportParentViewModel
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>, new()
        where TDeleteInput : IEntityDto<TPrimaryKey>
        where TService : IApplicationService
    {
        #region Campos Privados

        private readonly ICancellationTokenProvider _cancellationTokenProvider;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ObservableCollection<TEntityDto> _entities = new();
        private bool _isInitialized = false;

        #endregion

        #region Propiedades de Estado - DevExpress Intercepted

        /// <summary>
        /// Indica si se están cargando entidades en segundo plano.
        /// IMPORTANTE: Cuando IsLoading = true, los comandos de edición/eliminación se deshabilitan automáticamente.
        /// DevExpress actualiza automáticamente la UI y los comandos cuando cambia.
        /// </summary>
        public virtual bool IsLoading { get; protected set; }

        /// <summary>
        /// Indica si hay una operación de guardado en progreso.
        /// Se usa para deshabilitar comandos durante operaciones críticas.
        /// </summary>
        public virtual bool IsSaving { get; protected set; }

        /// <summary>
        /// Indica si hay cambios sin guardar en el ViewModel.
        /// Útil para mostrar advertencias al cerrar o cambiar de vista.
        /// </summary>
        public virtual bool HasChanges { get; protected set; }

        /// <summary>
        /// Indica si estamos en modo de edición.
        /// Afecta la habilitación de ciertos comandos y la UI.
        /// </summary>
        public virtual bool IsEditing { get; protected set; }

        /// <summary>
        /// Estado general del ViewModel para control fino de la UI.
        /// </summary>
        public virtual ViewModelState CurrentState { get; protected set; } = ViewModelState.Ready;

        #endregion

        #region Colecciones de Datos - DevExpress Intercepted

        /// <summary>
        /// Colección principal de entidades completa cargada desde el servidor.
        /// Al ser virtual, DevExpress genera automáticamente notificaciones PropertyChanged.
        /// NO usar directamente para binding de controles - usar FilteredEntities.
        /// </summary>
        public virtual ObservableCollection<TEntityDto> Entities => _entities;

        /// <summary>
        /// Colección filtrada y ordenada que se muestra en la UI.
        /// Esta es la colección que debe usarse para binding con GridControl y otros controles.
        /// Se actualiza automáticamente cuando cambian los filtros o la colección principal.
        /// </summary>
        public virtual ObservableCollection<TEntityDto> FilteredEntities { get; protected set; } = new();

        /// <summary>
        /// Total de registros en el servidor (útil para paginación).
        /// Puede ser diferente al count de FilteredEntities si hay filtros aplicados.
        /// </summary>
        public virtual int TotalCount { get; protected set; }

        /// <summary>
        /// Entidad seleccionada en la UI - fundamental para operaciones CRUD.
        /// DevExpress actualiza automáticamente los comandos cuando esto cambia.
        /// Los comandos de edición/eliminación dependen de que esto no sea null.
        /// </summary>
        public virtual TEntityDto SelectedEntity { get; set; }

        /// <summary>
        /// Lista de entidades seleccionadas (para operaciones en lote).
        /// Útil para selección múltiple en grids.
        /// </summary>
        public virtual IList<TEntityDto> SelectedEntities { get; set; } = new List<TEntityDto>();

        #endregion

        #region Filtros y Búsqueda - DevExpress Intercepted

        /// <summary>
        /// Filtros estructurados aplicados a la colección.
        /// Se envía al servidor en LoadDataSourceAsync.
        /// </summary>
        public virtual TGetAllInput Filter { get; set; }

        /// <summary>
        /// Texto de búsqueda rápida para filtrado local.
        /// Se aplica además de Filter para búsqueda instantánea.
        /// </summary>
        public virtual string SearchText { get; set; }

        /// <summary>
        /// Indica si los filtros locales están activos.
        /// Útil para mostrar indicadores visuales en la UI.
        /// </summary>
        public virtual bool HasActiveFilters =>
            !string.IsNullOrWhiteSpace(SearchText) || HasCustomFilters();

        #endregion

        #region Entidades de Edición - DevExpress Intercepted

        /// <summary>
        /// Entidad que está siendo editada actualmente.
        /// Se usa para formularios de edición in-place o en ventanas separadas.
        /// </summary>
        public virtual TUpdateInput EditingEntity { get; set; }

        /// <summary>
        /// Entidad que está siendo creada.
        /// Se usa para formularios de nuevo registro.
        /// </summary>
        public virtual TCreateInput NewEntity { get; set; }

        #endregion

        #region Propiedades Calculadas - ABP Integration

        /// <summary>
        /// Indica si los datos han sido cargados al menos una vez.
        /// </summary>
        protected bool IsLoaded => _entities.Count > 0;

        /// <summary>
        /// Indica si el ViewModel está en un estado que permite ejecutar operaciones.
        /// </summary>
        protected virtual bool CanExecuteOperations =>
            CurrentState == ViewModelState.Ready && !IsLoading && !IsSaving;

        /// <summary>
        /// Nombre del recurso de localización específico para este ViewModel.
        /// Override en clases derivadas para localización específica.
        /// Por defecto usa el nombre de la entidad.
        /// </summary>
        protected virtual string EntityLocalizationSourceName =>
            typeof(TEntityDto).Name.Replace("Dto", "");

        #endregion

        #region Constructor y Ciclo de Vida

        protected CollectionViewModelBase(ICancellationTokenProvider cancellationTokenProvider = null)
        {
            _cancellationTokenProvider = cancellationTokenProvider;

            // Configurar localización ABP
            LocalizationSourceName = "Kontecg"; // Base, se puede override
        }

        /// <summary>
        /// Método de fábrica recomendado por DevExpress para POCO ViewModels.
        /// Permite inyección de dependencias via Castle Windsor.
        /// IMPORTANTE: Se debe registrar el ViewModel en el contenedor IoC.
        /// </summary>
        /// <typeparam name="TViewModel">Tipo específico del ViewModel</typeparam>
        /// <returns>Instancia del ViewModel con dependencias inyectadas</returns>
        public static TViewModel Create<TViewModel>() where TViewModel : class, new()
        {
            return ViewModelSource.Create<TViewModel>();
        }

        /// <summary>
        /// Llamado automáticamente cuando DevExpress inicializa el ViewModel.
        /// Se ejecuta después de que se configure el proxy POCO y se inyecten dependencias.
        /// 
        /// ORDEN DE EJECUCIÓN:
        /// 1. Constructor
        /// 2. Inyección de dependencias (Castle Windsor)
        /// 3. OnInitializeInRuntime (aquí estamos)
        /// 4. OnParameterChanged (si hay parámetros)
        /// </summary>
        protected virtual void OnInitializeInRuntime()
        {
            if (_isInitialized) return;

            try
            {
                CurrentState = ViewModelState.Initializing;

                // Configurar recursos de localización específicos
                ConfigureLocalization();

                // Inicializar colecciones y eventos
                InitializeCollections();
                SubscribeToEvents();

                // Configurar servicios específicos del ViewModel
                InitializeServices();

                // Configurar filtros por defecto
                InitializeDefaultFilters();

                // Carga inicial de datos (asíncrona, no bloquea UI)
                _ = LoadAsync();

                CurrentState = ViewModelState.Ready;
                _isInitialized = true;

                Logger.Debug($"{GetType().Name} inicializado correctamente");
            }
            catch (Exception ex)
            {
                CurrentState = ViewModelState.Error;
                Logger.Error($"Error inicializando {GetType().Name}", ex);
                throw;
            }
        }

        /// <summary>
        /// Configura las fuentes de localización específicas.
        /// Virtual para permitir personalización en ViewModels específicos.
        /// </summary>
        protected virtual void ConfigureLocalization()
        {
            // Intentar usar fuente específica de la entidad si existe
            try
            {
                var entitySource = LocalizationManager.GetSource(EntityLocalizationSourceName);
                if (entitySource != null)
                {
                    LocalizationSourceName = EntityLocalizationSourceName;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug($"Fuente de localización específica '{EntityLocalizationSourceName}' no encontrada, usando fuente base", ex);
                // Mantener fuente base
            }
        }

        /// <summary>
        /// Inicializa las colecciones y sus configuraciones.
        /// </summary>
        private void InitializeCollections()
        {
            // Asegurar que FilteredEntities esté inicializada
            if (FilteredEntities == null)
            {
                FilteredEntities = new ObservableCollection<TEntityDto>();
            }

            // Inicializar listas de selección
            if (SelectedEntities == null)
            {
                SelectedEntities = new List<TEntityDto>();
            }
        }

        /// <summary>
        /// Inicializa servicios específicos del ViewModel.
        /// Virtual para permitir personalización en clases derivadas.
        /// </summary>
        protected virtual void InitializeServices()
        {
            // Las clases derivadas pueden sobrescribir para configurar servicios específicos
            // Ejemplo: cargar datos de lookup, configurar validadores, etc.
        }

        /// <summary>
        /// Inicializa los filtros por defecto del ViewModel.
        /// Virtual para permitir personalización.
        /// </summary>
        protected virtual void InitializeDefaultFilters()
        {
            // Las clases derivadas pueden sobrescribir para configurar filtros iniciales
        }

        #endregion

        #region Estados del ViewModel

        /// <summary>
        /// Cambia el estado del ViewModel y notifica a la UI.
        /// </summary>
        /// <param name="newState">Nuevo estado</param>
        /// <param name="reason">Razón del cambio (para logging)</param>
        protected virtual void ChangeState(ViewModelState newState, string reason = null)
        {
            var previousState = CurrentState;
            CurrentState = newState;

            Logger.Debug($"Estado cambiado de {previousState} a {newState}. Razón: {reason ?? "No especificada"}");

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
            this.RaiseCanExecuteChanged(x => x.LoadAsync());
            this.RaiseCanExecuteChanged(x => x.CreateAsync(null));
            this.RaiseCanExecuteChanged(x => x.UpdateAsync(null));
            this.RaiseCanExecuteChanged(x => x.DeleteAsync(null));
            this.RaiseCanExecuteChanged(x => x.ApplyFilterCommand());
            this.RaiseCanExecuteChanged(x => x.ClearFilterCommand());
        }

        #endregion

        #region Gestión de Eventos

        /// <summary>
        /// Suscribe a eventos de las colecciones.
        /// </summary>
        private void SubscribeToEvents()
        {
            Entities.CollectionChanged += OnEntitiesCollectionChanged;
            FilteredEntities.CollectionChanged += OnFilteredEntitiesCollectionChanged;
        }

        /// <summary>
        /// Desuscribe de eventos de las colecciones.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            Entities.CollectionChanged -= OnEntitiesCollectionChanged;
            FilteredEntities.CollectionChanged -= OnFilteredEntitiesCollectionChanged;
        }

        /// <summary>
        /// Maneja cambios en la colección principal de entidades.
        /// </summary>
        private void OnEntitiesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_isInitialized || CurrentState == ViewModelState.Loading)
                return;

            HasChanges = true;
            OnItemsCollectionChanged(e);

            // Actualizar colección filtrada si no estamos cargando
            if (CurrentState == ViewModelState.Ready)
            {
                ApplyFilter();
            }
        }

        /// <summary>
        /// Maneja cambios en la colección filtrada.
        /// </summary>
        private void OnFilteredEntitiesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnFilteredCollectionChanged(e);

            // Actualizar propiedades calculadas
            this.RaisePropertyChanged(x => x.HasActiveFilters);
        }

        /// <summary>
        /// Llamado cuando cambia la colección principal.
        /// Virtual para permitir personalización.
        /// </summary>
        protected virtual void OnItemsCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// Llamado cuando cambia la colección filtrada.
        /// Virtual para permitir personalización.
        /// </summary>
        protected virtual void OnFilteredCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
        }

        #endregion

        #region Carga de Datos - Métodos Principales

        /// <summary>
        /// Comando principal para cargar datos de forma asíncrona.
        /// Incluye cancelación automática, manejo de errores ABP y estados de UI.
        /// 
        /// CARACTERÍSTICAS ABP:
        /// - UnitOfWork automático si es necesario
        /// - Manejo de UserFriendlyException
        /// - Logging integrado
        /// - Cancelación cooperativa
        /// 
        /// El atributo [Command] hace que DevExpress genere automáticamente:
        /// - LoadAsyncCommand (ICommand)
        /// - CanLoadAsync para habilitar/deshabilitar el comando
        /// </summary>
        [Command]
        public virtual async Task LoadAsync()
        {
            if (!CanLoadAsync())
                return;

            // Cancelar operación anterior si existe
            await CancelCurrentOperationAsync();

            // Crear nuevo contexto de cancelación
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenProvider?.Use(_cancellationTokenSource.Token);

            ChangeState(ViewModelState.Loading, "Iniciando carga de datos");
            IsLoading = true;
            HasChanges = false;

            try
            {
                Logger.Debug($"Iniciando carga de datos para {typeof(TEntityDto).Name}");

                // Llamar método abstracto implementado por clase derivada
                var result = await LoadDataSourceAsync();

                // Actualizar UI en el hilo principal
                await DispatcherService.BeginInvoke(() => UpdateEntities(result));

                // Notificar carga exitosa
                OnDataLoadedSuccessfully(result);

                Logger.Info($"Cargados {result.Items.Count} elementos de {typeof(TEntityDto).Name} exitosamente");

                ChangeState(ViewModelState.Ready, "Carga completada exitosamente");
            }
            catch (OperationCanceledException ex)
            {
                Logger.Debug("Operación de carga cancelada por el usuario", ex);
                OnDataLoadCancelled();
                ChangeState(ViewModelState.Ready, "Carga cancelada");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error cargando datos para {typeof(TEntityDto).Name}", ex);
                await HandleLoadErrorAsync(ex);
                ChangeState(ViewModelState.Error, $"Error en carga: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Método abstracto que debe implementar cada ViewModel específico.
        /// Aquí es donde se hace la llamada real al servicio de aplicación ABP.
        /// 
        /// IMPORTANTE: Este método se ejecuta en un contexto UnitOfWork automático.
        /// No es necesario crear UoW manualmente.
        /// 
        /// EJEMPLO DE IMPLEMENTACIÓN:
        /// <code>
        /// protected override async ValueTask&lt;PagedResultDto&lt;ProductDto&gt;&gt; LoadDataSourceAsync()
        /// {
        ///     var service = IocManager.Resolve&lt;IProductAppService&gt;();
        ///     return await service.GetAllAsync(Filter ?? new GetAllProductsInput());
        /// }
        /// </code>
        /// </summary>
        /// <returns>Resultado paginado con las entidades cargadas</returns>
        protected abstract ValueTask<PagedResultDto<TEntityDto>> LoadDataSourceAsync();

        /// <summary>
        /// Actualiza las colecciones con los datos cargados del servidor.
        /// Maneja la sincronización con la UI de forma segura.
        /// </summary>
        /// <param name="result">Datos cargados del servidor</param>
        private void UpdateEntities(PagedResultDto<TEntityDto> result)
        {
            try
            {
                // Pausar eventos durante la actualización masiva
                UnsubscribeFromEvents();

                // Limpiar colecciones existentes
                _entities.Clear();
                FilteredEntities.Clear();

                // Agregar nuevas entidades
                foreach (var item in result.Items)
                {
                    _entities.Add(item);
                }

                // Actualizar metadatos
                TotalCount = result.TotalCount;

                // Aplicar filtros locales
                ApplyFilter();

                // Reactivar eventos
                SubscribeToEvents();

                // Notificar cambios a DevExpress para actualización de UI
                this.RaisePropertyChanged(x => x.Entities);
                this.RaisePropertyChanged(x => x.FilteredEntities);
                this.RaisePropertyChanged(x => x.TotalCount);
                this.RaisePropertyChanged(x => x.HasActiveFilters);

                // Restablecer estados
                HasChanges = false;

                // Callback para lógica específica
                OnEntitiesLoaded(_entities);

                Logger.Debug($"Actualización de UI completada. {_entities.Count} entidades cargadas, {FilteredEntities.Count} mostradas después de filtros");
            }
            catch (Exception ex)
            {
                Logger.Error("Error crítico actualizando entidades en UI", ex);
                throw;
            }
        }

        #endregion

        #region Control de Cancelación y Estados

        /// <summary>
        /// Cancela la operación actual si existe.
        /// </summary>
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
        /// Determina si se puede ejecutar el comando de carga.
        /// VIRTUAL para permitir personalización en clases derivadas.
        /// 
        /// DevExpress usa este método automáticamente para CanExecute del comando.
        /// </summary>
        /// <returns>true si se puede cargar, false en caso contrario</returns>
        public virtual bool CanLoadAsync()
        {
            return CurrentState != ViewModelState.Loading &&
                   CurrentState != ViewModelState.Saving &&
                   CurrentState != ViewModelState.Initializing;
        }

        #endregion

        #region Operaciones CRUD - Comandos DevExpress con Validación ABP

        /// <summary>
        /// Comando para crear nueva entidad.
        /// 
        /// IMPORTANTE: La validación ocurre en el servicio de aplicación via interceptores ABP.
        /// Este ViewModel NO realiza validación de entrada para evitar duplicación.
        /// Solo valida reglas de UI/negocio que no están en el servicio.
        /// 
        /// DevExpress generará automáticamente CreateAsyncCommand.
        /// </summary>
        /// <param name="input">Datos de entrada para creación</param>
        [Command]
        public virtual async Task CreateAsync(TCreateInput input)
        {
            if (!CanCreate(input))
                return;

            try
            {
                ChangeState(ViewModelState.Saving, "Creando nueva entidad");
                IsSaving = true;

                Logger.Debug($"Iniciando creación de {typeof(TEntityDto).Name}");

                // Validaciones de UI específicas (no de datos - eso lo hace ABP)
                if (!await ValidateForCreateAsync(input))
                    return;

                // Llamar servicio - las validaciones de datos ocurren aquí via ABP interceptors
                var result = await CreateEntityAsync(input);

                if (result != null)
                {
                    // Agregar a colección local para UI responsiva
                    await DispatcherService.BeginInvoke(() =>
                    {
                        Entities.Add(result);
                        SelectedEntity = result;
                    });

                    await ShowSuccessMessageAsync(L("EntityCreatedSuccessfully"));
                    OnEntityCreated(result);

                    Logger.Info($"Entidad {typeof(TEntityDto).Name} creada exitosamente. ID: {result.Id}");
                }

                ChangeState(ViewModelState.Ready, "Creación completada");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error creando {typeof(TEntityDto).Name}", ex);
                await HandleCreateErrorAsync(ex);
                ChangeState(ViewModelState.Ready, "Error en creación");
            }
            finally
            {
                IsSaving = false;
            }
        }

        /// <summary>
        /// Comando para actualizar entidad existente.
        /// 
        /// NOTA: Mismos principios que CreateAsync - validación en servicio via ABP.
        /// </summary>
        /// <param name="input">Datos de entrada para actualización</param>
        [Command]
        public virtual async Task UpdateAsync(TUpdateInput input)
        {
            if (!CanUpdate(input))
                return;

            try
            {
                ChangeState(ViewModelState.Saving, "Actualizando entidad");
                IsSaving = true;

                Logger.Debug($"Iniciando actualización de {typeof(TEntityDto).Name} ID: {input.Id}");

                // Validaciones de UI específicas
                if (!await ValidateForUpdateAsync(input))
                    return;

                // Llamar servicio - validaciones de datos via ABP
                var result = await UpdateEntityAsync(input);

                if (result != null)
                {
                    // Actualizar en colección local
                    await DispatcherService.BeginInvoke(() =>
                    {
                        var index = Entities.ToList().FindIndex(e => e.Id.Equals(result.Id));
                        if (index >= 0)
                        {
                            Entities[index] = result;
                            SelectedEntity = result;
                        }
                    });

                    await ShowSuccessMessageAsync(L("EntityUpdatedSuccessfully"));
                    OnEntityUpdated(result);

                    Logger.Info($"Entidad {typeof(TEntityDto).Name} actualizada exitosamente. ID: {result.Id}");
                }

                ChangeState(ViewModelState.Ready, "Actualización completada");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error actualizando {typeof(TEntityDto).Name}", ex);
                await HandleUpdateErrorAsync(ex);
                ChangeState(ViewModelState.Ready, "Error en actualización");
            }
            finally
            {
                IsSaving = false;
                EndEditing(true);
            }
        }

        /// <summary>
        /// Comando para eliminar entidad.
        /// 
        /// NOTA: Incluye confirmación de usuario antes de proceder.
        /// </summary>
        /// <param name="input">Datos de entrada para eliminación</param>
        [Command]
        public virtual async Task DeleteAsync(TDeleteInput input)
        {
            if (!CanDelete(input))
                return;

            // Confirmar eliminación con el usuario
            var confirmationMessage = await GetDeleteConfirmationMessageAsync(input);
            var result = await ShowConfirmationAsync(confirmationMessage);

            if (result != MessageResult.Yes)
                return;

            try
            {
                ChangeState(ViewModelState.Saving, "Eliminando entidad");
                IsSaving = true;

                Logger.Debug($"Iniciando eliminación de {typeof(TEntityDto).Name} ID: {input.Id}");

                // Validaciones previas a eliminación
                if (!await ValidateForDeleteAsync(input))
                    return;

                // Llamar servicio
                await DeleteEntityAsync(input);

                // Remover de colección local
                await DispatcherService.BeginInvoke(() =>
                {
                    var entityToRemove = Entities.FirstOrDefault(e => e.Id.Equals(input.Id));
                    if (entityToRemove != null)
                    {
                        Entities.Remove(entityToRemove);

                        // Limpiar selección si era la entidad eliminada
                        if (SelectedEntity?.Id.Equals(input.Id) == true)
                        {
                            SelectedEntity = null;
                        }
                    }
                });

                await ShowSuccessMessageAsync(L("EntityDeletedSuccessfully"));
                OnEntityDeleted(input);

                Logger.Info($"Entidad {typeof(TEntityDto).Name} eliminada exitosamente. ID: {input.Id}");

                ChangeState(ViewModelState.Ready, "Eliminación completada");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error eliminando {typeof(TEntityDto).Name}", ex);
                await HandleDeleteErrorAsync(ex);
                ChangeState(ViewModelState.Ready, "Error en eliminación");
            }
            finally
            {
                IsSaving = false;
            }
        }

        #endregion

        #region Métodos Abstractos para Operaciones CRUD

        /// <summary>
        /// Crea una nueva entidad usando el servicio de aplicación.
        /// Debe ser implementado por cada ViewModel específico.
        /// 
        /// IMPORTANTE: Este método se ejecuta en un contexto UnitOfWork automático.
        /// Las validaciones de entrada ocurren automáticamente via interceptores ABP.
        /// </summary>
        /// <param name="input">Datos de entrada validados por ABP</param>
        /// <returns>Entidad creada</returns>
        protected abstract ValueTask<TEntityDto> CreateEntityAsync(TCreateInput input);

        /// <summary>
        /// Actualiza una entidad existente usando el servicio de aplicación.
        /// </summary>
        /// <param name="input">Datos de entrada validados por ABP</param>
        /// <returns>Entidad actualizada</returns>
        protected abstract ValueTask<TEntityDto> UpdateEntityAsync(TUpdateInput input);

        /// <summary>
        /// Elimina una entidad del repositorio usando el servicio de aplicación.
        /// </summary>
        /// <param name="input">Datos de entrada validados por ABP</param>
        protected abstract ValueTask DeleteEntityAsync(TDeleteInput input);

        #endregion

        #region Validación de Operaciones - CanExecute Methods (VIRTUALES)

        /// <summary>
        /// Determina si se puede ejecutar la operación de creación.
        /// VIRTUAL para permitir personalización en clases derivadas.
        /// 
        /// IMPORTANTE: Esta validación es para reglas de UI/estado, NO para datos.
        /// Las validaciones de datos las hace ABP automáticamente.
        /// </summary>
        /// <param name="input">Entrada a validar (puede ser null)</param>
        /// <returns>true si se puede crear, false en caso contrario</returns>
        public virtual bool CanCreate(TCreateInput input) =>
            CanExecuteOperations &&
            CurrentState != ViewModelState.Loading &&
            !IsLoading &&
            !IsSaving;

        /// <summary>
        /// Determina si se puede ejecutar la operación de actualización.
        /// VIRTUAL para permitir personalización en clases derivadas.
        /// 
        /// REGLAS BASE:
        /// - No debe estar cargando o guardando
        /// - Debe tener una entidad válida
        /// - El estado debe permitir operaciones
        /// </summary>
        /// <param name="input">Entrada a validar (puede ser null)</param>
        /// <returns>true si se puede actualizar, false en caso contrario</returns>
        public virtual bool CanUpdate(TUpdateInput input) =>
            CanExecuteOperations &&
            input != null &&
            !IsLoading &&
            !IsSaving;

        /// <summary>
        /// Determina si se puede ejecutar la operación de eliminación.
        /// VIRTUAL para permitir personalización en clases derivadas.
        /// 
        /// REGLAS BASE:
        /// - No debe estar cargando o guardando  
        /// - Debe tener una entidad válida
        /// - El estado debe permitir operaciones
        /// </summary>
        /// <param name="input">Entrada a validar (puede ser null)</param>
        /// <returns>true si se puede eliminar, false en caso contrario</returns>
        public virtual bool CanDelete(TDeleteInput input) =>
            CanExecuteOperations &&
            input != null &&
            !IsLoading &&
            !IsSaving;

        /// <summary>
        /// Determina si se puede ejecutar la operación de guardado.
        /// VIRTUAL para permitir personalización.
        /// </summary>
        /// <returns>true si se puede guardar, false en caso contrario</returns>
        public virtual bool CanSave() =>
            CanExecuteOperations &&
            HasChanges &&
            !IsSaving;

        /// <summary>
        /// Determina si se puede ejecutar la operación de reset.
        /// VIRTUAL para permitir personalización.
        /// </summary>
        /// <returns>true si se puede resetear, false en caso contrario</returns>
        public virtual bool CanReset() =>
            CanExecuteOperations &&
            (HasChanges || IsEditing);

        #endregion

        #region Validaciones de UI/Negocio (NO de datos - eso lo hace ABP)

        /// <summary>
        /// Valida reglas de UI específicas antes de crear.
        /// VIRTUAL para personalización en clases derivadas.
        /// 
        /// IMPORTANTE: NO validar datos de entrada aquí - eso lo hace ABP.
        /// Solo validar reglas de UI como:
        /// - Estados del ViewModel
        /// - Permisos específicos
        /// - Reglas de negocio complejas que requieren UI
        /// </summary>
        /// <param name="input">Entrada a validar</param>
        /// <returns>true si pasa las validaciones de UI</returns>
        protected virtual async Task<bool> ValidateForCreateAsync(TCreateInput input)
        {
            // Validaciones base de UI
            if (!CanExecuteOperations)
            {
                await ShowWarningAsync(L("OperationNotAllowedInCurrentState"));
                return false;
            }

            // Las clases derivadas pueden sobrescribir para validaciones específicas
            return true;
        }

        /// <summary>
        /// Valida reglas de UI específicas antes de actualizar.
        /// VIRTUAL para personalización.
        /// </summary>
        /// <param name="input">Entrada a validar</param>
        /// <returns>true si pasa las validaciones de UI</returns>
        protected virtual async Task<bool> ValidateForUpdateAsync(TUpdateInput input)
        {
            if (!CanExecuteOperations)
            {
                await ShowWarningAsync(L("OperationNotAllowedInCurrentState"));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida reglas de UI específicas antes de eliminar.
        /// VIRTUAL para personalización.
        /// </summary>
        /// <param name="input">Entrada a validar</param>
        /// <returns>true si pasa las validaciones de UI</returns>
        protected virtual async Task<bool> ValidateForDeleteAsync(TDeleteInput input)
        {
            if (!CanExecuteOperations)
            {
                await ShowWarningAsync(L("OperationNotAllowedInCurrentState"));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Obtiene el mensaje de confirmación para eliminación.
        /// VIRTUAL para personalización del mensaje.
        /// </summary>
        /// <param name="input">Entidad a eliminar</param>
        /// <returns>Mensaje de confirmación</returns>
        protected virtual async Task<string> GetDeleteConfirmationMessageAsync(TDeleteInput input)
        {
            // Mensaje base - las clases derivadas pueden personalizar
            return L("ConfirmDeleteEntity");
        }

        #endregion

        #region Sistema de Filtrado Avanzado

        /// <summary>
        /// Aplica todos los filtros a la colección principal.
        /// Optimizado para rendimiento con colecciones grandes.
        /// </summary>
        private void ApplyFilter()
        {
            try
            {
                Logger.Debug($"Aplicando filtros. SearchText: '{SearchText}', Entidades: {Entities.Count}");

                var filtered = FilterEntities(Entities, SearchText, Filter);
                var sorted = SortEntities(filtered);

                // Actualizar colección filtrada de forma eficiente
                FilteredEntities.Clear();
                foreach (var entity in sorted)
                {
                    FilteredEntities.Add(entity);
                }

                Logger.Debug($"Filtros aplicados. Resultados: {FilteredEntities.Count}/{Entities.Count}");

                OnFilterApplied();
            }
            catch (Exception ex)
            {
                Logger.Error("Error aplicando filtros", ex);
            }
        }

        /// <summary>
        /// Filtra las entidades basándose en texto de búsqueda y filtros personalizados.
        /// VIRTUAL para permitir personalización completa en clases derivadas.
        /// </summary>
        /// <param name="entities">Entidades a filtrar</param>
        /// <param name="searchText">Texto de búsqueda</param>
        /// <param name="customFilter">Filtros estructurados</param>
        /// <returns>Entidades filtradas</returns>
        protected virtual IEnumerable<TEntityDto> FilterEntities(
            IEnumerable<TEntityDto> entities,
            string searchText,
            TGetAllInput customFilter)
        {
            var result = entities;

            // Filtro por texto de búsqueda rápida
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                result = result.Where(entity => EntityMatchesSearchText(entity, searchText));
            }

            // Filtros estructurados personalizados
            if (customFilter != null)
            {
                result = ApplyCustomFilters(result, customFilter);
            }

            return result;
        }

        /// <summary>
        /// Determina si una entidad coincide con el texto de búsqueda.
        /// VIRTUAL para personalización completa.
        /// 
        /// Implementación por defecto: busca en todas las propiedades string públicas.
        /// </summary>
        /// <param name="entity">Entidad a evaluar</param>
        /// <param name="searchText">Texto de búsqueda</param>
        /// <returns>true si coincide, false en caso contrario</returns>
        protected virtual bool EntityMatchesSearchText(TEntityDto entity, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText) || entity == null)
                return true;

            var search = searchText.ToLowerInvariant();

            try
            {
                // Implementación por defecto: buscar en todas las propiedades string
                var stringProperties = typeof(TEntityDto).GetProperties()
                    .Where(p => p.PropertyType == typeof(string) &&
                               p.CanRead &&
                               p.GetIndexParameters().Length == 0) // No indexers
                    .Select(p => {
                        try
                        {
                            return p.GetValue(entity) as string;
                        }
                        catch
                        {
                            return null; // Ignorar propiedades que fallan
                        }
                    })
                    .Where(value => !string.IsNullOrEmpty(value));

                return stringProperties.Any(value =>
                    value.ToLowerInvariant().Contains(search));
            }
            catch (Exception ex)
            {
                Logger.Debug($"Error en búsqueda de texto para entidad {entity.Id}", ex);
                return false;
            }
        }

        /// <summary>
        /// Aplica filtros personalizados específicos del dominio.
        /// VIRTUAL para implementación en clases derivadas.
        /// 
        /// EJEMPLO:
        /// <code>
        /// protected override IEnumerable&lt;ProductDto&gt; ApplyCustomFilters(
        ///     IEnumerable&lt;ProductDto&gt; entities, 
        ///     GetAllProductsInput filter)
        /// {
        ///     if (filter.CategoryId.HasValue)
        ///         entities = entities.Where(p =&gt; p.CategoryId == filter.CategoryId);
        ///     
        ///     return entities;
        /// }
        /// </code>
        /// </summary>
        /// <param name="entities">Entidades a filtrar</param>
        /// <param name="filter">Filtros estructurados</param>
        /// <returns>Entidades filtradas</returns>
        protected virtual IEnumerable<TEntityDto> ApplyCustomFilters(
            IEnumerable<TEntityDto> entities,
            TGetAllInput filter)
        {
            return entities;
        }

        /// <summary>
        /// Ordena las entidades filtradas.
        /// VIRTUAL para personalización del ordenamiento.
        /// </summary>
        /// <param name="entities">Entidades a ordenar</param>
        /// <returns>Entidades ordenadas</returns>
        protected virtual IEnumerable<TEntityDto> SortEntities(IEnumerable<TEntityDto> entities)
        {
            return entities.OrderBy(GetSortKeySelector());
        }

        /// <summary>
        /// Obtiene el selector para ordenamiento por defecto.
        /// VIRTUAL para personalización.
        /// </summary>
        /// <returns>Función de ordenamiento</returns>
        protected virtual Func<TEntityDto, object> GetSortKeySelector()
        {
            return entity => entity.Id;
        }

        /// <summary>
        /// Verifica si hay filtros personalizados activos.
        /// VIRTUAL para lógica específica en clases derivadas.
        /// </summary>
        /// <returns>true si hay filtros activos</returns>
        protected virtual bool HasCustomFilters()
        {
            return Filter != null;
        }

        /// <summary>
        /// Comando para aplicar filtros manualmente.
        /// Útil para botones "Buscar" en la UI.
        /// </summary>
        [Command]
        public virtual void ApplyFilterCommand()
        {
            if (!CanApplyFilter())
                return;

            Logger.Debug("Aplicando filtros por comando manual");
            ApplyFilter();
        }

        /// <summary>
        /// Comando para limpiar todos los filtros.
        /// </summary>
        [Command]
        public virtual void ClearFilterCommand()
        {
            if (!CanClearFilter())
                return;

            Logger.Debug("Limpiando filtros por comando manual");

            SearchText = string.Empty;
            Filter = default;

            ApplyFilter();

            Logger.Debug("Filtros limpiados exitosamente");
        }

        /// <summary>
        /// Determina si se pueden aplicar filtros.
        /// VIRTUAL para personalización.
        /// </summary>
        public virtual bool CanApplyFilter() => CanExecuteOperations;

        /// <summary>
        /// Determina si se pueden limpiar filtros.
        /// VIRTUAL para personalización.
        /// </summary>
        public virtual bool CanClearFilter() => CanExecuteOperations && HasActiveFilters;

        /// <summary>
        /// Llamado después de aplicar filtros.
        /// VIRTUAL para acciones personalizadas.
        /// </summary>
        protected virtual void OnFilterApplied()
        {
        }

        #endregion

        #region Gestión de Edición

        /// <summary>
        /// Inicia el proceso de edición para una entidad.
        /// </summary>
        /// <param name="entity">Entidad a editar</param>
        protected virtual void StartEditing(TUpdateInput entity)
        {
            if (entity == null) return;

            EditingEntity = entity;
            IsEditing = true;
            ChangeState(ViewModelState.Editing, $"Iniciando edición de entidad {entity.Id}");

            Logger.Debug($"Iniciada edición de {typeof(TEntityDto).Name} ID: {entity.Id}");
        }

        /// <summary>
        /// Termina el proceso de edición.
        /// </summary>
        /// <param name="successful">Indica si la edición fue exitosa</param>
        protected virtual void EndEditing(bool successful)
        {
            var entityId = EditingEntity != null ? EditingEntity.Id : default;

            EditingEntity = default;
            IsEditing = false;

            if (CurrentState == ViewModelState.Editing)
            {
                ChangeState(ViewModelState.Ready, $"Edición terminada. Exitosa: {successful}");
            }

            if (successful)
            {
                HasChanges = true;
            }

            Logger.Debug($"Terminada edición de {typeof(TEntityDto).Name} ID: {entityId}. Exitosa: {successful}");
        }

        #endregion

        #region Eventos y Callbacks Virtuales

        /// <summary>
        /// Llamado cuando los datos se cargan exitosamente.
        /// VIRTUAL para lógica específica en clases derivadas.
        /// </summary>
        /// <param name="result">Datos cargados</param>
        protected virtual void OnDataLoadedSuccessfully(PagedResultDto<TEntityDto> result)
        {
            // Las clases derivadas pueden sobrescribir para lógica específica
        }

        /// <summary>
        /// Llamado cuando se cancela la carga de datos.
        /// VIRTUAL para lógica específica.
        /// </summary>
        protected virtual void OnDataLoadCancelled()
        {
            // Las clases derivadas pueden sobrescribir
        }

        /// <summary>
        /// Llamado después de cargar entidades exitosamente.
        /// VIRTUAL para lógica específica.
        /// </summary>
        /// <param name="entities">Entidades cargadas</param>
        protected virtual void OnEntitiesLoaded(IEnumerable<TEntityDto> entities)
        {
            // Las clases derivadas pueden sobrescribir
        }

        /// <summary>
        /// Llamado después de crear una entidad exitosamente.
        /// VIRTUAL para lógica específica.
        /// </summary>
        /// <param name="entity">Entidad creada</param>
        protected virtual void OnEntityCreated(TEntityDto entity)
        {
            // Ejemplo: publicar evento en EventBus
            // EventBus.Trigger(new EntityCreatedEvent<TEntityDto> { Entity = entity });
        }

        /// <summary>
        /// Llamado después de actualizar una entidad exitosamente.
        /// VIRTUAL para lógica específica.
        /// </summary>
        /// <param name="entity">Entidad actualizada</param>
        protected virtual void OnEntityUpdated(TEntityDto entity)
        {
            // EventBus.Trigger(new EntityUpdatedEvent<TEntityDto> { Entity = entity });
        }

        /// <summary>
        /// Llamado después de eliminar una entidad exitosamente.
        /// VIRTUAL para lógica específica.
        /// </summary>
        /// <param name="input">Datos de la entidad eliminada</param>
        protected virtual void OnEntityDeleted(TDeleteInput input)
        {
            // EventBus.Trigger(new EntityDeletedEvent<TEntityDto> { EntityId = input.Id });
        }

        /// <summary>
        /// Llamado cuando cambia la entidad seleccionada.
        /// VIRTUAL para lógica específica.
        /// </summary>
        protected virtual void OnSelectedEntityChanged()
        {
            // Actualizar comandos que dependen de la selección
            this.RaiseCanExecuteChanged(x => x.UpdateAsync(null));
            this.RaiseCanExecuteChanged(x => x.DeleteAsync(null));
        }

        #endregion

        #region Manejo de Errores ABP-Aware

        /// <summary>
        /// Maneja errores durante la carga de datos.
        /// Reconoce tipos de excepción ABP y los maneja apropiadamente.
        /// </summary>
        /// <param name="ex">Excepción ocurrida</param>
        protected virtual async Task HandleLoadErrorAsync(Exception ex)
        {
            await HandleGenericErrorAsync(ex, "ErrorLoadingData");
        }

        /// <summary>
        /// Maneja errores durante la creación de entidades.
        /// </summary>
        /// <param name="ex">Excepción ocurrida</param>
        protected virtual async Task HandleCreateErrorAsync(Exception ex)
        {
            await HandleGenericErrorAsync(ex, "ErrorCreatingEntity");
        }

        /// <summary>
        /// Maneja errores durante la actualización de entidades.
        /// </summary>
        /// <param name="ex">Excepción ocurrida</param>
        protected virtual async Task HandleUpdateErrorAsync(Exception ex)
        {
            await HandleGenericErrorAsync(ex, "ErrorUpdatingEntity");
        }

        /// <summary>
        /// Maneja errores durante la eliminación de entidades.
        /// </summary>
        /// <param name="ex">Excepción ocurrida</param>
        protected virtual async Task HandleDeleteErrorAsync(Exception ex)
        {
            await HandleGenericErrorAsync(ex, "ErrorDeletingEntity");
        }

        /// <summary>
        /// Manejo genérico de errores con reconocimiento de tipos ABP.
        /// </summary>
        /// <param name="ex">Excepción</param>
        /// <param name="fallbackMessageKey">Clave de mensaje por defecto</param>
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
                    severity = LogSeverity.Warn;
                    break;

                case UserFriendlyException userFriendlyEx:
                    // Errores amigables para el usuario
                    message = userFriendlyEx.Message;
                    severity = LogSeverity.Warn;
                    break;

                case KontecgAuthorizationException authEx:
                    // Errores de autorización
                    message = L("NotAuthorizedForThisOperation");
                    severity = LogSeverity.Warn;
                    break;

                case TimeoutException:
                    // Timeout
                    message = L("OperationTimedOut");
                    severity = LogSeverity.Warn;
                    break;

                default:
                    // Error genérico
                    message = L(fallbackMessageKey);
                    severity = LogSeverity.Error;
                    break;
            }

            await ShowErrorAsync(message, severity);
        }

        /// <summary>
        /// Formatea errores de validación ABP para mostrar al usuario.
        /// </summary>
        /// <param name="validationEx">Excepción de validación</param>
        /// <returns>Mensaje formateado</returns>
        private string FormatValidationErrors(KontecgValidationException validationEx)
        {
            if (validationEx.ValidationErrors?.Any() == true)
            {
                var errors = validationEx.ValidationErrors
                    .Select(e => $"• {e.MemberNames.FirstOrDefault()}: {e.ErrorMessage}")
                    .Take(5); // Limitar a 5 errores para no sobrecargar UI

                return $"{L("ValidationErrors")}:\n{string.Join("\n", errors)}";
            }

            return validationEx.Message;
        }

        /// <summary>
        /// Muestra un mensaje de error usando servicios DevExpress.
        /// Integrado con sistema de logging ABP.
        /// </summary>
        /// <param name="message">Mensaje a mostrar</param>
        /// <param name="severity">Severidad del error</param>
        protected async Task ShowErrorAsync(string message, LogSeverity severity)
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

        /// <summary>
        /// Muestra un mensaje de advertencia.
        /// </summary>
        /// <param name="message">Mensaje de advertencia</param>
        protected async Task ShowWarningAsync(string message)
        {
            await DispatcherService.BeginInvoke(() =>
            {
                MessageBoxService.ShowMessage(
                    message,
                    L("Warning"),
                    MessageButton.OK,
                    MessageIcon.Warning);
            });
        }

        /// <summary>
        /// Muestra un mensaje de éxito.
        /// </summary>
        /// <param name="message">Mensaje de éxito</param>
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

        /// <summary>
        /// Muestra un diálogo de confirmación.
        /// </summary>
        /// <param name="message">Mensaje de confirmación</param>
        /// <returns>Resultado de la confirmación</returns>
        protected async Task<MessageResult> ShowConfirmationAsync(string message)
        {
            return await DispatcherService.BeginInvoke(() =>
                MessageBoxService.ShowMessage(
                    message,
                    L("Confirmation"),
                    MessageButton.YesNo,
                    MessageIcon.Question));
        }

        /// <summary>
        /// Obtiene el título de ventana apropiado según la severidad.
        /// </summary>
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

        /// <summary>
        /// Obtiene el icono apropiado según la severidad.
        /// </summary>
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

        #endregion

        #region Comandos Adicionales DevExpress

        /// <summary>
        /// Comando para actualizar manualmente la entidad seleccionada.
        /// Útil para refrescar binding después de cambios externos.
        /// </summary>
        [Command]
        public virtual void UpdateSelectedEntityCommand() =>
            this.RaisePropertyChanged(x => x.SelectedEntity);

        /// <summary>
        /// Comando para cerrar la vista/documento actual.
        /// </summary>
        [Command]
        public virtual void Close()
        {
            DocumentOwner?.Close(this);
        }

        /// <summary>
        /// Comando de guardado genérico.
        /// Las clases derivadas deben sobrescribir para implementar lógica específica.
        /// </summary>
        [Display(AutoGenerateField = false)]
        public virtual void Save()
        {
            // Implementación base vacía
            // Las clases derivadas pueden sobrescribir para lógica de guardado
            Logger.Debug("Comando Save ejecutado - implementación base");
        }

        /// <summary>
        /// Comando de reset genérico.
        /// Las clases derivadas deben sobrescribir para implementar lógica específica.
        /// </summary>
        [Display(AutoGenerateField = false)]
        public virtual void Reset()
        {
            HasChanges = false;
            IsEditing = false;
            EditingEntity = default;
            NewEntity = default;

            Logger.Debug("Comando Reset ejecutado - estado base reseteado");
        }

        #endregion

        #region Servicios DevExpress

        /// <summary>
        /// Servicio para mostrar mensajes de diálogo.
        /// </summary>
        protected IMessageBoxService MessageBoxService =>
            this.GetService<IMessageBoxService>();

        /// <summary>
        /// Servicio para manejo de documentos/ventanas.
        /// </summary>
        protected IDocumentManagerService DocumentManagerService =>
            this.GetService<IDocumentManagerService>();

        /// <summary>
        /// Servicio para sincronización con hilo de UI.
        /// </summary>
        protected IDispatcherService DispatcherService =>
            this.GetService<IDispatcherService>();

        /// <summary>
        /// Servicio para diálogos personalizados.
        /// </summary>
        protected IDialogService DialogService =>
            this.GetService<IDialogService>();

        /// <summary>
        /// Servicio para notificaciones no bloqueantes.
        /// </summary>
        protected INotificationService NotificationService =>
            this.GetService<INotificationService>();

        #endregion

        #region Interfaz IDocumentContent

        protected IDocumentOwner DocumentOwner { get; private set; }

        object IDocumentContent.Title => GetDocumentTitle();

        /// <summary>
        /// Obtiene el título del documento/ventana.
        /// VIRTUAL para personalización en clases derivadas.
        /// </summary>
        /// <returns>Título del documento</returns>
        protected virtual object GetDocumentTitle()
        {
            var entityName = typeof(TEntityDto).Name.Replace("Dto", "");
            var count = FilteredEntities?.Count ?? 0;

            return HasActiveFilters ?
                L("FilteredEntitiesTitle", entityName, count) :
                L("EntitiesTitle", entityName, count);
        }

        /// <summary>
        /// Llamado cuando se intenta cerrar el documento.
        /// Maneja cambios sin guardar.
        /// </summary>
        /// <param name="e">Argumentos del evento</param>
        protected virtual void OnClose(CancelEventArgs e)
        {
            if (HasChanges)
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

        void IDocumentContent.OnClose(CancelEventArgs e) => OnClose(e);

        void IDocumentContent.OnDestroy() => OnDestroy();

        IDocumentOwner IDocumentContent.DocumentOwner
        {
            get => DocumentOwner;
            set => DocumentOwner = value;
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
            Logger.Debug($"ViewModel padre establecido: {ParentViewModel?.GetType().Name}");
        }

        public bool AllowSaveReset { get; protected set; }

        #endregion

        #region Cleanup y Disposable

        /// <summary>
        /// Llamado cuando el ViewModel se destruye.
        /// Limpia recursos y cancela operaciones pendientes.
        /// </summary>
        protected virtual void OnDestroy()
        {
            try
            {
                Logger.Debug($"Destruyendo {GetType().Name}");

                // Cancelar operaciones pendientes
                CancelCurrentOperationAsync().GetAwaiter().GetResult();

                // Desuscribirse de eventos
                UnsubscribeFromEvents();

                // Limpiar referencias
                SelectedEntity = null;
                SelectedEntities?.Clear();
                EditingEntity = default;
                NewEntity = default;

                Logger.Debug($"{GetType().Name} destruido correctamente");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error destruyendo {GetType().Name}", ex);
            }
        }

        #endregion

        #region Propiedades de Soporte para UI

        /// <summary>
        /// Indica si hay entidades para mostrar.
        /// Útil para mostrar/ocultar mensajes de "sin datos".
        /// </summary>
        public virtual bool HasEntities => FilteredEntities?.Count > 0;

        /// <summary>
        /// Indica si hay una selección válida.
        /// Útil para habilitar/deshabilitar comandos dependientes.
        /// </summary>
        public virtual bool HasSelection => SelectedEntity != null;

        /// <summary>
        /// Indica si hay selección múltiple.
        /// Útil para operaciones en lote.
        /// </summary>
        public virtual bool HasMultipleSelection => SelectedEntities?.Count > 1;

        /// <summary>
        /// Mensaje de estado para mostrar en la UI.
        /// Se actualiza automáticamente según el estado del ViewModel.
        /// </summary>
        public virtual string StatusMessage => GetStatusMessage();

        /// <summary>
        /// Obtiene el mensaje de estado actual.
        /// VIRTUAL para personalización en clases derivadas.
        /// </summary>
        /// <returns>Mensaje de estado</returns>
        protected virtual string GetStatusMessage()
        {
            return CurrentState switch
            {
                ViewModelState.Loading => L("LoadingData"),
                ViewModelState.Saving => L("SavingData"),
                ViewModelState.Editing => L("EditingEntity"),
                ViewModelState.Error => L("ErrorOccurred"),
                ViewModelState.Ready when !HasEntities => L("NoDataAvailable"),
                ViewModelState.Ready when HasActiveFilters => L("FilteredResults", FilteredEntities.Count, TotalCount),
                ViewModelState.Ready => L("EntitiesLoaded", FilteredEntities.Count),
                _ => string.Empty
            };
        }

        #endregion
    }
}
#endif