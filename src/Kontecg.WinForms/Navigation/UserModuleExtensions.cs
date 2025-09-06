using System.Collections.Generic;
using System.Linq;
using Kontecg.Views;

namespace Kontecg.Navigation
{
    public static class UserModuleExtensions
    {
        /// <summary>
        /// Order UserMenuItem by custom values
        /// </summary>
        /// <param name="moduleItems"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<Module> OrderByCustom(this IList<Module> moduleItems)
        {
            return moduleItems
                   .OrderBy(moduleItem => moduleItem.Order)
                   .ThenBy(moduleItem => moduleItem.Order == 0 ? null : moduleItem.DisplayName);
        }
    }
}
