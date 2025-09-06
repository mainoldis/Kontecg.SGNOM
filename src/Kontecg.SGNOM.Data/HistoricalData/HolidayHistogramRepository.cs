using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Accounting;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using NMoneys;

namespace Kontecg.HistoricalData
{
    public class HolidayHistogramRepository : SGNOMRepositoryBase<HolidayHistogram, long>, IHolidayHistogramRepository
    {
        public HolidayHistogramRepository(IDbContextProvider<SGNOMDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public HolidayHistogramRecord AggregateByPersonId(long personId, Guid groupId, CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP)
        {
            var result = GetAll().Where(h => h.Status == AccountingNoteStatus.Made && h.PersonId == personId && h.GroupId == groupId && h.Currency == currencyIsoCode)
                .GroupBy(x => new {x.PersonId, x.GroupId})
                .ToList()
                .Select(g => new HolidayHistogramRecord(g.Key.PersonId, g.Key.GroupId, g.Sum(x => x.Hours), new Money(g.Sum(x => x.Amount.Amount), currencyIsoCode)))
                .SingleOrDefault();

            return result ?? new HolidayHistogramRecord(personId, groupId,0, Money.Zero(currencyIsoCode));
        }

        public async Task<HolidayHistogramRecord> AggregateByPersonIdAsync(long personId, Guid groupId, CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP)
        {
            var result = (await GetAllListAsync()).Where(h => h.Status == AccountingNoteStatus.Made && h.PersonId == personId && h.GroupId == groupId && h.Currency == currencyIsoCode)
                .GroupBy(x => new { x.PersonId, x.GroupId })
                .ToList()
                .Select(g => new HolidayHistogramRecord(g.Key.PersonId, g.Key.GroupId, g.Sum(x => x.Hours), new Money(g.Sum(x => x.Amount.Amount), currencyIsoCode)))
                .SingleOrDefault();

            return result ?? new HolidayHistogramRecord(personId, groupId, 0, Money.Zero(currencyIsoCode));
        }

        public IReadOnlyList<HolidayHistogramRecord> Aggregate(CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP)
        {
            var result = GetAll().Where(h => h.Status == AccountingNoteStatus.Made && h.Currency == currencyIsoCode)
                .GroupBy(x => new { x.PersonId, x.GroupId })
                .ToList()
                .Select(g => new HolidayHistogramRecord(g.Key.PersonId, g.Key.GroupId, g.Sum(x => x.Hours), new Money(g.Sum(x => x.Amount.Amount), currencyIsoCode)))
                .ToList();

            return result;
        }

        public async Task<IReadOnlyList<HolidayHistogramRecord>> AggregateAsync(CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP)
        {
            var result = (await GetAllListAsync()).Where(h => h.Status == AccountingNoteStatus.Made && h.Currency == currencyIsoCode)
                .GroupBy(x => new { x.PersonId, x.GroupId })
                .ToList()
                .Select(g => new HolidayHistogramRecord(g.Key.PersonId, g.Key.GroupId, g.Sum(x => x.Hours), new Money(g.Sum(x => x.Amount.Amount), currencyIsoCode)))
                .ToList();

            return result;
        }
    }
}
