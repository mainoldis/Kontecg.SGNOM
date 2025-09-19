using System;
using Kontecg.Currencies.Exchange;
using Kontecg.Domain;

namespace Kontecg.Currencies
{
    public interface IKontecgExchangeRateProvider : IExchangeRateProvider
    {
        string Provider { get; set; }

        DateTime? Since { get; set; }

        DateTime? Until { get; set; }

        ScopeData Scope { get; set; }
    }
}
