using System.Collections.Generic;

namespace Kontecg.Views
{
    public interface IUserModuleManager
    {
        Module Default { get; }

        /// <summary>
        ///     Gets all modules specialized for given user.
        /// </summary>
        /// <param name="user">User id or null for anonymous users</param>
        IList<Module> GetModules(UserIdentifier user);
    }
}