using System.Threading.Tasks;

namespace Kontecg.Accounting
{
    public interface IPersonalAccountingHelper
    {
        PersonalAccountingInfo[] CreatePersonalAccountingInfo();

        Task<PersonalAccountingInfo[]> CreatePersonalAccountingInfoAsync();

        void Save(PersonalAccountingInfo accountingInfo);

        Task SaveAsync(PersonalAccountingInfo accountingInfo);
    }
}
