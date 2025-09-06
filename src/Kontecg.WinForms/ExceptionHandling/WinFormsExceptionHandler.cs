using System;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using Kontecg.Authorization;
using Kontecg.Events.Bus.Exceptions;
using Kontecg.Events.Bus.Handlers;
using Kontecg.Extensions;
using Kontecg.Logging;
using Kontecg.Runtime;
using Kontecg.Runtime.Session;
using Kontecg.Runtime.Validation;
using Kontecg.Timing;
using Kontecg.UI;

namespace Kontecg.ExceptionHandling
{
    public class WinFormsExceptionHandler : KontecgCoreDomainServiceBase, IEventHandler<KontecgHandledExceptionData>
    {
        private readonly IExceptionHandlingConfiguration _exceptionHandlingConfiguration;
        private readonly IExceptionMailer _exceptionMailer;
        private readonly SnapshotManager _snapshotManager;
        private readonly IWinFormsRuntime _winFormsRuntime;

        public WinFormsExceptionHandler(
            IExceptionHandlingConfiguration exceptionHandlingConfiguration,
            IExceptionMailer exceptionMailer, 
            SnapshotManager snapshotManager, 
            IWinFormsRuntime winFormsRuntime)
        {
            _exceptionHandlingConfiguration = exceptionHandlingConfiguration;
            _exceptionMailer = exceptionMailer;
            _snapshotManager = snapshotManager;
            _winFormsRuntime = winFormsRuntime;

            KontecgSession = NullKontecgSession.Instance;
        }

        IKontecgSession KontecgSession { get; set; }

        private Form Owner => _winFormsRuntime.MainForm?.Target as Form;

        public void HandleEvent(KontecgHandledExceptionData eventData)
        {
            var exception = eventData.Exception;

            switch (exception)
            {
                case IHasLogSeverity exceptionWithLogSeverity:
                {
                    Logger.Log(exceptionWithLogSeverity.Severity, exception.Message, exception);

                    switch (exceptionWithLogSeverity)
                    {
                        case KontecgAuthorizationException kontecgAuthorizationException:
                            XtraMessageBox.Show(UserLookAndFeel.Default, Owner, kontecgAuthorizationException.Message,
                                GetMessageCaption(kontecgAuthorizationException.Severity),
                                MessageBoxButtons.OK, GetMessageIcon(kontecgAuthorizationException.Severity));
                            break;
                        case KontecgValidationException kontecgValidationException:
                            StringBuilder sb = new StringBuilder(kontecgValidationException.Message);
                            sb.AppendLine();
                            foreach (var validationResult in kontecgValidationException.ValidationErrors)
                                sb.AppendLine(validationResult.ErrorMessage);
                            XtraMessageBox.Show(UserLookAndFeel.Default, Owner, sb.ToString(),
                                GetMessageCaption(kontecgValidationException.Severity),
                                MessageBoxButtons.OK, GetMessageIcon(kontecgValidationException.Severity));
                                break;
                        case KontecgInvalidPeriodException kontecgInvalidPeriodException:
                            XtraMessageBox.Show(UserLookAndFeel.Default, Owner, kontecgInvalidPeriodException.Message,
                                GetMessageCaption(kontecgInvalidPeriodException.Severity),
                                MessageBoxButtons.OK, GetMessageIcon(kontecgInvalidPeriodException.Severity));
                                break;
                        case UserFriendlyException userFriendlyException:
                            XtraMessageBox.Show(UserLookAndFeel.Default, Owner, userFriendlyException.Message,
                                GetMessageCaption(userFriendlyException.Severity),
                                MessageBoxButtons.OK, GetMessageIcon(userFriendlyException.Severity));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(exceptionWithLogSeverity));
                    }
                    break;
                }
                default:
                    if (_exceptionHandlingConfiguration.SendDetailedExceptionsToSupport)
                    {
                        var snapshot = _snapshotManager.GrabSnapshot();
                        _exceptionMailer.Send(KontecgSession, exception, L("ExceptionMessage"), snapshot);
                    }

                    if (_exceptionHandlingConfiguration.PropagatedHandledExceptions)
                        exception.ReThrow();
                    break;
            }
        }

        private string GetMessageCaption(LogSeverity logSeverity)
        {
            switch (logSeverity)
            {
                case LogSeverity.Debug:
                case LogSeverity.Info:
                    return L("Information");
                case LogSeverity.Warn:
                    return L("Warning");
                case LogSeverity.Error:
                case LogSeverity.Fatal:
                    return L("Error");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private MessageBoxIcon GetMessageIcon(LogSeverity logSeverity)
        {
            switch (logSeverity)
            {
                case LogSeverity.Debug:
                case LogSeverity.Info:
                    return MessageBoxIcon.Information;
                case LogSeverity.Warn:
                    return MessageBoxIcon.Warning;
                case LogSeverity.Error:
                case LogSeverity.Fatal:
                    return MessageBoxIcon.Error;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
