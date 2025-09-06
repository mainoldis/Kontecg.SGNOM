using System.ComponentModel.DataAnnotations;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain;
using NMoneys;
using System.Text;

namespace Kontecg.Accounting
{
    [Table("accounting_voucher_notes", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Host)]
    public class AccountingVoucherNote : CreationAuditedEntity<long>, IMustHaveCompany
    {
        public virtual int DocumentId { get; set; }

        [Required]
        [ForeignKey("DocumentId")]
        public virtual AccountingVoucherDocument Document { get; set; }

        [Required]
        public virtual AccountOperation Operation { get; set; }

        public virtual int ScopeId { get; set; }

        [Required]
        [ForeignKey("ScopeId")]
        public virtual AccountingClassifierDefinition Scope { get; set; }

        [Required]
        public virtual int Account { get; set; }

        [Required]
        public virtual int SubAccount { get; set; }

        [Required]
        public virtual int SubControl { get; set; }

        [Required]
        public virtual int Analysis { get; set; }

        [Required]
        public virtual Money Amount { get; set; }

        /// <summary>
        ///     Currency.
        ///     Default: CUP
        /// </summary>
        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        public virtual int CompanyId { get; set; }

        public AccountingVoucherNote()
        {
            ScopeId = (int) ScopeData.Company;
            Currency = CurrencyIsoCode.CUP;
        }

        public AccountingVoucherNote(AccountingVoucherDocument document, AccountOperation operation, int account, int subAccount, int subControl, int analysis, Money amount, int scope = (int) ScopeData.Company)
        {
            Document = document;
            Operation = operation;
            Account = account;
            SubAccount = subAccount;
            SubControl = subControl;
            Analysis = analysis;
            Amount = amount;
            Currency = amount.CurrencyCode;
            ScopeId = scope;
        }

        public AccountingVoucherNote(int documentId, AccountOperation operation, int account, int subAccount, int subControl, int analysis, Money amount, int scope = (int)ScopeData.Company)
        {
            DocumentId = documentId;
            Operation = operation;
            Account = account;
            SubAccount = subAccount;
            SubControl = subControl;
            Analysis = analysis;
            Amount = amount;
            Currency = amount.CurrencyCode;
            ScopeId = scope;
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
            return $"({ScopeId}) {sb} - {Amount.ToString()} - {Operation}";
        }
    }
}
