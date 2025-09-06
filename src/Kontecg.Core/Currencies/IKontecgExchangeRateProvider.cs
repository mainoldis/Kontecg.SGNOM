using System;
using Kontecg.Domain;
using NMoneys.Exchange;

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
