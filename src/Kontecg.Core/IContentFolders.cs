using System.Collections.Generic;

namespace Kontecg
{
    public interface IContentFolders
    {
        string ExtensionsFolder { get; set; }

        string LogsFolder { get; set; }

        string DataFolder { get; set; }

        string TempFolder { get; set; }

        Dictionary<string, string> AdditionalFolders { get; }
    }
}
