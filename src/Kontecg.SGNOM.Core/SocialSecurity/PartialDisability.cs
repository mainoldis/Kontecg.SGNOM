using System.ComponentModel.DataAnnotations;
using NMoneys;

namespace Kontecg.SocialSecurity
{
    public class PartialDisability : SubsidyDocument
    {
        [Required]
        public virtual Money FixedAmountPerMonth { get; set; }

        /// <inheritdoc />
        public PartialDisability()
        {
            FixedAmountPerMonth = Money.Zero();
        }
    }
}
