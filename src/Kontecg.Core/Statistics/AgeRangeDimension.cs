using Kontecg.Localization;

namespace Kontecg.Statistics
{
    public class AgeRangeDimension(string name, int input) 
        : Dimension<int, string>(name, new LocalizableString(name, KontecgCoreConsts.LocalizationSourceName), x => GetRangeForAge(input))
    {
        private static string GetRangeForAge(int age)
        {
            return age switch
                   {
                       >= 0 and <= 25 => "<=25",
                       > 25 and <= 35 => "26-35",
                       > 35 and <= 45 => "36-45",
                       > 45 and <= 50 => "46-50",
                       > 50 and <= 55 => "51-55",
                       > 55 and <= 65 => "56-65",
                       _ => ">65"
                   };
        }
    }
}