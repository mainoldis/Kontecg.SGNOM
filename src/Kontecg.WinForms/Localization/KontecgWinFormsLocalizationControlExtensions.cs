using DevExpress.XtraBars.Ribbon;
using Kontecg.Localization.Sources;
using System.Linq;

namespace Kontecg.Localization
{
    public static class KontecgWinFormsLocalizationControlExtensions
    {
        private static readonly string[] LocalizableProperties =
        [
            "Text", "Caption", "Hint", "ToolTip", "TabName",
            "CustomizationFormText", "Header", "Description",
            "Title", "Value", "ToolTipText", "SuperTip"
        ];

        public static void LocalizeWithSource(this object control, ILocalizationSource source)
        {
            if (control == null || source == null) return;

            switch (control)
            {
                case RibbonControl ribbon:
                    foreach (RibbonPage page in ribbon.Pages)
                    {
                        LocalizeSingleControl(page, source);
                        foreach (RibbonPageGroup group in page.Groups)
                        {
                            group.LocalizeWithSource(source);
                            foreach (var itemLink in group.ItemLinks)
                                itemLink.LocalizeWithSource(source);
                        }
                    }
                    break;
                case DevExpress.XtraBars.BarManager barManager:
                    foreach (DevExpress.XtraBars.Bar bar in barManager.Bars) bar.LocalizeWithSource(source);
                    break;
                case DevExpress.XtraBars.Bar bar:
                    LocalizeSingleControl(bar, source);
                    break;

                case RibbonPage page:
                    LocalizeSingleControl(page, source);
                    break;

                case RibbonPageGroup group:
                    LocalizeSingleControl(group, source);
                    break;

                case DevExpress.XtraBars.BarButtonItem buttonItem:
                    LocalizeSingleControl(buttonItem, source);
                    break;

                case DevExpress.XtraGrid.GridControl grid:
                    if (grid.MainView != null)
                        grid.MainView.LocalizeWithSource(source);
                    break;

                case DevExpress.XtraGrid.Views.Grid.GridView view:
                    foreach (DevExpress.XtraGrid.Columns.GridColumn column in view.Columns)
                        column.LocalizeWithSource(source);
                    break;
                case DevExpress.XtraGrid.Columns.GridColumn column:
                    LocalizeSingleControl(column, source);
                    break;
            }
        }

        private static void LocalizeSingleControl(object control, ILocalizationSource source)
        {
            var properties = control.GetType().GetProperties()
                                    .Where(p => LocalizableProperties.Contains(p.Name) &&
                                                p.CanWrite &&
                                                p.PropertyType == typeof(string))
                                    .ToArray();

            foreach (var property in properties)
            {
                var currentValue = property.GetValue(control) as string;
                if (!string.IsNullOrWhiteSpace(currentValue))
                {
                    var key = currentValue.Replace(" ", "_");
                    var translatedText = source.GetString(key);
                    if (!string.IsNullOrEmpty(translatedText))
                        property.SetValue(control, translatedText);
                }
            }
        }
    }
}
