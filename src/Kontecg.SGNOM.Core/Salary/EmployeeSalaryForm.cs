namespace Kontecg.Salary
{
    public enum EmployeeSalaryForm : byte
    {
        Average = 1, //Mensual, se paga salario escala y se deduce lo que dejó de trabajar
        Royal = 2, // Jornalero, se paga por tarifa horaria
        Piece = 3 // Jornalero a destajo, se tiene en cuenta indicadores a cumplir?
    }
}
