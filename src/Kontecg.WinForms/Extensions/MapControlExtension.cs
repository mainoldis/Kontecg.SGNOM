using DevExpress.XtraMap;
using Kontecg.Runtime;

namespace Kontecg.Extensions
{
    public static class MapControlExtension
    {
        public static void Export(this MapControl mapControl, string path)
        {
            mapControl.ExportToImage(path, DevExpress.Drawing.DXImageFormat.Png);
            AppHelper.ProcessStart(path);
        }
    }
}