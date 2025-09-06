using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Domain.Repositories;
using NMoneys;

namespace Kontecg.HistoricalData
{
    public interface IHolidayHistogramRepository : IRepository<HolidayHistogram, long>
    {
        /// <summary>
        ///    Search for the histogram of holidays
        /// </summary>
        /// <param name="personId">person id</param>
        /// <param name="groupId">Employment group id</param>
        /// <param name="currencyIsoCode">Currency Iso Code</param>
        /// <returns></returns>
        HolidayHistogramRecord AggregateByPersonId(long personId, Guid groupId, CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP);

        /// <summary>
        ///    Search for the histogram of holidays
        /// </summary>
        /// <param name="personId">person id</param>
        /// <param name="groupId">Employment group id</param>
        /// <param name="currencyIsoCode">Currency Iso Code</param>
        /// <returns></returns>
        Task<HolidayHistogramRecord> AggregateByPersonIdAsync(long personId, Guid groupId, CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP);

        /// <summary>
        ///    Search for the histogram of holidays
        /// </summary>
        /// <param name="currencyIsoCode">Currency Iso Code</param>
        /// <returns></returns>
        IReadOnlyList<HolidayHistogramRecord> Aggregate(CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP);

        /// <summary>
        ///    Search for the histogram of holidays
        /// </summary>
        /// <param name="currencyIsoCode">Currency Iso Code</param>
        /// <returns></returns>
        Task<IReadOnlyList<HolidayHistogramRecord>> AggregateAsync(CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP);
    }
}
