using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NMoneys;

namespace Kontecg.EFCore.ValueConverters
{
    /// <summary>
    /// Defines conversions from an instance of <see cref="Money"/> in a model to a <see cref="string"/> in the store.
    /// </summary>
    public class QuantityValueConverter : ValueConverter<Money, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuantityValueConverter"/> class.
        /// </summary>
        public QuantityValueConverter() : base(
            m => m.AsQuantity(),
            str => Money.Parse(str))
        { }
    }
}