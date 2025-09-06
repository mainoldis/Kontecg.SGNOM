using System.Threading.Tasks;

namespace Kontecg.Salary
{
    public interface IPaymentSettingStore
    {
        PaymentSettingRecord GetPaymentDefinition(int? companyId);

        Task<PaymentSettingRecord> GetPaymentDefinitionAsync(int? companyId);

        string GetNormalWorkShiftName(int? companyId);

        Task<string> GetNormalWorkShiftNameAsync(int? companyId);
    }
}
