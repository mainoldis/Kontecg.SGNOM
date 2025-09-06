using System.Collections.Generic;
using Kontecg.Dependency;

namespace Kontecg
{
    public class ContentFolders : IContentFolders, ISingletonDependency
    {
        public ContentFolders()
        {
            AdditionalFolders = new Dictionary<string, string>();
        }
        public string ExtensionsFolder { get; set; }
        public string LogsFolder { get; set; }
        public string DataFolder { get; set; }
        public string TempFolder { get; set; }

        public Dictionary<string, string> AdditionalFolders { get; private set; }
    }
}
