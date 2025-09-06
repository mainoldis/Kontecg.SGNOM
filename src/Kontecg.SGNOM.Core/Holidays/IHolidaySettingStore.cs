using System.Threading.Tasks;

namespace Kontecg.Holidays
{
    public interface IHolidaySettingStore
    {
        int GetMaxAllowedDays(int? companyId);

        Task<int> GetMaxAllowedDaysAsync(int? companyId);
    }
}
