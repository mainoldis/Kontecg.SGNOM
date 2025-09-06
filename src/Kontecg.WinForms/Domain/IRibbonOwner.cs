using DevExpress.XtraBars.Ribbon;

namespace Kontecg.Domain
{
    public interface IRibbonOwner
    {
        RibbonControl Ribbon { get; }
    }
}
