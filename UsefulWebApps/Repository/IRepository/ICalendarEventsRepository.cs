using UsefulWebApps.Models.Calendar;

namespace UsefulWebApps.Repository.IRepository
{
    public interface ICalendarEventsRepository : IRepository<CalendarEvents>
    {
        //any CalendarEvents model specific database methods here
        Task<List<CalendarEvents>> GetUserCalendarEventsForDateRange(DateTime startDate, DateTime endDate, string userId);
    }
}
