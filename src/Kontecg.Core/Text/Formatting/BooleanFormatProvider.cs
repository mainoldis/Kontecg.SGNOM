using System;

namespace Kontecg.Text.Formatting
{
    public class BooleanFormatProvider : IFormatProvider, ICustomFormatter
    {
        public static BooleanFormatProvider Instance = new();

        public object GetFormat(Type formatType) => formatType is ICustomFormatter ? this : null;

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var flag = Convert.ToBoolean(arg);

            format = format?.Trim().ToLower();

            return format switch
            {
                "io" => !flag ? "0" : "1",
                "sn" => !flag ? "No" : "Si",
                _ => arg is IFormattable ? ((IFormattable) arg).ToString(format, formatProvider) : arg.ToString()
            };
        }
    }
}
