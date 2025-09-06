using Itenso.TimePeriod;
using System;
using System.Collections.Generic;
using System.Linq;
using Kontecg.Extensions;

namespace Kontecg.Timing
{
    public static class TimePeriodCollectionExtensions
    {
        public static Dictionary<CalendarTimeDecorator, TimeSpan> Summarize(
            this ITimePeriodCollection timePeriodCollection)
        {
            Check.NotNull(timePeriodCollection, nameof(timePeriodCollection));

            DurationProvider dp = new();

            var summary = timePeriodCollection.SelectMany(k => k.As<WorkingHours>().Decorators,
                    (period, collection) => new { Period = period, Decorator = collection })
                .GroupBy(x => x.Decorator.Decorator)
                .ToDictionary(
                    g => g.Key,
                    g => TimeSpan.FromHours(g.Sum(x => x.Decorator.Range.Duration.TotalHours))
                );

            if (!summary.ContainsKey(CalendarTimeDecorator.WorkingTime))
                summary.Add(CalendarTimeDecorator.WorkingTime,
                    TimeSpan.FromHours(timePeriodCollection.GetTotalDuration(dp).TotalHours));

            return summary;
        }
    }
}
