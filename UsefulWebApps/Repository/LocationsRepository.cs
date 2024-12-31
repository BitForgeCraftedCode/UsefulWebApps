using MySqlConnector;
using UsefulWebApps.Models.Weather;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class LocationsRepository : Repository<Locations>, ILocationsRepository
    {
        private readonly MySqlConnection _connection;
        public LocationsRepository(MySqlConnection db) : base(db) 
        {
            _connection = db;
        }
        //any LocationJSON model specific database methods here
    }
}
