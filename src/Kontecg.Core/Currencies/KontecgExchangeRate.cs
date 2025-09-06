using Kontecg.Data;
using NMoneys;
using NMoneys.Exchange;
using System;
using Kontecg.Domain;

namespace Kontecg.Currencies
{
    public class KontecgExchangeRate : ExchangeRate
    {
        public KontecgExchangeRate(string provider, CurrencyIsoCode from, CurrencyIsoCode to, decimal rate, DateTime since, DateTime until, ScopeData scope = ScopeData.Company)
            : base(from, to, rate)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));

            Provider = provider;
            Since = since;
            Until = until;
            Scope = scope;
        }

        public string Provider { get; }

        public DateTime Since { get; }

        public DateTime Until { get; }

        public ScopeData Scope { get; }
    }
}
