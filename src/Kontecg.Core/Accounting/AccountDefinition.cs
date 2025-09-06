using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;

namespace Kontecg.Accounting
{
    [Table("account_definitions", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public class AccountDefinition : FullAuditedEntity, IPassivable
    {
        /// <summary>
        ///     Max length of the <see cref="Description" /> property.
        /// </summary>
        public const int MaxDescriptionLength = 150;

        /// <summary>
        ///     Max length of the <see cref="Reference" /> property.
        /// </summary>
        public const int MaxReferenceLength = 50;

        [Required]
        public virtual int Account { get; set; }

        [Required]
        public virtual int SubAccount { get; set; }

        [Required]
        public virtual int SubControl { get; set; }

        [Required]
        public virtual int Analysis { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }

        [StringLength(MaxReferenceLength)]
        public virtual string Reference { get; set; }

        [Required]
        public virtual AccountKind Kind { get; set; }

        public virtual bool IsActive { get; set; }

        public AccountDefinition()
        {
            IsActive = true;
        }

        public AccountDefinition(int account, int subAccount, int subControl, int analysis, string description, AccountKind kind, string reference = null)
            : this()
        {
            Account = account;
            SubAccount = subAccount;
            SubControl = subControl;
            Analysis = analysis;
            Description = description;
            Kind = kind;
            Reference = reference ?? $"CTA_{Account}_{SubAccount}";

            SetNormalizedDescription();
            SetNormalizedReference();
        }
        public virtual void SetNormalizedDescription()
        {
            Description = Description?.ToUpperInvariant();
        }
        public virtual void SetNormalizedReference()
        {
            Reference = Reference?.ToUpperInvariant();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Analysis > 0)
                sb.Insert(0, $".{Analysis}");
            if (SubControl > 0 || (SubControl == 0 && Analysis > 0))
                sb.Insert(0, $".{SubControl}");
            if (SubAccount > 0 || (SubAccount == 0 && (SubControl > 0 || Analysis > 0)))
                sb.Insert(0, $".{SubAccount}");

            sb.Insert(0, $"{Account}");
            return $"{sb} - {Description}";
        }
    }
}
