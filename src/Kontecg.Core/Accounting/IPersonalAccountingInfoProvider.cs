using System.Threading.Tasks;

namespace Kontecg.Accounting
{
    /// <summary>
    ///     Provides an interface to provide personal accounting information in the upper layers.
    /// </summary>
    public interface IPersonalAccountingInfoProvider
    {
        /// <summary>
        ///     Called to fill needed properties.
        /// </summary>
        PersonalAccountingInfo[] GetPersonalAccountingInformation(int? companyId);

        Task<PersonalAccountingInfo[]> GetPersonalAccountingInformationAsync(int? companyId);
    }
}
