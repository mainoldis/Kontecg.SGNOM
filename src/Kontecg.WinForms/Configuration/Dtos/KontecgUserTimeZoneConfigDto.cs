namespace Kontecg.Configuration.Dtos
{
    public class KontecgUserTimeZoneConfigDto
    {
        public KontecgUserWindowsTimeZoneConfigDto Windows { get; set; }

        public KontecgUserIanaTimeZoneConfigDto Iana { get; set; }
    }
}
