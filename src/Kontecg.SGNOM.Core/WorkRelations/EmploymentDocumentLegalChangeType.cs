using System;

namespace Kontecg.WorkRelations
{
    [Flags]
    public enum EmploymentDocumentLegalChangeType
    {
        None = 0, // No se modifica ningún elemento, pero genera un nuevo movimiento
        Validity = 1, // Elementos de fecha (Vencimiento)
        Type = 2, // Tipo de contrato o Tipo de movimiento
        OrganizationUnit = 4, // Elementos estructurales como cambios en la plantilla
        CenterCost = 8, // Centro de costo
        Occupation = 16, // Elementos del cargo incluyendo salario
        Plus = 32, // Elementos de pagos adicionales
        WorkShift = 64, // Turnos de trabajo
        EmployeeSalaryForm = 128, // Formas de pago
        Summary = 256, // Motivos del movimiento
        OccupationWithPlus = Occupation | Plus, // Elementos del cargo incluyendo salario y adiciones
        AllWithoutEmployeeSalaryForm = Validity | Type | OrganizationUnit | CenterCost | OccupationWithPlus | WorkShift | Summary,
        All = Validity | Type | OrganizationUnit | CenterCost | OccupationWithPlus | WorkShift | EmployeeSalaryForm | Summary,
    }
}