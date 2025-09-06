using System;
using System.IO;
using System.Linq;
using Kontecg.Reflection.Extensions;

namespace Kontecg.IO
{
    /// <summary>
    ///     This class is used to find root path of the kontecg project in;
    ///     unit tests (to find views) and entity framework core command line commands (to find conn string).
    /// </summary>
    public static class KontecgContentDirectoryFinder
    {
        public static string CalculateContentRootFolder()
        {
            var coreAssemblyDirectoryPath = Path.GetDirectoryName(typeof(KontecgCoreModule).GetAssembly().Location);
            if (coreAssemblyDirectoryPath == null)
                throw new Exception("Could not find location of Kontecg.Core assembly!");

            var directoryInfo = new DirectoryInfo(coreAssemblyDirectoryPath);
            while (!DirectoryContains(directoryInfo.FullName, "GlobalAssemblyInfo.cs"))
                directoryInfo = directoryInfo.Parent ?? throw new Exception("Could not find content root folder!");

            var desktopFolder = 
                Path.Combine(directoryInfo.FullName, $"src{Path.DirectorySeparatorChar}Kontecg.Migrator");
            if (Directory.Exists(desktopFolder)) return desktopFolder;

            throw new Exception("Could not find root folder of the Kontecg project!");
        }

        private static bool DirectoryContains(string directory, string fileName)
        {
            return Directory.GetFiles(directory).Any(filePath => string.Equals(Path.GetFileName(filePath), fileName));
        }
    }
}
