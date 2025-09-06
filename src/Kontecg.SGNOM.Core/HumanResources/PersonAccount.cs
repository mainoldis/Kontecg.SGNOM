using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using NMoneys;

namespace Kontecg.HumanResources
{
    [Table("person_accounts", Schema = "docs")]
    public class PersonAccount : FullAuditedEntity<long>, IPassivable, ISoftDelete
    {
        public const int AccountNumberMaxLength = 20;

        /// <summary>
        ///     Id of the Person.
        /// </summary>
        [Required]
        public virtual long PersonId { get; set; }

        [NotMapped]
        public virtual Person Person { get; set; }

        /// <summary>
        ///     Bank Account Number.
        /// </summary>
        [Required]
        [StringLength(AccountNumberMaxLength)]
        public virtual string AccountNumber { get; set; }

        /// <summary>
        ///     Currency of Bank Account Number.
        ///     Default: CUP
        /// </summary>
        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        [Required]
        public virtual bool IsActive { get; set; }

        public PersonAccount()
        {
            IsActive = true;
        }

        public PersonAccount(long personId, string accountNumber, string currency = KontecgCoreConsts.DefaultCurrency)
            :this()
        {
            PersonId = personId;
            AccountNumber = accountNumber;
            Currency = Enum.TryParse(typeof(CurrencyIsoCode), currency, out object result)
                ? (CurrencyIsoCode) result
                : CurrencyIsoCode.CUP;
        }
    }
}
