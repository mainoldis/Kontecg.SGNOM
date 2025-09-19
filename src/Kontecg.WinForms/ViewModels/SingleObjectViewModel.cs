using System;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Kontecg.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using DevExpress.Mvvm.POCO;
using System.Threading.Tasks;
using Kontecg.Authorization;
using Kontecg.Logging;
using Kontecg.Runtime.Validation;
using Kontecg.UI;

namespace Kontecg.ViewModels
{
    [POCOViewModel]
    public abstract class SingleObjectViewModel<TEntityDto, TPrimaryKey> : KontecgViewModelBase,
        ISingleObjectViewModel<TEntityDto, TPrimaryKey>, 
        IDocumentContent,
        ISupportParentViewModel, 
        ISupportParameter
        where TEntityDto : class, IEntityDto<TPrimaryKey>
    {
        private bool _isInitialized;
        private Action<TEntityDto> _entityInitializer;
        private ViewModelEntityState _entityState;
        private bool _dontUpdateEntityState;

        protected SingleObjectViewModel(ViewModelConfiguration configuration)
        {
            Configuration = configuration ?? new ViewModelConfiguration();
            _isInitialized = false;
            LocalizationSourceName = Configuration.LocalizationSourceName;
        }

        protected ViewModelConfiguration Configuration { get; private set; }

        /// <inheritdoc />
        public virtual TEntityDto Entity { get; set; }

        /// <inheritdoc />
        public virtual TPrimaryKey Id => GetPrimaryKey();

        public virtual bool IsEnabled { get; protected set; }

        protected override void OnInitializeInRuntime()
        {
            if (_isInitialized) return;

            try
            {
                Logger.Debug($"Initializing {this.GetUnproxiedType().Name}");
                InitializeServices();
                _isInitialized = true;
                Logger.Debug($"{this.GetUnproxiedType().Name} setup successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"An error has occurred initializing {this.GetUnproxiedType().Name}", ex);
                throw;
            }
        }

        protected virtual void InitializeServices()
        {
        }

        protected virtual TPrimaryKey GetPrimaryKey()
        {
            return Entity != null ? Entity.Id : default;
        }

        protected virtual void OnEntityChanged()
        {
            IsEnabled = Entity != null;
        }

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

        #region Interfaz ISupportParentViewModel

        object ISupportParentViewModel.ParentViewModel { get; set; }

        protected void OnParentViewModelChanged()
        {
            AllowSaveReset = Configuration.AllowSaveReset;
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

        public virtual bool CanSave() => AllowSaveReset;

        /// <summary>
        /// Comando de reset genérico.
        /// Las clases derivadas deben sobrescribir para implementar lógica específica.
        /// </summary>
        [Display(AutoGenerateField = false)]
        [Command]
        public virtual void Reset()
        {
            Logger.Debug("Reset command executed - base implementation");
        }

        public virtual bool CanReset() => AllowSaveReset;

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
            return L("EntityTitle", entityName);
        }

        void IDocumentContent.OnClose(CancelEventArgs e) => OnClose(e);

        void IDocumentContent.OnDestroy() => OnDestroy();

        protected virtual void OnClose(CancelEventArgs e)
        {
            if (_entityState != ViewModelEntityState.ExistingUnchanged)
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

        #region Cleanup y Disposable

        protected virtual void OnDestroy()
        {
            try
            {
                Logger.Debug($"Disposing {this.GetUnproxiedType().Name}");
                //AsyncHelper.RunSync(CancelCurrentOperationAsync);
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

        protected IDispatcherService DispatcherService =>
            this.GetService<IDispatcherService>();

        #endregion

        #region ISupportParameter

        object ISupportParameter.Parameter
        {
            get => Entity;
            set
            {
                Entity = (TEntityDto) value;
                OnParameterChanged(Entity);
            }
        }

        protected virtual void OnParameterChanged(TEntityDto parameter)
        {
        }


        #endregion
    }
}