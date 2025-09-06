using Itenso.TimePeriod;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Kontecg.Linq.Extensions
{
    public static class TimePeriodExtensions
    {
        public static IQueryable<T> WhereInTimePeriod<T>(this IQueryable<T> source, Func<T, ITimePeriod> selector, ITimePeriod timePeriod)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Invoke(Expression.Constant(selector), parameter);
            var start = Expression.Property(property, "Start");
            var end = Expression.Property(property, "End");

            var parameterStart = Expression.Constant(timePeriod.Start);
            var parameterEnd = Expression.Constant(timePeriod.End);
            var condition = Expression.AndAlso(
                Expression.GreaterThanOrEqual(start, parameterStart),
                Expression.LessThanOrEqual(end, parameterEnd)
            );

            return source.Where(Expression.Lambda<Func<T, bool>>(condition, parameter));
        }

        public static IQueryable<T> WhereIfInTimePeriod<T>(this IQueryable<T> source, bool condition, Func<T, ITimePeriod> selector, ITimePeriod timePeriod)
        {
            return condition ? source.WhereInTimePeriod(selector, timePeriod) : source;
        }

        public static Expression<Func<T, bool>> ToTimePeriod<T>(this T source, Func<T, ITimePeriod> selector, ITimePeriod timePeriod)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Invoke(Expression.Constant(selector), parameter);
            var start = Expression.Property(property, "Start");
            var end = Expression.Property(property, "End");

            var parameterStart = Expression.Constant(timePeriod.Start);
            var parameterEnd = Expression.Constant(timePeriod.End);
            var condition = Expression.AndAlso(
                Expression.GreaterThanOrEqual(start, parameterStart),
                Expression.LessThanOrEqual(end, parameterEnd)
            );

            return Expression.Lambda<Func<T, bool>>(condition, parameter);
        }
    }
}
