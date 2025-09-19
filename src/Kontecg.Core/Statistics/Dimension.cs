using System;
using Itenso.TimePeriod;
using Kontecg.Localization;

namespace Kontecg.Statistics
{
    public abstract class Dimension<TInput,TResult>(string name, LocalizableString displayName, Func<TInput,TResult> transform = null)
    {
        public int Year { get; protected set; }

        public YearMonth Month { get; protected set; }

        public int MonthDay { get; protected set; } = 1;

        public Date Date => new Date(Year, (int) Month, MonthDay);

        public string Name { get; protected set; } = name;

        public LocalizableString DisplayName { get; protected set; } = displayName;

        public TResult Value { get; protected set; } = default;
    }
}