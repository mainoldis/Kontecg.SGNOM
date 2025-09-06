using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NMoneys;

namespace Kontecg.EFCore.ValueConverters
{
    public class KontecgCurrencyValueConverter() : ValueConverter<Money?, decimal?>(ConvertToDecimal, ConvertToMoney,
        new ConverterMappingHints(precision: 10, scale: 2))
    {
        private static readonly Expression<Func<Money?, decimal?>> ConvertToDecimal = x =>
            x.HasValue ? x.Value.Amount : null;

        private static readonly Expression<Func<decimal?, Money?>> ConvertToMoney = x =>
            x.HasValue ? new Money(x.Value, KontecgCoreConsts.DefaultCurrency) : null;
    }
}
