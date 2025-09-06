using System;
using System.Threading.Tasks;
using Kontecg.HumanResources;
using NMoneys;

namespace Kontecg.Taxes
{
    public interface ITaxManager
    {
        void CreateRange(TaxType type, TaxRangeRecord range);

        Task CreateRangeAsync(TaxType type, TaxRangeRecord range);

        void CreateTaxForPerson(PersonTax taxPerson);

        Task CreateTaxForPersonAsync(PersonTax taxPerson);

        decimal Calculate(long personId, Guid groupId, TaxType taxType, decimal amount, decimal discount = 0);

        Money Calculate(long personId, Guid groupId, TaxType taxType, Money amount, Money? discount = null);
    }
}