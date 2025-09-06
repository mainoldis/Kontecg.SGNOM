using Kontecg.Domain.Repositories;
using Kontecg.Salary;
using NMoneys;
using System;
using System.Threading.Tasks;

namespace Kontecg.HistoricalData
{
    public interface IPaymentHistogramRepository : IRepository<PaymentHistogram, long>
    {
        /// <summary>
        ///    Search for the histogram of Payments
        /// </summary>
        /// <param name="personId">person id</param>
        /// <param name="groupId">Employment group id</param>
        /// <param name="wageAdjuster">Salary component to search for the average wage</param>
        /// <param name="months">Months to evaluate</param>
        /// <param name="currencyIsoCode">Currency Iso Code</param>
        /// <returns></returns>
        Money AverageByPersonId(long personId, Guid groupId, WageAdjuster wageAdjuster = WageAdjuster.Salary, int months = 12, CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP);

        /// <summary>
        ///    Search for the histogram of Payments
        /// </summary>
        /// <param name="personId">person id</param>
        /// <param name="groupId">Employment group id</param>
        /// <param name="wageAdjuster">Salary component to search for the average wage</param>
        /// <param name="months">Months to evaluate</param>
        /// <param name="currencyIsoCode">Currency Iso Code</param>
        /// <returns></returns>
        Task<Money> AverageByPersonIdAsync(long personId, Guid groupId, WageAdjuster wageAdjuster = WageAdjuster.Salary, int months = 12, CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP);
    }
}