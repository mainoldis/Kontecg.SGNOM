using System.Threading.Tasks;
using Kontecg.Organizations;

namespace Kontecg.WorkRelations
{
    public interface IEmploymentSettingStore
    {
        EmploymentIndexSettingRecord GetIndexRanges(int? companyId);

        Task<EmploymentIndexSettingRecord> GetIndexRangesAsync(int? companyId);

        AllowEmploymentsOutOfTemplateOptions GetAllowEmploymentsOutOfTemplateOptions(int? companyId);

        Task<AllowEmploymentsOutOfTemplateOptions> GetAllowEmploymentsOutOfTemplateOptionsAsync(int? companyId);
    }
}
