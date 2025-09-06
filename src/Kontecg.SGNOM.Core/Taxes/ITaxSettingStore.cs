using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kontecg.Taxes
{
    public interface ITaxSettingStore
    {
        IReadOnlyDictionary<TaxType, TaxInfo> GetTaxesInfo(int? companyId);

        Task<IReadOnlyDictionary<TaxType, TaxInfo>> GetTaxesInfoAsync(int? companyId);
    }
}
