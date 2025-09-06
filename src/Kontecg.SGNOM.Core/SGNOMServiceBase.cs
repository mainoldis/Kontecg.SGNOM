namespace Kontecg
{
    public abstract class SGNOMServiceBase : KontecgServiceBase
    {
        protected SGNOMServiceBase()
        {
            LocalizationSourceName = SGNOMConsts.LocalizationSourceName;
        }
    }
}