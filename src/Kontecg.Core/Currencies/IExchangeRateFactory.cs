using NMoneys;
using System;
using Kontecg.Currencies.Exchange;
using Kontecg.Domain;

namespace Kontecg.Currencies
{
    public interface IExchangeRateFactory
    {
        ExchangeRate CreateExchangeRate(string provider, CurrencyIsoCode from, CurrencyIsoCode to, decimal rate, DateTime since, DateTime until, ScopeData scope = ScopeData.Company);
    }
}
