using System.IO;

namespace Kontecg.Domain
{
    public interface ISupportLayout
    {
        void SaveLayoutToStream(MemoryStream ms);

        void RestoreLayoutFromStream(MemoryStream ms);
    }
}
