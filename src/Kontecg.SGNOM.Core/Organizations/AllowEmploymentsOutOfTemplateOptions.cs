namespace Kontecg.Organizations
{
    public enum AllowEmploymentsOutOfTemplateOptions
    {
        Strict, //Solo se permite una plaza fija y un movimiento provisional, contrato o período a prueba
        Flexible, //Se permite una plaza fija y tantos movimientos no indeterminados como se requiera por plaza (Fuera del marco legislativo)
        NotAllowed //Se permite solo una plaza fija
    }
}
