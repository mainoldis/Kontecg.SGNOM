using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NMoneys;

namespace Kontecg.EFCore.Utils
{
    public static class CurrencyPropertyInfoHelper
    {
        /// <summary>
        ///     Key: Entity type
        ///     Value: Currency property infos
        /// </summary>
        private static readonly ConcurrentDictionary<Type, EntityCurrencyPropertiesInfo> CurrencyProperties;

        static CurrencyPropertyInfoHelper()
        {
            CurrencyProperties = new ConcurrentDictionary<Type, EntityCurrencyPropertiesInfo>();
        }

        public static EntityCurrencyPropertiesInfo GetCurrencyPropertyInfos(Type entityType)
        {
            return CurrencyProperties.GetOrAdd(
                entityType,
                key => FindCurrencyPropertyInfosForType(entityType)
            );
        }

        /// <summary>
        ///     Gets Currency properties
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private static EntityCurrencyPropertiesInfo FindCurrencyPropertyInfosForType(Type entityType)
        {
            var currencyProperties = entityType.GetProperties()
                .Where(property =>
                    (property.PropertyType == typeof(Money) ||
                     property.PropertyType == typeof(Money?)) &&
                    property.CanWrite &&
                    !property.IsDefined(typeof(NotMappedAttribute))
                ).ToList();

            var complexTypeProperties = entityType.GetProperties()
                .Where(p => p.PropertyType.IsDefined(typeof(OwnedAttribute), true))
                .ToList();

            var complexTypeCurrencyPropertyPaths = new List<string>();
            foreach (var complexTypeProperty in complexTypeProperties)
                AddComplexTypeCurrencyPropertyPaths(entityType.FullName + "." + complexTypeProperty.Name,
                    complexTypeProperty, complexTypeCurrencyPropertyPaths);

            return new EntityCurrencyPropertiesInfo
            {
                CurrencyPropertyInfos = currencyProperties,
                ComplexTypePropertyPaths = complexTypeCurrencyPropertyPaths
            };
        }

        private static void AddComplexTypeCurrencyPropertyPaths(string pathPrefix, PropertyInfo complexProperty,
            List<string> complexTypeCurrencyPropertyPaths)
        {
            if (!complexProperty.PropertyType.IsDefined(typeof(OwnedAttribute), true)) return;

            var complexTypeCurrencyProperties = complexProperty.PropertyType
                .GetProperties()
                .Where(property =>
                    (property.PropertyType == typeof(Money) ||
                     property.PropertyType == typeof(Money?)) &&
                    property.CanWrite
                ).Select(p => pathPrefix + "." + p.Name).ToList();

            complexTypeCurrencyPropertyPaths.AddRange(complexTypeCurrencyProperties);

            var complexTypeProperties = complexProperty.PropertyType.GetProperties()
                .Where(p => p.PropertyType.IsDefined(typeof(OwnedAttribute), true))
                .ToList();

            if (!complexTypeProperties.Any()) return;

            foreach (var complexTypeProperty in complexTypeProperties)
                AddComplexTypeCurrencyPropertyPaths(pathPrefix + "." + complexTypeProperty.Name, complexTypeProperty,
                    complexTypeCurrencyPropertyPaths);
        }
    }
}
