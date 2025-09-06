namespace Kontecg.Timing.Dto
{
    public class CalendarTimeDecorationDto
    {
        public string Decorator { get; set; }

        public WorkingHoursDto Range { get; set; }

        public int Order { get; set; }
    }
}