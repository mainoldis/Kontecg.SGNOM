using NMoneys;
using NMoneys.Exchange;
using System;
using Kontecg.Domain;

namespace Kontecg.Currencies
{
    internal class DefaultExchangeRateFactory : IExchangeRateFactory
    {
        public ExchangeRate CreateExchangeRate(string provider, CurrencyIsoCode from, CurrencyIsoCode to, decimal rate, DateTime since, DateTime until, ScopeData scope = ScopeData.Company)
        {
            return new KontecgExchangeRate(provider, from, to, rate, since, until, scope);
        }
    }
}
