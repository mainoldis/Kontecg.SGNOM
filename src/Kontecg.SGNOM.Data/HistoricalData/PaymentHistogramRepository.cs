using System;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Accounting;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using Kontecg.Salary;
using NMoneys;

namespace Kontecg.HistoricalData
{
    public class PaymentHistogramRepository : SGNOMRepositoryBase<PaymentHistogram, long>, IPaymentHistogramRepository
    {
        public PaymentHistogramRepository(IDbContextProvider<SGNOMDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        /// <inheritdoc />
        public Money AverageByPersonId(long personId, Guid groupId, WageAdjuster wageAdjuster = WageAdjuster.Average,
            int months = 12, CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP)
        {
            Money Selector(PaymentHistogram p)
            {
                var money = Money.Zero(currencyIsoCode);
                if ((wageAdjuster & WageAdjuster.Salary) == WageAdjuster.Salary) money = Money.Add(money, p.SalaryPaymentReceived);
                if ((wageAdjuster & WageAdjuster.Bonus) == WageAdjuster.Bonus) money = Money.Add(money, p.BonusPaymentReceived);
                if ((wageAdjuster & WageAdjuster.Holiday) == WageAdjuster.Holiday) money = Money.Add(money, p.HolidaysPaymentReceived);
                if ((wageAdjuster & WageAdjuster.Plus) == WageAdjuster.Plus) money = Money.Add(money, p.SalaryPlusPaymentReceived);
                if ((wageAdjuster & WageAdjuster.Subsidy) == WageAdjuster.Subsidy) money = Money.Add(money, p.SickLeavePaymentReceived);
                return money;
            }

            var query = GetAll()
                        .Where(h => h.Status == AccountingNoteStatus.Made && h.PersonId == personId && h.GroupId == groupId && h.Currency == currencyIsoCode)
                        .GroupBy(x => new { x.GroupId, x.Year, x.Month })
                        .AsEnumerable()
                        .OrderByDescending(p => p.Key.Year)
                        .ThenByDescending(p => p.Key.Month)
                        .ToList()
                        .Select(g => new { g.Key.GroupId, g.Key.Year, g.Key.Month, Total = new Money(g.Sum(x => Selector(x).Amount), currencyIsoCode) })
                        .Take(months).ToList();

            var realCount = query.Count(p => p.Total > Money.Zero(currencyIsoCode));
            var total = query.Sum(p => p.Total.Amount);

            return new Money(realCount > 0 ? total / realCount : total, currencyIsoCode);
        }

        /// <inheritdoc />
        public async Task<Money> AverageByPersonIdAsync(long personId, Guid groupId, WageAdjuster wageAdjuster = WageAdjuster.Salary,
            int months = 12, CurrencyIsoCode currencyIsoCode = CurrencyIsoCode.CUP)
        {
            Money Selector(PaymentHistogram p)
            {
                var money = Money.Zero(currencyIsoCode);
                if ((wageAdjuster & WageAdjuster.Salary) == WageAdjuster.Salary) money = Money.Add(money, p.SalaryPaymentReceived);
                if ((wageAdjuster & WageAdjuster.Bonus) == WageAdjuster.Bonus) money = Money.Add(money, p.BonusPaymentReceived);
                if ((wageAdjuster & WageAdjuster.Holiday) == WageAdjuster.Holiday) money = Money.Add(money, p.HolidaysPaymentReceived);
                if ((wageAdjuster & WageAdjuster.Plus) == WageAdjuster.Plus) money = Money.Add(money, p.SalaryPlusPaymentReceived);
                if ((wageAdjuster & WageAdjuster.Subsidy) == WageAdjuster.Subsidy) money = Money.Add(money, p.SickLeavePaymentReceived);
                return money;
            }

            var query = (await GetAllAsync())
                        .Where(h => h.Status == AccountingNoteStatus.Made && h.PersonId == personId && h.GroupId == groupId && h.Currency == currencyIsoCode)
                        .GroupBy(x => new { x.GroupId, x.Year, x.Month })
                        .OrderByDescending(p => p.Key.Year)
                        .ThenByDescending(p => p.Key.Month)
                        .ToList()
                        .Select(g => new { g.Key.GroupId, g.Key.Year, g.Key.Month, Total = new Money(g.Sum(x => Selector(x).Amount), currencyIsoCode) })
                        .Take(months).ToList();

            var realCount = query.Count(p => p.Total > Money.Zero(currencyIsoCode));
            var total = query.Sum(p => p.Total.Amount);

            return new Money(realCount > 0 ? total / realCount : total, currencyIsoCode);
        }
    }
}