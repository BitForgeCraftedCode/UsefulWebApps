using UsefulWebApps.Models.Calendar;
using UsefulWebApps.Models.ViewModels.Calendar;

namespace UsefulWebApps.Helpers.Calendar
{
    public interface ICalendarDisplayService
    {
        CalendarMonthVM BuildCalendarMonth(DateTime firstOfMonth);
        Task LoadEventsForMonth(CalendarMonthVM vm, string? userId);
        void ExpandAndAttachEvents(CalendarMonthVM vm, List<CalendarEvents> events, DateTime rangeStart, DateTime rangeEnd);
        void AttachSingleEvent(CalendarMonthVM vm, CalendarEvents ev);
        void AttachRecurringEvent(CalendarMonthVM vm, CalendarEvents ev, DateTime rangeStart, DateTime rangeEnd);
    }
}
