using System;
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils.Svg;
using Kontecg.Reflection.Extensions;

namespace Kontecg.Presenters
{
    public static class SvgHelper
    {
        //TODO:Las imágenes se deben obtener desde los recursos pero usando IoC
        public static Image CreateImageFromSvg(Type resourceType, ISkinProvider skinProvider, string rootPath, string imageName)
        {
            var assembly = resourceType.GetAssembly();
            SvgBitmap svgBitmap;
            var stream = assembly.GetManifestResourceStream(rootPath + imageName) ?? assembly.GetManifestResourceStream(imageName);
            if (stream == null) return null;
            using (stream)
            {
                svgBitmap = SvgBitmap.FromStream(stream);
            }
            if (svgBitmap == null) return null;
            var pallete = SvgPaletteHelper.GetSvgPalette(skinProvider, DevExpress.Utils.Drawing.ObjectState.Normal);
            return svgBitmap.Render(pallete, 1);
        }

        public static Image CreateImageFromSvg(Type resourceType, ISkinProvider skinProvider, string rootPath, string imageName, Size imageSize)
        {
            var assembly = resourceType.GetAssembly();
            SvgBitmap svgBitmap;
            var stream = assembly.GetManifestResourceStream(rootPath + imageName) ??
                         assembly.GetManifestResourceStream(imageName);

            if (stream == null) return null;
            using (stream)
            {
                svgBitmap = SvgBitmap.FromStream(stream);
            }
            if (svgBitmap == null) return null;
            var pallete = SvgPaletteHelper.GetSvgPalette(skinProvider, DevExpress.Utils.Drawing.ObjectState.Normal);
            return svgBitmap.Render(imageSize, pallete);

        }
    }
}
