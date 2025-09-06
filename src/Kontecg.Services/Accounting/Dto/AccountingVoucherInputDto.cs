using Kontecg.Runtime.Validation;
using Kontecg.Timing;
using System;
using System.ComponentModel.DataAnnotations;

namespace Kontecg.Accounting.Dto
{
    public class AccountingVoucherInputDto : IShouldNormalize
    {
        [Required]
        public int DocumentId { get; set; }

        public string Description { get; set; }

        public DateTime? MadeOn { get; set; }

        /// <inheritdoc />
        public void Normalize()
        {
            Description = Description?.Trim().ToUpperInvariant();
            MadeOn = MadeOn.HasValue ? Clock.Normalize(MadeOn.Value) : null;
        }
    }
}