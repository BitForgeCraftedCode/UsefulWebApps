using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;

namespace UsefulWebApps.Repository
{
    public class NotesRepository : Repository<Notes>, INotesRepository
    {
        public NotesRepository(MySqlConnection connection) : base(connection) { }
        //any Notes model specific database methods here
    }
}
