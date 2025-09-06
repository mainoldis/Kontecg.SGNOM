using System.Threading.Tasks;
using Castle.Core.Logging;

namespace Kontecg.Accounting
{
    /// <summary>
    ///     Implements <see cref="IPersonalAccountingInfoStore" /> to simply write personal accounting info to logs.
    /// </summary>
    internal class SimplePersonalAccountingInfoStore : IPersonalAccountingInfoStore
    {
        public SimplePersonalAccountingInfoStore()
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        /// <summary>
        ///     Singleton instance.
        /// </summary>
        public static SimplePersonalAccountingInfoStore Instance { get; } = new();

        public Task SaveAsync(PersonalAccountingInfo accountingInfo)
        {
            Logger.Info(accountingInfo.ToString());
            return Task.FromResult(Task.FromResult(0));
        }

        public void Save(PersonalAccountingInfo accountingInfo)
        {
            Logger.Info(accountingInfo.ToString());
        }
    }
}
