using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;

namespace Kontecg.Presenters
{
    public static class FilterControlWithoutLikeHelper
    {
        public static void Apply(FilterControl filterControl)
        {
            filterControl.ShowGroupCommandsIcon = true;
            filterControl.PopupMenuShowing += FilterControl_PopupMenuShowing;
        }

        private static void FilterControl_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            e.Menu.Remove(ClauseType.Like);
            e.Menu.Remove(ClauseType.NotLike);
        }
    }
}