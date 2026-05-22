using Ical.Net.DataTypes;
using UsefulWebApps.Models.Calendar;
using UsefulWebApps.Models.ViewModels.Calendar;

namespace UsefulWebApps.Helpers.Calendar
{
    public interface ICalendarRecurrenceService
    {
        RecurrencePattern GetRecurrencePattern(CalendarEvents calendarEvent);
        string GetDayOfWeekFromPattern(RecurrencePattern pattern);
        string GetFrequencyFromPattern(RecurrencePattern pattern);
        string BuildRRule(CalendarEventsVM vm, DateTime startDate);
    }
}
