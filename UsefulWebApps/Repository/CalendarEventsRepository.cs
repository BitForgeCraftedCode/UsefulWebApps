using UsefulWebApps.Models.Calendar;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;

namespace UsefulWebApps.Repository
{
    public class CalendarEventsRepository : Repository<CalendarEvents>, ICalendarEventsRepository
    {
        private readonly MySqlConnection _connection;

        public CalendarEventsRepository(MySqlConnection db) : base(db) 
        { 
            _connection = db;
        }
        //any CalendarEvents model specific database methods here
    }
}
