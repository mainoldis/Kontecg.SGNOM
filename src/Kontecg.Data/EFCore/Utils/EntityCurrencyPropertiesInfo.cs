using System.Collections.Generic;
using System.Reflection;

namespace Kontecg.EFCore.Utils
{
    public class EntityCurrencyPropertiesInfo
    {
        public List<PropertyInfo> CurrencyPropertyInfos { get; set; }

        public List<string> ComplexTypePropertyPaths { get; set; }
    }
}
