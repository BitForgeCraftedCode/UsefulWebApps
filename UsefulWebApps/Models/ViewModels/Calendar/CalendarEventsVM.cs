using UsefulWebApps.Models.Calendar;

namespace UsefulWebApps.Models.ViewModels.Calendar
{
    public class CalendarEventsVM
    {
        public CalendarEvents Event { get; set; }

        public bool IsPrivateEvent { get; set; }

        // Recurrence
        public bool IsRecurring { get; set; }

        public string? Frequency { get; set; }
        // Daily, Weekly, Monthly, Yearly

        public int Interval { get; set; } = 1;

        public string? DaysOfWeek { get; set; }

        public DateTime? RecurrenceEndDate { get; set; }

    }
}
