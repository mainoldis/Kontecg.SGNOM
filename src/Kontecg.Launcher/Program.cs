using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Taskbar;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.EFCore;
using Kontecg.Extensions;
using Kontecg.Runtime;
using ILoggerFactory = Castle.Core.Logging.ILoggerFactory;
using Velopack;

namespace Kontecg
{
    public static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            //Initialization code
            AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;

            //Update checks
            VelopackApp.Build()
                .Run();

            OnAppRun();
        }

        private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (IocManager.Instance.IsRegistered<ILoggerFactory>())
            {
                var logger = IocManager.Instance.Resolve<ILoggerFactory>().Create(typeof(Program));
                logger.Fatal(((Exception) e.ExceptionObject).ToString);
            }
        }

        private static Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var startIndex = args.Name.IndexOf(',');
            var partialName = (startIndex >= 0 ? args.Name.Remove(startIndex) : args.Name).ToLower();
            if ((partialName.StartsWith("devexpress") ||
                partialName == "nhibernate" ||
                partialName == "system.data.sqlite" ||
                partialName == "microsoft.data.sqlite" ||
                partialName == "npgsql") && !partialName.Contains("resources"))
            {
                var path = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? "..",
                    partialName + ".dll");
                return Assembly.LoadFrom(path);
            }

            return null;
        }

        private static void OnAppRun()
        {
            //Check and register default connectionstring if it's missing on config.json
            CheckMissingConfig();

            KontecgApplication<MainModule> app = new KontecgApplication<MainModule>();

            app.Start(resolver =>
            {
                //Setup for WinForms Application, remove this line if it's a console app
                PrepareSettings();

                using var context = resolver.ResolveAsDisposable<IWinFormsRuntime>();
                context.Object.Run();

            }, typeof(SGNOMCoreModule), typeof(SGNOMDataModule), typeof(SGNOMServicesModule), typeof(SGNOMPresentationModule));

            app.End();
        }

        private static void PrepareSettings()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);

            if (Debugging.DebugHelper.IsDebug)
                DevExpress.Utils.Localization.XtraLocalizer.EnableTraceSource();
            //else
            //DevExpress.Utils.Localization.XtraLocalizer.UserResourceManager = DXLocalization.ResourceManager;

            var defaultFont = Presenters.FontHelper.GetFont("Microsoft Sans Serif", 8.25f);
            var printingFont = Presenters.FontHelper.GetFont("Verdana", 10F);
            TaskbarAssistant.Default.Initialize();
            
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession && Screen.AllScreens.Length > 1)
                DevExpress.XtraEditors.WindowsFormsSettings.SetPerMonitorDpiAware();
            else
                DevExpress.XtraEditors.WindowsFormsSettings.SetDPIAware();

            WindowsFormsSettings.EnableFormSkins();
            WindowsFormsSettings.DefaultLookAndFeel.SetSkinStyle(SkinStyle.Office2019Colorful);
            WindowsFormsSettings.DefaultRibbonStyle = DefaultRibbonControlStyle.Office2019;
            WindowsFormsSettings.FindPanelBehavior = FindPanelBehavior.Search;
            WindowsFormsSettings.FilterCriteriaDisplayStyle = FilterCriteriaDisplayStyle.Visual;
            WindowsFormsSettings.PreferredDateTimeCulture = DateTimeCulture.CurrentUICulture;
            WindowsFormsSettings.AllowPixelScrolling = DefaultBoolean.True;
            WindowsFormsSettings.DefaultFont = defaultFont;
            WindowsFormsSettings.DefaultPrintFont = printingFont;
            AppearanceObject.DefaultFont = defaultFont;
            WindowsFormsSettings.ScrollUIMode = ScrollUIMode.Default;
            WindowsFormsSettings.CustomizationFormSnapMode = SnapMode.OwnerControl;
            WindowsFormsSettings.ColumnFilterPopupMode = ColumnFilterPopupMode.Excel;
            WindowsFormsSettings.AllowSkinEditorAttach = DefaultBoolean.True;
            WindowsFormsSettings.AllowRoundedWindowCorners = DefaultBoolean.True;
            WindowsFormsSettings.AllowRoundedWindowCorners = DefaultBoolean.True;

            WindowsFormsSettings.DefaultSettingsCompatibilityMode = SettingsCompatibilityMode.v18_1;
            System.Windows.Forms.Application.SetDefaultFont(defaultFont);

        }

        private static void CheckMissingConfig()
        {
            var configurationAccessor = new DefaultAppConfigurationAccessor();
            if (configurationAccessor.Configuration[$"ConnectionStrings:{KontecgCoreConsts.ConnectionStringName}"].IsNullOrWhiteSpace())
            {
                XtraInputBoxForm inputBoxForm = new();
                DialogResult dialogResult = inputBoxForm.ShowInputBoxDialog(new XtraInputBoxArgs(
                    UserLookAndFeel.Default, prompt: "ConnectionString",
                    title: "Setup connection string?"));

                while (dialogResult != DialogResult.OK || inputBoxForm.InputResult.As<string>() == null)
                {
                    dialogResult = inputBoxForm.ShowInputBoxDialog(new XtraInputBoxArgs(
                        UserLookAndFeel.Default, prompt: "I need a real database connection string:",
                        title: "Requesting a valid connection string"));
                }

                var writer = new DefaultAppConfigurationWriter();
                writer.Write($"ConnectionStrings:{KontecgCoreConsts.ConnectionStringName}", inputBoxForm.InputResult.As<string>());
            }
        }
    }
}
