using MySqlConnector;
using UsefulWebApps.Models.Weather;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class LocationsRepository : Repository<Locations>, ILocationsRepository
    {
        public LocationsRepository(MySqlConnection connection) : base(connection) { }
        //any LocationJSON model specific database methods here
    }
}
