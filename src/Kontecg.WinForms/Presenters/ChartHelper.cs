using System.Drawing;

namespace Kontecg.Presenters
{
    public static class ChartHelper
    {
        public static Color GetBackColor(DevExpress.XtraCharts.ChartControl chartControl)
        {
            return ((DevExpress.XtraCharts.Native.IChartContainer)chartControl).Chart.ActualBackColor;
        }
    }
}
