using System;
using System.ComponentModel.DataAnnotations;
using Kontecg.Domain;
using Kontecg.Runtime.Validation;

namespace Kontecg.Currencies.Dtos
{
    public class ExchangeInputDto : IShouldNormalize
    {
        [Required]
        public string Provider { get; set; } = KontecgCoreConsts.DefaultExchangeRateProvider;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        public DateTime? Since { get; set; }

        public DateTime? Until { get; set; }

        public ScopeData? Scope { get; set; }

        public void Normalize()
        {
            Provider = Provider.ToUpper();
            From = From.ToUpper();
            To = To.ToUpper();
        }
    }
}
