using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class NotesRepository : Repository<Notes>, INotesRepository
    {
        public NotesRepository(MySqlConnection connection) : base(connection) { }
        //any Notes model specific database methods here
        
        //concurrent edits 
        public async Task<(bool Success, bool WasConflict)> UpdateWithVersionCheck(Notes entity)
        {
            string sql = @"UPDATE notes 
                   SET Note = @Note, 
                       NoteTitle = @NoteTitle,
                       Version = Version + 1
                   WHERE Id = @Id 
                   AND UserId = @UserId
                   AND Version = @Version";

            int rows = await _connection.ExecuteAsync(sql, entity);

            if (rows > 0) return (true, false);

            // 0 rows could mean conflict OR note doesn't exist — check which
            Notes? current = await _connection.QuerySingleOrDefaultAsync<Notes>(
                "SELECT * FROM notes WHERE Id = @Id", new { entity.Id });

            bool wasConflict = current != null; // exists but version mismatch = conflict
            return (false, wasConflict);
        }
    }
}
