using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.Calendar;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class CalendarEventSharesRepository : Repository<CalendarEventShares>, ICalendarEventSharesRepository
    {
        public CalendarEventSharesRepository(MySqlConnection connection) : base(connection) { }
        //any CalendarEventShares model specific database methods here
        public async Task<bool> ShareCalendarEvent(long eventId, string sharedWithUserId)
        {
            //IGNORE so it will not throw duplicate key error if trying to share same note with same user twice
            string sql = @"INSERT IGNORE INTO calendar_event_shares (CalendarEventId, SharedWithUserId) 
                           VALUES (@eventId, @sharedWithUserId)";
            int rows = await _connection.ExecuteAsync(sql, new { eventId, sharedWithUserId });
            return rows > 0;
        }

        public async Task<List<CalendarEvents>> GetCalendarEventsSharedWithUserForDateRange(DateTime startDate, DateTime endDate, string userId)
        {
            string sql = @"
                SELECT e.* FROM calendar_events e
                INNER JOIN calendar_event_shares es ON es.CalendarEventId = e.Id
                WHERE es.SharedWithUserId = @userId
                AND(RRule IS NOT NULL OR (StartDate < @endDate AND EndDate >= @startDate))
            ";
            List<CalendarEvents> calendarEvents = (await _connection.QueryAsync<CalendarEvents>(sql, new
            {
                startDate,
                endDate,
                userId
            })).ToList();
            return calendarEvents;
        }

        public async Task<bool> UnshareCalendarEvent(long eventId)
        {
            string sql = "DELETE FROM calendar_event_shares WHERE CalendarEventId = @eventId";
            int rows = await _connection.ExecuteAsync(sql, new { eventId });
            return rows > 0;
        }
        public async Task<List<string>> GetSharedFriendNamesForEvent(long eventId, string ownerUserId)
        {
            /* gets the names of all users the event is shared with, but only if the event belongs to ownerUserId
             * 
             * Find everyone this calendar event was shared with, verify the event belongs to the specified owner, then return the shared users' display names sorted alphabetically.
             */
            string sql = @"SELECT COALESCE(up.DisplayName, 'Unknown') AS DisplayName
                           FROM calendar_event_shares ces
                           INNER JOIN calendar_events ce ON ce.Id = ces.CalendarEventId
                           LEFT JOIN user_profiles up ON up.UserId = ces.SharedWithUserId
                           WHERE ces.CalendarEventId = @eventId
                           AND ce.UserId = @ownerUserId
                           ORDER BY DisplayName";

            List<string> names = (await _connection.QueryAsync<string>(sql, new { eventId, ownerUserId })).ToList();
            return names;
        }
    }
}
