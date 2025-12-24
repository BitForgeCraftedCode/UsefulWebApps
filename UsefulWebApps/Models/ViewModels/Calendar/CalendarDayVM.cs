using UsefulWebApps.Models.Calendar;

namespace UsefulWebApps.Models.ViewModels.Calendar
{
    public class CalendarDayVM
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }

        public List<CalendarEvents> Events { get; set; } = new();
    }
}
