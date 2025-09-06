using System.Drawing;
using DevExpress.Data.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars.Ribbon;
using Kontecg.Dependency;
using Kontecg.Views;

namespace Kontecg.Runtime
{
    public static class AppHelper
    {
        private static readonly IWinFormsRuntime Runtime = IocManager.Instance.Resolve<IWinFormsRuntime>();

        public static RibbonForm MainForm => Runtime.MainForm?.Target as BaseRibbonForm;

        public static SvgImage AppIcon => Runtime.AppImage;

        private static Image _img;

        public static Image AppImage
        {
            get { return _img ??= AppIcon.Render(null); }
        }

        public static void ProcessStart(string name)
        {
            ProcessStart(name, string.Empty);
        }

        public static void ProcessStart(string name, string arguments)
        {
            try
            {
                SafeProcess.Open(name, arguments);
            }
            catch (System.ComponentModel.Win32Exception)
            {
            }
        }
    }
}