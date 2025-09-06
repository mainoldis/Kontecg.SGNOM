using System;
using System.Threading.Tasks;
using Kontecg.WorkRelations;

namespace Kontecg.Data
{
    public interface ICodeGenerator
    {
        string CreateEmploymentCode(DateTime effectiveDate, EmploymentType relationType);

        Task<string> CreateEmploymentCodeAsync(DateTime effectiveDate, EmploymentType relationType);

        string CreateTimeDistributionDocumentCode(DateTime workTime);

        Task<string> CreateTimeDistributionDocumentCodeAsync(DateTime workTime);

        string CreateHolidayDocumentCode(DateTime workTime);

        Task<string> CreateHolidayDocumentCodeAsync(DateTime workTime);

        Guid CreateOrUpdateEmploymentGroupId(Guid? id = null);

        Task<Guid> CreateOrUpdateEmploymentGroupIdAsync(Guid? id = null);
    }
}
