using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kontecg.Extensions
{
    public static class PropertyDictionaryExtensions
    {
        public static IDictionary<string, object> ToPropertyDictionary(
            this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
        {
            return source.ToDictionaryInternal(bindingAttr);
        }

        /// <summary>Converts an object into a dictionary.</summary>
        public static IReadOnlyDictionary<string, object> ToPropertyReadOnlyDictionary(
            this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
        {
            return source.ToDictionaryInternal(bindingAttr);
        }

        private static Dictionary<string, object> ToDictionaryInternal(
            this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary((Func<PropertyInfo, string>)(propInfo => propInfo.Name.ToUpperInvariant()), (Func<PropertyInfo, object>)(propInfo => propInfo.GetValue(source, null)));
        }
    }
}