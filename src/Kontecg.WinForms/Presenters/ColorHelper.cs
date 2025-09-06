using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;

namespace Kontecg.Presenters
{
    public static class ColorHelper
    {
        public static void UpdateColor(ImageList list, UserLookAndFeel lf)
        {
            for (int i = 0; i < list.Images.Count; i++)
                list.Images[i] = SetColor(list.Images[i] as Bitmap, GetHeaderForeColor(lf));
        }

        public static Color GetHeaderForeColor(UserLookAndFeel lf)
        {
            Color ret = SystemColors.ControlText;
            if (lf.ActiveStyle != ActiveLookAndFeelStyle.Skin) return ret;
            return GridSkins.GetSkin(lf)[GridSkins.SkinHeader].Color.GetForeColor();
        }

        static Bitmap SetColor(Bitmap bmp, Color color)
        {
            for (int i = 0; i < bmp.Width; i++)
            for (int j = 0; j < bmp.Height; j++)
                if (bmp.GetPixel(i, j).Name != "0")
                    bmp.SetPixel(i, j, color);
            return bmp;
        }

        public static Color DisabledTextColor => CommonSkins.GetSkin(DevExpress.LookAndFeel.UserLookAndFeel.Default).Colors.GetColor("DisabledText");

        public static Color CriticalColor => CommonColors.GetCriticalColor(DevExpress.LookAndFeel.UserLookAndFeel.Default);

        public static Color WarningColor => CommonColors.GetWarningColor(DevExpress.LookAndFeel.UserLookAndFeel.Default);

        public static string GetRgbColor(Color color)
        {
            return $"{color.R},{color.G},{color.B}";
        }
    }
}
