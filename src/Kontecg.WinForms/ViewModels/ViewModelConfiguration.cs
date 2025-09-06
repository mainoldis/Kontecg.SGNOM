namespace Kontecg.ViewModels
{
    /// <summary>
    /// Información de configuración para el ViewModel.
    /// Permite configurar comportamientos sin sobrescribir métodos.
    /// </summary>
    public class ViewModelConfiguration
    {
        public string LocalizationSourceName { get; set; } = KontecgWinFormsConsts.LocalizationSourceName;

        /// <summary>
        /// Habilita/deshabilita carga automática al inicializar.
        /// </summary>
        public bool AutoLoadOnInitialize { get; set; } = true;

        /// <summary>
        /// Habilita/deshabilita actualización automática de filtros.
        /// </summary>
        public bool AutoApplyFilters { get; set; } = true;

        /// <summary>
        /// Tiempo de espera para operaciones en millisegundos.
        /// </summary>
        public int OperationTimeoutMs { get; set; } = 30000;

        /// <summary>
        /// Habilita/deshabilita confirmación antes de eliminar.
        /// </summary>
        public bool ConfirmDelete { get; set; } = true;

        /// <summary>
        /// Habilita/deshabilita mensajes de éxito.
        /// </summary>
        public bool ShowSuccessMessages { get; set; } = true;

        /// <summary>
        /// Número máximo de elementos para aplicar filtros locales.
        /// Si hay más elementos, se recomienda filtrar en el servidor.
        /// </summary>
        public int MaxEntitiesForLocalFiltering { get; set; } = KontecgCoreConsts.DefaultPageSize / 2;
    }
}