using MySqlConnector;
using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class SlideShowRepository : Repository<SlideShowImages>, ISlideShowRepository
    {
        private readonly MySqlConnection _connection;

        public SlideShowRepository(MySqlConnection db) : base(db) 
        {
            _connection = db;
        }
        //any SlideShow specific database methods here
    }
}
