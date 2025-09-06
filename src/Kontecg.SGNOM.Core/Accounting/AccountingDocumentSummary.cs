using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.HumanResources;
using Kontecg.MultiCompany;
using NMoneys;

namespace Kontecg.Accounting
{
    [Table("accounting_document_summaries", Schema = "cnt")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class AccountingDocumentSummary : CreationAuditedEntity<long>, IMustHaveCompany
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
        public virtual Guid? GroupId { get; set; }

        [Required]
        public virtual Money Payment { get; set; }

        [Required]
        public virtual Money PaymentPlus { get; set; }

        [Required]
        public virtual decimal ReservedForHoliday { get; set; }

        [Required]
        public virtual Money AmountReservedForHoliday { get; set; }

        [Required]
        public virtual Money SocialSecurityTaxes { get; set; }

        [Required]
        public virtual Money IncomeTaxes { get; set; }

        [Required]
        public virtual Money Retentions { get; set; }

        [NotMapped]
        public virtual Money NetIncome => Money.Total(Payment.Abs(), PaymentPlus.Abs(), SocialSecurityTaxes.Abs().Negate(), IncomeTaxes.Abs().Negate(), Retentions.Abs().Negate());

        /// <summary>
        ///     Currency.
        ///     Default: CUP
        /// </summary>
        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        public AccountingDocumentSummary(int documentId, long personId, Guid? groupId, Money payment,
            Money paymentPlus, decimal reservedForHoliday, Money amountReservedForHoliday, Money socialSecurityTaxes,
            Money incomeTaxes, Money retentions, CurrencyIsoCode currency)
        {
            DocumentId = documentId;
            PersonId = personId;
            GroupId = groupId;
            Currency = currency;
            Payment = new Money(payment.Amount, currency);
            PaymentPlus = new Money(paymentPlus.Amount, currency);
            ReservedForHoliday = reservedForHoliday;
            AmountReservedForHoliday = new Money(amountReservedForHoliday.Amount, currency);
            SocialSecurityTaxes = new Money(socialSecurityTaxes.Amount, currency);
            IncomeTaxes = new Money(incomeTaxes.Amount, currency);
            Retentions = new Money(retentions.Amount, currency);
        }

        public AccountingDocumentSummary(int documentId, long personId, Guid? groupId, decimal payment,
            decimal paymentPlus, decimal reservedForHoliday, decimal amountReservedForHoliday,
            decimal socialSecurityTaxes, decimal incomeTaxes, decimal retentions, CurrencyIsoCode currency)
        {
            DocumentId = documentId;
            PersonId = personId;
            GroupId = groupId;
            Currency = currency;
            Payment = new Money(payment, currency);
            PaymentPlus = new Money(paymentPlus, currency);
            ReservedForHoliday = reservedForHoliday;
            AmountReservedForHoliday = new Money(amountReservedForHoliday, currency);
            SocialSecurityTaxes = new Money(socialSecurityTaxes, currency);
            IncomeTaxes = new Money(incomeTaxes, currency);
            Retentions = new Money(retentions, currency);
        }
    }
}
