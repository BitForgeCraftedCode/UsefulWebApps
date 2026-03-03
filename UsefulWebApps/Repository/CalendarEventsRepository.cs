using UsefulWebApps.Models.Calendar;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;

namespace UsefulWebApps.Repository
{
    public class CalendarEventsRepository : Repository<CalendarEvents>, ICalendarEventsRepository
    {
        public CalendarEventsRepository(MySqlConnection connection) : base(connection) { }
        //any CalendarEvents model specific database methods here
        public async Task<List<CalendarEvents>> GetUserCalendarEventsForDateRange(DateTime startDate, DateTime endDate, string userId)
        {
            string sql = @$"
                SELECT * 
                FROM calendar_events 
                WHERE(UserId = @userId OR UserId IS NULL)
                AND(RRule IS NOT NULL OR (StartDate < @endDate AND EndDate >= @startDate)) 
            ";

            List<CalendarEvents> calendarEvents = (List<CalendarEvents>)await _connection.QueryAsync<CalendarEvents>(sql, new 
            { 
                startDate,
                endDate,
                userId
            });
            return calendarEvents;
        }
    }
}
