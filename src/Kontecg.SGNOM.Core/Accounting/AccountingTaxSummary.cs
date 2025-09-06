using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Kontecg.HumanResources;
using System;
using NMoneys;
using Kontecg.Taxes;

namespace Kontecg.Accounting
{
    [Table("accounting_tax_summaries", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class AccountingTaxSummary : CreationAuditedEntity<long>, IMustHaveCompany
    {
        [Required]
        public virtual int DocumentId { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [Required]
        public virtual long PersonId { get; set; }

        [NotMapped]
        public virtual Person Person { get; set; }

        [Required]
        public virtual Guid GroupId { get; set; }

        [Required]
        public virtual Money Tax { get; set; }

        /// <summary>
        ///     Currency.
        ///     Default: CUP
        /// </summary>
        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        [Required]
        public virtual TaxType TaxType { get; set; }

        /// <inheritdoc />
        public AccountingTaxSummary(int documentId, long personId, Guid groupId, Money tax, CurrencyIsoCode currency, TaxType taxType)
        {
            DocumentId = documentId;
            PersonId = personId;
            GroupId = groupId;
            Tax = tax;
            Currency = currency;
            TaxType = taxType;
        }

        /// <inheritdoc />
        public AccountingTaxSummary(int documentId, long personId, Guid groupId, decimal tax, CurrencyIsoCode currency, TaxType taxType)
        {
            DocumentId = documentId;
            PersonId = personId;
            GroupId = groupId;
            Tax = new Money(tax, currency);
            Currency = currency;
            TaxType = taxType;
        }
    }
}