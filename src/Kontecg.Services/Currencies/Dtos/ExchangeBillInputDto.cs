using Kontecg.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace Kontecg.Currencies.Dtos
{
    public class ExchangeBillInputDto : IShouldNormalize
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string From { get; set; }

        public void Normalize()
        {
            From = From.ToUpper();
        }
    }
}
