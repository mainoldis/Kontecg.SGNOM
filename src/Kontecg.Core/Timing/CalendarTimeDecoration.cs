using Itenso.TimePeriod;
using Kontecg.Primitives;

namespace Kontecg.Timing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CalendarTimeDecoration
    {
        public CalendarTimeDecoration(ITimeRange range, CalendarTimeDecorator decorator,
            Priority priority = Priority.Normal)
        {
            Range = range;
            Decorator = decorator;
            Priority = priority;
        }

        public ITimeRange Range { get; }

        public CalendarTimeDecorator Decorator { get; }

        public Priority Priority { get; }

        public override string ToString()
        {
            return $"{Range} | {Decorator}";
        }
    }
}
