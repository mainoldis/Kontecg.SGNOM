using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Itenso.TimePeriod;
using Kontecg.Data;
using Kontecg.Domain;
using Kontecg.Domain.Entities.Auditing;
using NMoneys;

namespace Kontecg.Currencies
{
    [Table("exchange_rates", Schema = "cnt")]
    public class ExchangeRateInfo : AuditedEntity
    {
        public virtual int BankId { get; set; }

        [Required]
        [ForeignKey("BankId")]
        public virtual Bank Bank { get; set; }

        [Required]
        public virtual CurrencyIsoCode From { get; set; }

        [Required]
        public virtual CurrencyIsoCode To { get; set; }

        [Required]
        public virtual decimal Rate { get; set;}

        [Required]
        public virtual ScopeData Scope { get; set; }

        [Required]
        public virtual DateTime Since { get; set; }

        public virtual DateTime Until { get; set; }

        public ExchangeRateInfo()
        {
        }

        public ExchangeRateInfo(int bankId, CurrencyIsoCode from, CurrencyIsoCode to, decimal rate, DateTime since, DateTime until, ScopeData scope = ScopeData.Company)
        {
            BankId = bankId;
            From = from;
            To = to;
            Rate = rate;
            Since = since;
            Until = until;
            Scope = scope;
        }
        
        public ITimePeriod ToTimePeriod()
        {
            return new TimeRange(Since, Until, true);
        }
    }
}
