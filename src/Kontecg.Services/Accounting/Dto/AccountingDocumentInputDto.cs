using System;
using System.ComponentModel.DataAnnotations;
using Kontecg.Runtime.Validation;
using Kontecg.Timing;

namespace Kontecg.Accounting.Dto
{
    public class AccountingDocumentInputDto : IShouldNormalize
    {
        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public string Reference { get; set; }

        [Required]
        public string ReferenceGroup { get; set; }

        public string Description { get; set; }

        public DateTime? MadeOn { get; set; }

        /// <inheritdoc />
        public void Normalize()
        {
            Description = Description?.Trim().ToUpperInvariant();
            Reference = Reference?.Trim().ToUpperInvariant();
            ReferenceGroup = ReferenceGroup?.Trim().ToUpperInvariant();
            MadeOn = MadeOn.HasValue ? Clock.Normalize(MadeOn.Value) : null;
        }
    }
}