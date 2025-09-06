using Kontecg.HumanResources;

namespace Kontecg.Taxes
{
    public static class TaxPersonInfoExtensions
    {
        public static TaxPersonInfo CreateTaxInfo(this PersonTax personTax)
        {
            return new TaxPersonInfo(personTax.PersonId, personTax.GroupId, personTax.TaxType, personTax.MathType,
                personTax.Factor, personTax.Formula, personTax.CompanyId);
        }
    }
}