using System;
using System.Drawing;
using DevExpress.Utils.Svg;

namespace Kontecg.Runtime
{
    public interface IWinFormsRuntime
    {
        WeakReference MainForm { get; }
        
        bool RunningContext { get; }
        
        Icon AppIcon { get; }

        SvgImage AppImage { get; }
        
        void Run();
    }
}
