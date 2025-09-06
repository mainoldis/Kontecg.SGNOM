using Kontecg.Domain.Entities.Auditing;
using NMoneys;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.Currencies
{
    [Table("bill_denominations", Schema = "gen")]
    public class BillDenomination : AuditedEntity, IPassivable
    {
        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        [Required]
        public virtual decimal Bill { get; set; }

        public virtual bool IsActive { get; set; }

        public BillDenomination()
        {
            IsActive = true;
        }

        public BillDenomination(CurrencyIsoCode currency, decimal bill, bool isActive = true)
        {
            Currency = currency;
            Bill = bill;
            IsActive = isActive;
        }

    }
}
