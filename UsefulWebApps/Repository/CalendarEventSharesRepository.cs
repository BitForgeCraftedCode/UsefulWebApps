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
    }
}
