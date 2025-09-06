using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kontecg.IO
{
    public static class AppFileHelper
    {
        public static IEnumerable<string> ReadLines(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000,
                FileOptions.SequentialScan);
            using var sr = new StreamReader(fs, Encoding.UTF8);
            while (sr.ReadLine() is { } line) yield return line;
        }

        public static void DeleteFilesInFolderIfExists(string folderPath, string fileNameWithoutExtension)
        {
            var directory = new DirectoryInfo(folderPath);
            var temps = directory.GetFiles(fileNameWithoutExtension + ".*", SearchOption.AllDirectories)
                .ToList();
            foreach (var tempe in temps)
                FileHelper.DeleteIfExists(tempe.FullName);
        }

        public static void SaveToFile(string path, byte[] stream)
        {
            if (stream == null || stream is {Length: 0}) return;

            using FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            fs.Write(stream, 0, stream.Length);
            fs.Flush();
        }

        public static string NormalizeToFileName(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Normalizar caracteres acentuados y diacríticos
            string normalized = input.Normalize(NormalizationForm.FormD);

            // Paso 1: Reemplazar caracteres acentuados por sus equivalentes sin acento
            // Paso 2: Reemplazar símbolos no alfanuméricos por "_"
            // Paso 3: Reducir múltiples "_" consecutivos
            return Regex.Replace(normalized, @"[^\p{L}\p{N}]+", "_")
                        .Replace("_", " ")  // Temporal para manejar espacios
                        .Trim()             // Eliminar espacios sobrantes al inicio/final
                        .Replace(" ", "_"); // Revertir espacios a guiones
        }
    }
}
