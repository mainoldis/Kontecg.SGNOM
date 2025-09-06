namespace Kontecg.Domain
{
    /// <summary>
    /// Data generation status in the system
    /// </summary>
    public enum GenerationSystemData : byte
    {
        Unknown = 0,
        Reported = 1,
        Presented = 2,
        Generated = 3,
    }
}
