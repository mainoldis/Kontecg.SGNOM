using System.ComponentModel.DataAnnotations;

namespace Kontecg.Currencies.Dtos
{
    public class BillDto
    {
        public BillDto()
        {
            Currency = KontecgCoreConsts.DefaultCurrency;
        }

        public BillDto(int count, decimal bill, string currency = KontecgCoreConsts.DefaultCurrency)
        {
            Count = count;
            Bill = bill;
            Currency = currency;
        }

        [Required]
        public decimal Bill { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public string Currency { get; set; }
    }
}
