namespace Kontecg.Domain
{
    public interface ISupportModifications
    {
        bool Modified { get; }
        void Save();
    }
}
