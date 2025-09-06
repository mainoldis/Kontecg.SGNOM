namespace Kontecg.Extensions
{
    public static class SizeExtensions
    {
        private static readonly string[] SizeUnits = new[] {"Bytes", "KB", "MB", "GB", "TB", "PB"};

        public static string ToSize(this ulong size)
        {
            if (size == 0) return "0 Bytes";

            int parts = 0;
            ulong integral = size;

            while (integral >= 1024 && parts < SizeUnits.Length)
            {
                integral /= 1024;
                parts++;
            }

            return $"{integral} {SizeUnits[parts]}";
        }
    }
}
