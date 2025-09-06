using NMoneys;

namespace Kontecg.Model
{
    public class PersonAccountView
    {
        public virtual long PersonId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Surname { get; set; }

        public virtual string Lastname { get; set; }

        public virtual string IdentityCard { get; set; }

        public virtual string AccountNumber { get; set; }

        public virtual CurrencyIsoCode? Currency { get; set; }

        public virtual bool? IsActive { get; set; }
    }
}