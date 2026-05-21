using UsefulWebApps.Models.Calendar;

namespace UsefulWebApps.Repository.IRepository
{
    public interface ICalendarEventSharesRepository : IRepository<CalendarEventShares>
    {
        //any CalendarEventShares model specific database methods here
        Task<bool> ShareCalendarEvent(long eventId, string sharedWithUserId);
        Task<List<CalendarEvents>> GetCalendarEventsSharedWithUserForDateRange(DateTime startDate, DateTime endDate, string userId);
        Task<bool> UnshareCalendarEvent(long eventId);
    }
}
