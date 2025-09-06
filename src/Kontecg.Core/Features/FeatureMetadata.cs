using System;
using System.Drawing;
using Kontecg.Localization;

namespace Kontecg.Features
{
    public class FeatureMetadata
    {
        public const string CustomFeatureKey = "FeatureMetadata";

        public FeatureMetadata()
        {
            TextColor = Color.FromName;
            IsVisibleOnInfoTable = false;
        }

        public Func<string, ILocalizableString> ValueTextNormalizer { get; set; }

        public bool IsVisibleOnInfoTable { get; set; }

        public Func<string, Color> TextColor { get; set; }
    }
}
