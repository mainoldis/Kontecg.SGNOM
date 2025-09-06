#if false
// ==================================================================================
// EXTENSIÓN DE COLLECTIONVIEWMODELBASE CON PERMISOS ABP
// ==================================================================================

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using Kontecg.Authorization;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm.POCO;

namespace Kontecg.ViewModels
{
    /// <summary>
    /// Extensión de CollectionViewModelBase con integración completa de permisos ABP.
    /// 
    /// CARACTERÍSTICAS DE PERMISOS:
    /// - Verificación automática de permisos en comandos CRUD
    /// - Cache de permisos para mejor rendimiento
    /// - Invalidación automática cuando cambian permisos
    /// - Soporte para permisos dinámicos y condicionales
    /// - Integración con roles y características ABP
    /// </summary>
    public abstract partial class CollectionViewModelBase<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput, TService>
    {
        #region Propiedades de Permisos - Cached para Performance

        /// <summary>
        /// Cache de permisos verificados para evitar múltiples consultas.
        /// Se invalida automáticamente cuando es necesario.
        /// </summary>
        protected Dictionary<string, bool> PermissionCache = new();

        /// <summary>
        /// Timestamp del último refresh de permisos para control de caché.
        /// </summary>
        protected DateTime LastPermissionRefresh = DateTime.MinValue;

        /// <summary>
        /// Tiempo de vida del cache de permisos en minutos.
        /// </summary>
        protected virtual int PermissionCacheLifetimeMinutes => 5;

        /// <summary>
        /// Indica si los permisos están siendo verificados actualmente.
        /// Evita verificaciones simultáneas.
        /// </summary>
        public virtual bool IsCheckingPermissions { get; protected set; }

        #endregion

        #region Permisos Base del Dominio - VIRTUAL para personalización

        /// <summary>
        /// Nombre base para permisos de esta entidad.
        /// Por defecto usa el nombre del DTO sin 'Dto'.
        /// VIRTUAL para personalización en clases derivadas.
        /// 
        /// EJEMPLO: Para ProductDto retorna "Products"
        /// </summary>
        protected virtual string BasePermissionName => typeof(TEntityDto).Name.Replace("Dto", "");

        /// <summary>
        /// Permiso para ver/listar entidades.
        /// VIRTUAL para personalización.
        /// </summary>
        protected virtual string ViewPermissionName => $"{BasePermissionName}.View";

        /// <summary>
        /// Permiso para crear entidades.
        /// VIRTUAL para personalización.
        /// </summary>
        protected virtual string CreatePermissionName => $"{BasePermissionName}.Create";

        /// <summary>
        /// Permiso para actualizar entidades.
        /// VIRTUAL para personalización.
        /// </summary>
        protected virtual string UpdatePermissionName => $"{BasePermissionName}.Update";

        /// <summary>
        /// Permiso para eliminar entidades.
        /// VIRTUAL para personalización.
        /// </summary>
        protected virtual string DeletePermissionName => $"{BasePermissionName}.Delete";

        /// <summary>
        /// Permiso para exportar datos.
        /// VIRTUAL para personalización.
        /// </summary>
        protected virtual string ExportPermissionName => $"{BasePermissionName}.Export";

        #endregion

        #region Propiedades de Estado de Permisos - DevExpress Intercepted

        /// <summary>
        /// Indica si el usuario puede ver/listar entidades.
        /// Se actualiza automáticamente y se usa para binding de UI.
        /// </summary>
        public virtual bool CanViewEntities { get; protected set; }

        /// <summary>
        /// Indica si el usuario puede crear nuevas entidades.
        /// </summary>
        public virtual bool CanCreateEntities { get; protected set; }

        /// <summary>
        /// Indica si el usuario puede actualizar entidades.
        /// </summary>
        public virtual bool CanUpdateEntities { get; protected set; }

        /// <summary>
        /// Indica si el usuario puede eliminar entidades.
        /// </summary>
        public virtual bool CanDeleteEntities { get; protected set; }

        /// <summary>
        /// Indica si el usuario puede exportar datos.
        /// </summary>
        public virtual bool CanExportEntities { get; protected set; }

        /// <summary>
        /// Lista de permisos específicos adicionales.
        /// Para funcionalidades específicas del dominio.
        /// </summary>
        public virtual ObservableCollection<string> GrantedPermissions { get; protected set; } = new();

        #endregion

        #region Inicialización de Permisos

        /// <summary>
        /// OVERRIDE: Incluir verificación de permisos en la inicialización.
        /// </summary>
        protected override void OnInitializeInRuntime()
        {
            // Verificar permisos antes de cualquier otra inicialización
            _ = InitializePermissionsAsync();

            // Continuar con inicialización base
            base.OnInitializeInRuntime();
        }

        /// <summary>
        /// Inicializa y verifica todos los permisos necesarios.
        /// Se ejecuta de forma asíncrona para no bloquear la UI.
        /// </summary>
        protected virtual async Task InitializePermissionsAsync()
        {
            try
            {
                Logger.Debug($"Inicializando permisos para {typeof(TEntityDto).Name}");
                IsCheckingPermissions = true;

                // Verificar permisos base
                await RefreshPermissionsAsync();

                // Verificar permisos específicos del dominio
                await InitializeDomainSpecificPermissionsAsync();

                // Notificar cambios a la UI
                NotifyPermissionPropertiesChanged();

                // Actualizar estado de comandos
                RefreshCommandStates();

                Logger.Info($"Permisos inicializados para {typeof(TEntityDto).Name}. CanView: {CanViewEntities}, CanCreate: {CanCreateEntities}");
            }
            catch (Exception ex)
            {
                Logger.Error("Error inicializando permisos", ex);

                // En caso de error, denegar todos los permisos por seguridad
                SetAllPermissions(false);
            }
            finally
            {
                IsCheckingPermissions = false;
            }
        }

        /// <summary>
        /// Inicializa permisos específicos del dominio.
        /// VIRTUAL para implementación en clases derivadas.
        /// 
        /// EJEMPLO:
        /// <code>
        /// protected override async Task InitializeDomainSpecificPermissionsAsync()
        /// {
        ///     CanApproveProducts = await IsGrantedAsync("Products.Approve");
        ///     CanSetPrices = await IsGrantedAsync("Products.ManagePricing");
        /// }
        /// </code>
        /// </summary>
        protected virtual async Task InitializeDomainSpecificPermissionsAsync()
        {
            // Las clases derivadas pueden sobrescribir para permisos específicos
            await Task.CompletedTask;
        }

        #endregion

        #region Verificación y Cache de Permisos

        /// <summary>
        /// Refresca todos los permisos base desde ABP.
        /// </summary>
        private async Task RefreshPermissionsAsync()
        {
            // Usar verificación en paralelo para mejor rendimiento
            var permissionTasks = new Dictionary<string, Task<bool>>
            {
                [ViewPermissionName] = IsGrantedAsync(ViewPermissionName),
                [CreatePermissionName] = IsGrantedAsync(CreatePermissionName),
                [UpdatePermissionName] = IsGrantedAsync(UpdatePermissionName),
                [DeletePermissionName] = IsGrantedAsync(DeletePermissionName),
                [ExportPermissionName] = IsGrantedAsync(ExportPermissionName)
            };

            await Task.WhenAll(permissionTasks.Values);

            // Actualizar propiedades con los resultados
            CanViewEntities = permissionTasks[ViewPermissionName].Result;
            CanCreateEntities = permissionTasks[CreatePermissionName].Result;
            CanUpdateEntities = permissionTasks[UpdatePermissionName].Result;
            CanDeleteEntities = permissionTasks[DeletePermissionName].Result;
            CanExportEntities = permissionTasks[ExportPermissionName].Result;

            // Actualizar cache
            foreach (var task in permissionTasks)
            {
                PermissionCache[task.Key] = task.Value.Result;
            }

            LastPermissionRefresh = DateTime.Now;
        }

        /// <summary>
        /// Verifica un permiso específico con cache para mejor rendimiento.
        /// </summary>
        /// <param name="permissionName">Nombre del permiso a verificar</param>
        /// <param name="forceRefresh">Forzar verificación sin usar cache</param>
        /// <returns>true si el permiso está concedido</returns>
        protected virtual async Task<bool> IsGrantedWithCacheAsync(string permissionName, bool forceRefresh = false)
        {
            // Verificar si el cache es válido y no se fuerza refresh
            if (!forceRefresh && IsCacheValid() && PermissionCache.ContainsKey(permissionName))
            {
                Logger.Debug($"Permiso {permissionName} obtenido de cache: {PermissionCache[permissionName]}");
                return PermissionCache[permissionName];
            }

            // Verificar permiso desde ABP
            var isGranted = await IsGrantedAsync(permissionName);

            // Actualizar cache
            PermissionCache[permissionName] = isGranted;

            Logger.Debug($"Permiso {permissionName} verificado desde ABP: {isGranted}");

            return isGranted;
        }

        /// <summary>
        /// Verifica si el cache de permisos sigue siendo válido.
        /// </summary>
        private bool IsCacheValid()
        {
            return DateTime.Now.Subtract(LastPermissionRefresh).TotalMinutes < PermissionCacheLifetimeMinutes;
        }

        /// <summary>
        /// Invalida el cache de permisos forzando una nueva verificación.
        /// Útil cuando se sabe que los permisos han cambiado.
        /// </summary>
        public virtual async Task InvalidatePermissionCacheAsync()
        {
            Logger.Debug("Invalidando cache de permisos");

            PermissionCache.Clear();
            LastPermissionRefresh = DateTime.MinValue;

            await RefreshPermissionsAsync();
            NotifyPermissionPropertiesChanged();
            RefreshCommandStates();
        }

        #endregion

        #region OVERRIDE de Métodos CanExecute con Permisos

        /// <summary>
        /// OVERRIDE: Incluir verificación de permisos en CanLoadAsync.
        /// </summary>
        public override bool CanLoadAsync()
        {
            return base.CanLoadAsync() && CanViewEntities;
        }

        /// <summary>
        /// OVERRIDE: Incluir verificación de permisos en CanCreate.
        /// </summary>
        public override bool CanCreate(TCreateInput input)
        {
            return base.CanCreate(input) && CanCreateEntities;
        }

        /// <summary>
        /// OVERRIDE: Incluir verificación de permisos en CanUpdate.
        /// Las clases derivadas pueden agregar lógica adicional.
        /// </summary>
        public override bool CanUpdate(TUpdateInput input)
        {
            if (!base.CanUpdate(input) || !CanUpdateEntities)
                return false;

            // Verificaciones específicas de entidad si es necesario
            return CanUpdateSpecificEntity(input);
        }

        /// <summary>
        /// OVERRIDE: Incluir verificación de permisos en CanDelete.
        /// </summary>
        public override bool CanDelete(TDeleteInput input)
        {
            if (!base.CanDelete(input) || !CanDeleteEntities)
                return false;

            // Verificaciones específicas de entidad
            return CanDeleteSpecificEntity(input);
        }

        /// <summary>
        /// Verifica si se puede actualizar una entidad específica.
        /// VIRTUAL para lógica específica en clases derivadas.
        /// 
        /// EJEMPLO: Verificar que el usuario solo puede editar sus propios registros.
        /// </summary>
        protected virtual bool CanUpdateSpecificEntity(TUpdateInput input)
        {
            // Implementación base permite si tiene permiso general
            // Las clases derivadas pueden agregar lógica específica
            return true;
        }

        /// <summary>
        /// Verifica si se puede eliminar una entidad específica.
        /// VIRTUAL para lógica específica.
        /// </summary>
        protected virtual bool CanDeleteSpecificEntity(TDeleteInput input)
        {
            return true;
        }

        #endregion

        #region Permisos Condicionales y Dinámicos

        /// <summary>
        /// Verifica permisos condicionales basados en el estado de la entidad.
        /// VIRTUAL para implementación específica.
        /// 
        /// EJEMPLO: Solo permitir editar productos no aprobados.
        /// </summary>
        /// <param name="entity">Entidad a evaluar</param>
        /// <param name="action">Acción a realizar</param>
        /// <returns>true si la acción está permitida</returns>
        protected virtual async Task<bool> IsActionAllowedForEntityAsync(TEntityDto entity, string action)
        {
            if (entity == null)
                return false;

            // Verificar permiso base primero
            var basePermission = await IsGrantedWithCacheAsync($"{BasePermissionName}.{action}");
            if (!basePermission)
                return false;

            // Las clases derivadas pueden sobrescribir para lógica específica
            return await IsActionAllowedForEntitySpecificAsync(entity, action);
        }

        /// <summary>
        /// Verifica permisos específicos para una entidad particular.
        /// VIRTUAL para implementación en clases derivadas.
        /// </summary>
        protected virtual async Task<bool> IsActionAllowedForEntitySpecificAsync(TEntityDto entity, string action)
        {
            // Implementación base - siempre permitido si tiene permiso general
            await Task.CompletedTask;
            return true;
        }

        /// <summary>
        /// Verifica permisos dinámicos que pueden cambiar según el contexto.
        /// EJEMPLO: Permisos que dependen de la hora, ubicación, etc.
        /// </summary>
        /// <param name="permissionName">Nombre del permiso</param>
        /// <param name="context">Contexto adicional</param>
        /// <returns>true si está concedido</returns>
        protected virtual async Task<bool> IsDynamicPermissionGrantedAsync(string permissionName, object context = null)
        {
            // Verificar permiso base
            var baseGranted = await IsGrantedWithCacheAsync(permissionName);
            if (!baseGranted)
                return false;

            // Verificaciones dinámicas adicionales
            return await EvaluateDynamicPermissionRulesAsync(permissionName, context);
        }

        /// <summary>
        /// Evalúa reglas dinámicas para permisos.
        /// VIRTUAL para implementación específica.
        /// </summary>
        protected virtual async Task<bool> EvaluateDynamicPermissionRulesAsync(string permissionName, object context)
        {
            // Ejemplo de reglas dinámicas:

            // 1. Restricciones de horario
            if (permissionName.Contains("Delete") && DateTime.Now.Hour > 18)
            {
                Logger.Debug($"Permiso {permissionName} denegado por restricción horaria");
                return false;
            }

            // 2. Límites de sesión
            if (KontecgSession.ImpersonatorUserId.HasValue && permissionName.Contains("Delete"))
            {
                Logger.Debug($"Permiso {permissionName} denegado durante impersonación");
                return false;
            }

            // Las clases derivadas pueden agregar más reglas
            return await EvaluateCustomDynamicRulesAsync(permissionName, context);
        }

        /// <summary>
        /// Evalúa reglas dinámicas personalizadas.
        /// VIRTUAL para implementación específica del dominio.
        /// </summary>
        protected virtual async Task<bool> EvaluateCustomDynamicRulesAsync(string permissionName, object context)
        {
            await Task.CompletedTask;
            return true; // Por defecto permitir
        }

        #endregion

        #region Comandos con Verificación de Permisos

        /// <summary>
        /// OVERRIDE: Incluir verificación de permisos en CreateAsync.
        /// </summary>
        [Command]
        public override async Task CreateAsync(TCreateInput input)
        {
            // Verificación adicional de permisos en tiempo de ejecución
            if (!await IsGrantedWithCacheAsync(CreatePermissionName))
            {
                await ShowUnauthorizedMessageAsync("CreateEntity");
                return;
            }

            // Verificaciones específicas del contexto
            if (!await IsCreateAllowedInCurrentContextAsync(input))
            {
                return;
            }

            // Continuar con el proceso de creación
            await base.CreateAsync(input);
        }

        /// <summary>
        /// OVERRIDE: Incluir verificación de permisos en UpdateAsync.
        /// </summary>
        [Command]
        public override async Task UpdateAsync(TUpdateInput input)
        {
            if (!await IsGrantedWithCacheAsync(UpdatePermissionName))
            {
                await ShowUnauthorizedMessageAsync("UpdateEntity");
                return;
            }

            // Verificar permisos específicos para esta entidad
            var entity = Entities.FirstOrDefault(e => e.Id.Equals(input.Id));
            if (entity != null && !await IsActionAllowedForEntityAsync(entity, "Update"))
            {
                await ShowUnauthorizedMessageAsync("UpdateThisEntity");
                return;
            }

            await base.UpdateAsync(input);
        }

        /// <summary>
        /// OVERRIDE: Incluir verificación de permisos en DeleteAsync.
        /// </summary>
        [Command]
        public override async Task DeleteAsync(TDeleteInput input)
        {
            if (!await IsGrantedWithCacheAsync(DeletePermissionName))
            {
                await ShowUnauthorizedMessageAsync("DeleteEntity");
                return;
            }

            // Verificar permisos específicos
            var entity = Entities.FirstOrDefault(e => e.Id.Equals(input.Id));
            if (entity != null && !await IsActionAllowedForEntityAsync(entity, "Delete"))
            {
                await ShowUnauthorizedMessageAsync("DeleteThisEntity");
                return;
            }

            await base.DeleteAsync(input);
        }

        #endregion

        #region Comandos Adicionales con Permisos

        /// <summary>
        /// Comando para exportar con verificación de permisos.
        /// </summary>
        [Command]
        public virtual async Task ExportWithPermissionsAsync()
        {
            if (!CanExportWithPermissions())
                return;

            // Verificación adicional en tiempo de ejecución
            if (!await IsGrantedWithCacheAsync(ExportPermissionName))
            {
                await ShowUnauthorizedMessageAsync("ExportData");
                return;
            }

            try
            {
                await PerformExportAsync();
            }
            catch (KontecgAuthorizationException ex)
            {
                Logger.Warn("Usuario sin permisos intentó exportar", ex);
                await ShowUnauthorizedMessageAsync("ExportData");
            }
        }

        /// <summary>
        /// Comando para operaciones administrativas especiales.
        /// </summary>
        [Command]
        public virtual async Task AdminActionAsync(string action)
        {
            var adminPermission = $"{BasePermissionName}.Admin.{action}";

            if (!await IsGrantedWithCacheAsync(adminPermission))
            {
                await ShowUnauthorizedMessageAsync("AdminAction");
                return;
            }

            await PerformAdminActionAsync(action);
        }

        /// <summary>
        /// CanExecute para exportación con permisos.
        /// </summary>
        public virtual bool CanExportWithPermissions()
        {
            return CanExecuteOperations &&
                   CanExportEntities &&
                   FilteredEntities?.Count > 0;
        }

        /// <summary>
        /// CanExecute para acciones administrativas.
        /// </summary>
        public virtual bool CanExecuteAdminAction(string action)
        {
            // Verificación rápida con cache (síncrona para CanExecute)
            var adminPermission = $"{BasePermissionName}.Admin.{action}";
            return CanExecuteOperations &&
                   PermissionCache.GetValueOrDefault(adminPermission, false);
        }

        #endregion

        #region Validaciones con Permisos

        /// <summary>
        /// OVERRIDE: Incluir verificación de permisos en validaciones.
        /// </summary>
        protected override async Task<bool> ValidateForCreateAsync(TCreateInput input)
        {
            // Validación base de UI
            if (!await base.ValidateForCreateAsync(input))
                return false;

            // Verificación de permisos específicos
            if (!await IsCreateAllowedInCurrentContextAsync(input))
                return false;

            return true;
        }

        /// <summary>
        /// Verifica si la creación está permitida en el contexto actual.
        /// VIRTUAL para implementación específica.
        /// </summary>
        protected virtual async Task<bool> IsCreateAllowedInCurrentContextAsync(TCreateInput input)
        {
            // Ejemplo: Verificar límites de creación
            if (!await IsGrantedAsync($"{CreatePermissionName}.Unlimited"))
            {
                var todayCount = await GetTodayCreationCountAsync();
                if (todayCount >= GetMaxDailyCreations())
                {
                    await ShowWarningAsync(L("DailyCreationLimitReached"));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Obtiene el límite diario de creaciones para el usuario actual.
        /// VIRTUAL para personalización.
        /// </summary>
        protected virtual int GetMaxDailyCreations()
        {
            // Diferentes límites según rol
            if (IsGranted($"{BasePermissionName}.PowerUser"))
                return 100;

            return 10; // Límite base
        }

        /// <summary>
        /// Obtiene el conteo de creaciones del día actual.
        /// VIRTUAL para implementación específica.
        /// </summary>
        protected virtual async Task<int> GetTodayCreationCountAsync()
        {
            // Implementar según tu lógica de negocio
            await Task.CompletedTask;
            return 0;
        }

        #endregion

        #region Métodos de Utilidad para Permisos

        /// <summary>
        /// Establece todos los permisos a un valor específico.
        /// Útil para casos de error o inicialización.
        /// </summary>
        private void SetAllPermissions(bool value)
        {
            CanViewEntities = value;
            CanCreateEntities = value;
            CanUpdateEntities = value;
            CanDeleteEntities = value;
            CanExportEntities = value;
        }

        /// <summary>
        /// Notifica cambios en las propiedades de permisos a DevExpress.
        /// </summary>
        private void NotifyPermissionPropertiesChanged()
        {
            this.RaisePropertyChanged(x => x.CanViewEntities);
            this.RaisePropertyChanged(x => x.CanCreateEntities);
            this.RaisePropertyChanged(x => x.CanUpdateEntities);
            this.RaisePropertyChanged(x => x.CanDeleteEntities);
            this.RaisePropertyChanged(x => x.CanExportEntities);
            this.RaisePropertyChanged(x => x.GrantedPermissions);
        }

        /// <summary>
        /// Muestra mensaje de no autorizado contextual.
        /// </summary>
        protected virtual async Task ShowUnauthorizedMessageAsync(string action)
        {
            var message = L($"NotAuthorizedFor{action}");
            await ShowWarningAsync(message);

            Logger.Warn($"Usuario {KontecgSession.UserId} sin permisos para {action} en {typeof(TEntityDto).Name}");
        }

        /// <summary>
        /// Realiza la exportación con permisos verificados.
        /// VIRTUAL para implementación específica.
        /// </summary>
        protected virtual async Task PerformExportAsync()
        {
            // Implementar lógica de exportación específica
            await Task.CompletedTask;
        }

        /// <summary>
        /// Realiza acciones administrativas especiales.
        /// VIRTUAL para implementación específica.
        /// </summary>
        protected virtual async Task PerformAdminActionAsync(string action)
        {
            // Implementar según acciones disponibles
            await Task.CompletedTask;
        }

        #endregion

        #region Eventos de Permisos

        /// <summary>
        /// Evento que se dispara cuando cambian los permisos.
        /// </summary>
        public event EventHandler<PermissionsChangedEventArgs> PermissionsChanged;

        /// <summary>
        /// Dispara el evento de cambio de permisos.
        /// </summary>
        protected virtual void OnPermissionsChanged(string[] changedPermissions)
        {
            PermissionsChanged?.Invoke(this, new PermissionsChangedEventArgs(changedPermissions));
        }

        #endregion
    }

    #region Clases de Soporte para Permisos

    /// <summary>
    /// Argumentos para el evento de cambio de permisos.
    /// </summary>
    public class PermissionsChangedEventArgs : EventArgs
    {
        public string[] ChangedPermissions { get; }
        public DateTime ChangedAt { get; }

        public PermissionsChangedEventArgs(string[] changedPermissions)
        {
            ChangedPermissions = changedPermissions;
            ChangedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Resultado de verificación de permisos con contexto.
    /// </summary>
    public class PermissionCheckResult
    {
        public bool IsGranted { get; set; }
        public string Reason { get; set; }
        public string[] RequiredPermissions { get; set; }
        public DateTime CheckedAt { get; set; }

        public static PermissionCheckResult Granted() => new() { IsGranted = true, CheckedAt = DateTime.Now };
        public static PermissionCheckResult Denied(string reason) => new() { IsGranted = false, Reason = reason, CheckedAt = DateTime.Now };
    }

    #endregion
}
#endif