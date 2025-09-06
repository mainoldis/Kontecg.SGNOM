namespace Kontecg.Timing
{
    public enum PeriodOpenMode
    {
        None = 0, //No permite ningún período abierto
        CurrentMonth = 1, //Solo permite el mes activo abierto
        LastMonth = 2, //Solo permite el mes anterior abierto
        Any = 3, //Permite cualquier período abierto
    }
}