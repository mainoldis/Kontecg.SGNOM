using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Castle.Core.Logging;
using DevExpress.LookAndFeel;
using DevExpress.Mvvm;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using Kontecg.Application.Features;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Events.Bus;
using Kontecg.Events.Bus.Exceptions;
using Kontecg.ExceptionHandling;
using Kontecg.Extensions;
using Kontecg.Features;
using Kontecg.Localization;
using Kontecg.Localization.Sources;
using Kontecg.Logging;
using Kontecg.Runtime.Events;
using Kontecg.Threading;
using Kontecg.Timing;
using Kontecg.Updates;
using Kontecg.ViewModels.Account;
using Kontecg.Views.Account;
using Microsoft.Extensions.FileProviders;

namespace Kontecg.Runtime
{
    public class WinFormsRuntime : IShouldInitialize, ISingletonDependency, IWinFormsRuntime
    {
        private readonly IKontecgWinFormsConfiguration _winFormsConfiguration;
        private readonly IKontecgStartupConfiguration _startupConfiguration;
        private readonly ILocalizationManager _localizationManager;
        private readonly IExceptionHandlingConfiguration _exceptionHandlingConfiguration;
        private readonly IIocResolver _iocResolver;
        private readonly IUpdateManager _updateManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly AppTimes _appTimes;

        private bool _runningContext;
        private WeakReference _mainForm;
        private Icon _appIcon;
        private SvgImage _appSvgImage;

        public WinFormsRuntime(
            IKontecgWinFormsConfiguration winFormsConfiguration, 
            IExceptionHandlingConfiguration exceptionHandlingConfiguration,
            IKontecgStartupConfiguration startupConfiguration,
            IIocResolver iocResolver,
            IUpdateManager updateManager,
            ILocalizationManager localizationManager,
            IFeatureChecker featureChecker,
            AppTimes appTimes)
        {
            _winFormsConfiguration = winFormsConfiguration;
            _exceptionHandlingConfiguration = exceptionHandlingConfiguration;
            _startupConfiguration = startupConfiguration;
            _iocResolver = iocResolver;
            _updateManager = updateManager;
            _appTimes = appTimes;
            _featureChecker = featureChecker;
            _localizationManager = localizationManager;

            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
        }

        public WeakReference MainForm => _mainForm;

        public ILogger Logger { get; set; }

        public IEventBus EventBus { get; set; }

        public ISettingManager SettingManager { get; set; }

        public bool RunningContext => _runningContext;

        public Icon AppIcon => _appIcon;

        public SvgImage AppImage => _appSvgImage;

        public void Initialize()
        {
            SetupAppIcon();
            ChangeCulture();
            ChangeTheme(false); // Setup default theme ignoring features and permissions
            PrepareForWinForms();
        }

        [STAThread]
        public void Run()
        {
            try
            {
                using (var arguments = _iocResolver.ResolveAsDisposable<CommandLineParserService>())
                {
                    if (arguments.Object.ArgsCount > 0)
                    {
                        _winFormsConfiguration.UseSingleProcess = arguments.Object.GetArgValueAsBoolean("single", true);
                        _winFormsConfiguration.UseDirectX = arguments.Object.GetArgValueAsBoolean("directx", false);

                        if (_winFormsConfiguration.UseDirectX)
                        {
                            Logger.Debug("Force DirectX painting support");
                            WindowsFormsSettings.ForceDirectXPaint();
                        }
                    }
                }

                using var instance =
                    CheckIfRunningAnotherInstance(KontecgWinFormsConsts.ProcessName, out var hasToExit);
                if (hasToExit) return;

                AssertThisIsTheOnlyRunningContext();

                EventBus.Trigger(this, new StartupProcessChanged(L("Starting")));
                var needToRestart = CheckUpdates();

                switch (CheckForAuthorization())
                {
                    case DialogResult.None:
                        ShowMainForm(); //No login required, use previous settings
                        break;
                    case DialogResult.OK:
                        ChangeCulture();
                        ChangeTheme();
                        ShowMainForm();
                        break;
                    default:
                        EventBus.Trigger(this,
                            new StartupProcessCompleted(Clock.Now - _appTimes.StartupTime)); //Just finish
                        break;
                }
            }
            catch (KontecgException ex)
            {
                if(_exceptionHandlingConfiguration.TriggerDomainEvents)
                    EventBus.Trigger(this, new KontecgHandledExceptionData(ex));
                else
                    switch (ex)
                    {
                        case IHasLogSeverity exceptionWithLogSeverity:
                        {
                            Logger.Log(exceptionWithLogSeverity.Severity, ex.Message, ex);
                            break;
                        }
                        default:
                            Logger.Error(ex.Message);
                            break;
                    }
            }
            catch (Exception ex)
            {
                if (_exceptionHandlingConfiguration.TriggerDomainEvents)
                {
                    _exceptionHandlingConfiguration.PropagatedHandledExceptions = true;
                    EventBus.Trigger(this, new KontecgHandledExceptionData(ex));
                }
                else
                    Logger.Error(ex.Message);
            }
        }

        private void ShowMainForm()
        {
            Logger.Debug("Entering 'ApplicationContext' message loop");

            var form = _iocResolver.Resolve(_winFormsConfiguration.MainType).As<Form>();
            if (form != null)
            {
                form.Icon = _appIcon;
                _mainForm = new WeakReference(form);
                System.Windows.Forms.Application.Run(new ApplicationContext(form));
                //Releases resources for GC
                _iocResolver.Release(form);
                _mainForm = null;
                _appIcon = null;
            }
            else
                Logger.Warn("Can't find a proper form to init de app!!!");

            Logger.Debug("Exiting 'ApplicationContext' message loop");
        }

        private DialogResult CheckForAuthorization()
        {
            var loginFirst = CheckIfAuthenticationRequired();
            if (!loginFirst)
                return DialogResult.None;

            Logger.Debug("Authentication is required at startup");
            EventBus.Trigger(this, new StartupProcessChanged(L("AuthorizationRequired")));
            return ShowLoginForm();
        }

        private DialogResult ShowLoginForm()
        {
            if (_iocResolver.Resolve<LoginDialogDocumentManagerService>() is IDocumentManagerService dms)
            {
                var document = dms.Documents.FirstOrDefault(d => d.Content is LoginViewModel) ??
                               dms.CreateDocument("LoginView", null, null, null);
                document.Show();
                return ((LoginViewModel) document.Content).DialogResult;
            }

            EventBus.Trigger(this, new StartupProcessChanged(L("AuthorizationCancelled")));
            return DialogResult.Cancel;
        }

        private IDisposable CheckIfRunningAnotherInstance(string processName, out bool exit)
        {
            Logger.Debug(
                $"Allowing {(_winFormsConfiguration.UseSingleProcess ? "only one" : "multiple")} running process of '{processName}'");
            var mutex = new Mutex(true, processName);
            if (mutex.WaitOne(0, false) || !_winFormsConfiguration.UseSingleProcess)
            {
                exit = false;
            }
            else
            {
                var current = Process.GetCurrentProcess();
                foreach (var process in Process.GetProcessesByName(current.ProcessName))
                    if (process.Id != current.Id && process.MainWindowHandle != IntPtr.Zero)
                    {
#if WINDOWS
                        Win32Api.SetForegroundWindow(process.MainWindowHandle);
                        Win32Api.RestoreWindowAsync(process.MainWindowHandle);
#endif
                        break;
                    }
                exit = true;
            }

            return mutex;
        }

        private bool CheckIfAuthenticationRequired()
        {
            return SettingManager.GetSettingValueForApplication<bool>(WinFormsSettings.AuthManagement.LoginFirstRequired);
        }

        private void AssertThisIsTheOnlyRunningContext()
        {
            // there can be only one context running per application
            if (_runningContext)
            {
                Logger.WarnFormat(
                    "A KontecgWinFormsContext is already running for the AppDomain '{0}'. Only one context can be run per application.",
                    AppDomain.CurrentDomain.FriendlyName);
                return;
            }

            // set this so that future calls to the Run method will exit
            _runningContext = true;
        }

        private void ChangeTheme(bool mustBeEnabled = true)
        {
            if (mustBeEnabled && !_featureChecker.IsEnabled(WinFormsFeatureNames.ChangeThemeFeature)) return;

            if (mustBeEnabled && !SettingManager.GetSettingValue<bool>(WinFormsSettings.Theme.AllowChangeTheme)) return;
            
            var skinName = SettingManager.GetSettingValue(WinFormsSettings.Theme.SkinName);
            var paletteName = SettingManager.GetSettingValue(WinFormsSettings.Theme.PaletteName);
            var forceCompactUiMode = SettingManager.GetSettingValue<bool>(WinFormsSettings.Theme.CompactUi);

            if (forceCompactUiMode && skinName == "WXI")
            {
                skinName = $"{skinName}(Compact)";
                Logger.Debug($"Setting skin to '{skinName}' with palette '{paletteName}'");
                switch (paletteName)
                {
                    case "Darkness":
                        WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(SkinSvgPalette.WXICompact.Darkness);
                        break;
                    case "Clearness":
                        WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(SkinSvgPalette.WXICompact.Clearness);
                        break;
                    case "Sharpness":
                        WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(SkinSvgPalette.WXICompact.Sharpness);
                        break;
                    case "Calmness":
                        WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(SkinSvgPalette.WXICompact.Calmness);
                        break;
                    case "Office Dark Gray":
                        WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(SkinSvgPalette.WXICompact.OfficeDarkGray);
                        break;
                    case "Office Black":
                        WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(SkinSvgPalette.WXICompact.OfficeBlack);
                        break;
                    case "Office Colorful":
                        WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(SkinSvgPalette.WXICompact.OfficeColorful);
                        break;
                    case "Office White":
                        WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(SkinSvgPalette.WXICompact.OfficeWhite);
                        break;
                    default:
                        WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(SkinSvgPalette.WXICompact.Default);
                    break;
                }
                return;
            }

            Logger.Debug($"Setting skin to '{skinName}' with palette '{paletteName}'");
            WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(skinName, paletteName);
        }

        private void ChangeCulture()
        {
            string defaultLanguage = "en";
            try
            {
                defaultLanguage = SettingManager.GetSettingValue(LocalizationSettingNames.DefaultLanguage);
            }
            catch (Exception ex)
            {
                Logger.Warn("Can't get default language.", ex);
            }
            finally
            {
                Logger.Debug($"Setting cultureInfo to '{defaultLanguage}'");
                var cultureInfo = CultureHelper.GetCultureInfoByChecking(defaultLanguage ?? "en");

                // The following line provides localization for the application's user interface.
                Thread.CurrentThread.CurrentUICulture = cultureInfo;

                // The following line provides localization for data formats.
                Thread.CurrentThread.CurrentCulture = cultureInfo;

                // Set this culture as the default culture for all threads in this application.
                // Note: The following properties are supported in the .NET Framework 4.5+
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            }
        }

        private void SetupAppIcon()
        {
            using var fileProvider = _iocResolver.ResolveAsDisposable<IFileProvider>();
            var resourceInfo = fileProvider.Object.GetFileInfo(KontecgWinFormsConsts.ResourcesNames.AppIcon);
            _appIcon = resourceInfo.Length > 0 ? new Icon(resourceInfo.CreateReadStream()) : null;

            resourceInfo = fileProvider.Object.GetFileInfo(KontecgWinFormsConsts.ResourcesNames.BrandIcon);
            _appSvgImage = resourceInfo.Length > 0 ? new SvgImage(resourceInfo.CreateReadStream()) : null;
        }

        private void PrepareForWinForms()
        {
            WindowsFormsSynchronizationContext.AutoInstall = true;

            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());

            ScreenManager.Initialize();
        }

        private bool CheckUpdates()
        {
            bool updatesCheckForUpdatesAtStartup = _startupConfiguration.Updates.IsUpdateCheckEnabled &&
                                                   _startupConfiguration.Updates.CheckForUpdatesAtStartup;
            if (!updatesCheckForUpdatesAtStartup)
                return false;

            try
            {
                Logger.Debug("Check for new updates at startup");
                var updateNeeded = AsyncHelper.RunSync(() => _updateManager.CheckForUpdateAsync(percent =>
                    EventBus.Trigger(this, new StartupProcessChanged(L("SearchForUpdates", percent)))));

                return updateNeeded;
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to check for updates due an error", ex);
                EventBus.Trigger(this, new StartupProcessChanged(L("UpdatedNotFound")));
                return false;
            }
        }

        private ILocalizationSource LocalizationSource => _localizationManager.GetSource(KontecgWinFormsConsts.LocalizationSourceName);

        /// <summary>
        ///     Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return LocalizationSource?.GetString(name) ?? name;
        }

        /// <summary>
        ///     Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, params object[] args)
        {
            return LocalizationSource?.GetString(name, args) ?? string.Format(name, args);
        }
    }
}
