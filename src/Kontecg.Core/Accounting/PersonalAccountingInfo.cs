using NMoneys;
using System;
using JetBrains.Annotations;
using Kontecg.HumanResources;
using Kontecg.MultiCompany;

namespace Kontecg.Accounting
{
    /// <summary>
    ///     This information are collected for an ... method.
    /// </summary>
    public class PersonalAccountingInfo
    {
        /// <summary>
        ///     CompanyId.
        /// </summary>
        public Company Company { get; set; }

        public Person Person { get; set; }

        public int? Exp { get; set; }

        public int? Account { get; set; }

        public int? CenterCost { get; set; }

        public bool? IsContract { get; set; }

        public bool? PayablePerRate { get; set; }

        public bool PayablePerATM { get; set; }

        [CanBeNull]
        public string BankAccount { get; set; }

        public CurrencyIsoCode? Currency { get; set; }

        public Guid? DocumentGroup { get; set; }

        public long? LastDocumentId { get; set; }

        public long? DocumentId { get; set; }

        [CanBeNull]
        public string Type { get; set; }

        public override string ToString()
        {
            return $"{nameof(Company)}: {Company}, {nameof(Person)}: {Person}, {nameof(Exp)}: {Exp}, {nameof(Account)}: {Account}, {nameof(CenterCost)}: {CenterCost}, {nameof(IsContract)}: {IsContract}, {nameof(PayablePerRate)}: {PayablePerRate}, {nameof(PayablePerATM)}: {PayablePerATM}, {nameof(BankAccount)}: {BankAccount}, {nameof(Currency)}: {Currency}, {nameof(DocumentGroup)}: {DocumentGroup}, {nameof(LastDocumentId)}: {LastDocumentId}, {nameof(DocumentId)}: {DocumentId}, {nameof(Type)}: {Type}";
        }
    }
}
