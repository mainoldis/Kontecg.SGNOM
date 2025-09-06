using System;
using Kontecg.Salary;

namespace Kontecg.Taxes
{
    [Serializable]
    public class TaxPersonInfo
    {
        public TaxPersonInfo()
        {
        }

        public TaxPersonInfo(long personId, Guid groupId, TaxType taxType, MathType mathType, decimal factor, string formula, int companyId)
        {
            PersonId = personId;
            GroupId = groupId;
            TaxType = taxType;
            MathType = mathType;
            Factor = factor;
            Formula = formula;
            CompanyId = companyId;
        }

        public long PersonId { get; set; }

        public Guid? GroupId { get; set; }

        public TaxType TaxType { get; set; }

        public MathType MathType { get; set; }

        public decimal Factor { get; set; }

        public string Formula { get; set; }

        public int CompanyId { get; set; }
    }
}