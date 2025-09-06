using System.ComponentModel.DataAnnotations;

namespace Kontecg.Currencies.Dtos
{
    public class MoneyDto
    {
        public MoneyDto()
        {
            Currency = KontecgCoreConsts.DefaultCurrency;
        }

        public MoneyDto(decimal amount, string currency = KontecgCoreConsts.DefaultCurrency)
        {
            Amount = amount;
            Currency = currency;
        }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Amount} {Currency}";
        }
    }
}
