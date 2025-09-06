using System;
using System.Collections.Generic;

namespace Kontecg.Taxes
{
    [Serializable]
    public class TaxInfo
    {
        public TaxInfo()
        {
            Percent = 0;
            Reference = "EMPTY";
        }

        public TaxInfo(TaxType type, decimal percent, string reference = "EMPTY")
        {
            Type = type;

            if (decimal.IsNegative(percent) || percent > 100)
                throw new ArgumentOutOfRangeException(nameof(percent), percent, "Must be between 0 and 100");

            Percent = percent;
            Ranges = new List<TaxRangeRecord>();
            Persons = new List<TaxPersonInfo>();
            Reference = reference;
        }

        public TaxType Type { get; set; }

        public IReadOnlyList<TaxRangeRecord> Ranges { get; set; }

        public IReadOnlyList<TaxPersonInfo> Persons { get; set; }

        public string Reference { get; set; }

        public decimal Percent { get; set; }
    }
}