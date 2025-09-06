using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;

namespace Kontecg.Presenters
{
    public static class FontHelper
    {
        private static readonly Dictionary<string, Font> FontCache;

        static FontHelper()
        {
            FontCache = new Dictionary<string, Font>();
        }

        public static Font GetSegoeUIFont(FontStyle fontStyle)
        {
            float defaultSize = DevExpress.Utils.AppearanceObject.DefaultFont.Size;
            return GetFont("Segoe UI", defaultSize, fontStyle);
        }

        public static Font GetSegoeUIFont(float sizeGrow = 0)
        {
            float defaultSize = DevExpress.Utils.AppearanceObject.DefaultFont.Size;
            return GetFont("Segoe UI", defaultSize + sizeGrow);
        }

        public static Font GetSegoeUILightFont(float sizeGrow = 0)
        {
            float defaultSize = DevExpress.Utils.AppearanceObject.DefaultFont.Size;
            return GetFont("Segoe UI Light", defaultSize + sizeGrow);
        }

        public static Font GetFont(string familyName, float size, FontStyle style = FontStyle.Regular)
        {
            string key = familyName + "#" + size.ToString(CultureInfo.InvariantCulture);

            if (style != FontStyle.Regular) key += ("#" + style);

            if (!FontCache.TryGetValue(key, out Font result))
            {
                try
                {
                    var family = FindFontFamily(familyName);
                    result = new Font(family ?? FontFamily.GenericSansSerif, size, style);
                }
                catch (ArgumentException)
                {
                    result = DevExpress.Utils.AppearanceObject.DefaultFont;
                }

                FontCache.Add(key, result);
            }
            return result;
        }

        private static FontFamily FindFontFamily(string familyName)
        {
            return Array.Find(FontFamily.Families, (f) => f.Name == familyName);
        }
    }
}
