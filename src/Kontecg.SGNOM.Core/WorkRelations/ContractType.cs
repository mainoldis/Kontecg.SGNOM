namespace Kontecg.WorkRelations
{
    public enum ContractType : byte
    {
        I = 1,
        D = 2,
    }

    /// <summary>
    /// Used for generate Exp value to specify ranges
    /// </summary>
    public enum ContractSubType : byte
    {
        I = 1, //1 a 39999
        A = 2, //60000 a 69999
        D = 3, //40000 a 59999 
        P = 4, //70000+
    }
}
