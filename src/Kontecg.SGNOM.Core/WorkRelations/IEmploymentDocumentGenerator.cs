namespace Kontecg.WorkRelations
{
    public interface IEmploymentDocumentGenerator
    {
        EmploymentDocument Clone(EmploymentDocument document);

        EmploymentDocument Clone(EmploymentDocument document, EmploymentDocumentToGenerate legalChange);
    }
}