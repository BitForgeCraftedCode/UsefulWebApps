using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;

namespace UsefulWebApps.Repository
{
    public class NotesRepository : Repository<Notes>, INotesRepository
    {
        private readonly MySqlConnection _connection;

        public NotesRepository(MySqlConnection db) : base(db) 
        {
            _connection = db;
        }
        //any Notes model specific database methods here
    }
}
