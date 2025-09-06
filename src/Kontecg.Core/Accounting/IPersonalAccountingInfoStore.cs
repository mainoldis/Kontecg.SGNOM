using System.Threading.Tasks;

namespace Kontecg.Accounting
{
    /// <summary>
    ///     This interface should be implemented by vendors to
    ///     make personal accounting working.
    ///     Default implementation is <see cref="SimplePersonalAccountingInfoStore" />.
    /// </summary>
    public interface IPersonalAccountingInfoStore
    {
        /// <summary>
        ///     Should save personal accounting info to a persistent store.
        /// </summary>
        /// <param name="accountingInfo">Personal accounting information</param>
        Task SaveAsync(PersonalAccountingInfo accountingInfo);

        /// <summary>
        ///     Should save personal accounting info to a persistent store.
        /// </summary>
        /// <param name="accountingInfo">Personal accounting information</param>
        void Save(PersonalAccountingInfo accountingInfo);
    }
}
