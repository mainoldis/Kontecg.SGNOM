using NMoneys;
using System;
using JetBrains.Annotations;
using Kontecg.HumanResources.Dto;
using Kontecg.MultiCompany.Dto;
using Kontecg.Auditing;

namespace Kontecg.Accounting.Dto
{
    public class PersonalAccountingInfoDto
    {
        public CompanyListDto Company { get; set; }

        public PersonDto Person { get; set; }

        public int? Exp { get; set; }

        public int? Account { get; set; }

        public int? CenterCost { get; set; }

        public bool? IsContract { get; set; }

        public bool? PayablePerRate { get; set; }

        public bool PayablePerATM { get; set; }

        [CanBeNull]
        [DisableAuditing]
        public string BankAccount { get; set; }

        public CurrencyIsoCode? Currency { get; set; }

        public Guid? DocumentGroup { get; set; }

        public long? LastDocumentId { get; set; }

        public long? DocumentId { get; set; }

        [CanBeNull]
        public string Type { get; set; }
    }
}
