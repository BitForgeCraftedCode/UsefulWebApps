using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class NoteSharesRepository : Repository<NoteShares>, INoteSharesRepository
    {
        public NoteSharesRepository(MySqlConnection connection) : base(connection) { }
        //any NoteShares model specific database methods here
        public async Task<bool> ShareNote(long noteId, string sharedWithUserId)
        {
            //IGNORE so it will not throw duplicate key error if trying to share same note with same user twice
            string sql = @"INSERT IGNORE INTO note_shares (NoteId, SharedWithUserId) 
                           VALUES (@noteId, @sharedWithUserId)";
            int rows = await _connection.ExecuteAsync(sql, new { noteId, sharedWithUserId });
            return rows > 0;
        }

        public async Task<bool> UnshareNote(long noteId)
        {
            string sql = "DELETE FROM note_shares WHERE NoteId = @noteId";
            int rows = await _connection.ExecuteAsync(sql, new { noteId });
            return rows > 0;
        }

        public async Task<List<Notes>> GetNotesSharedWithUser(string userId)
        {
            string sql = @"SELECT n.* FROM notes n
                           INNER JOIN note_shares ns ON ns.NoteId = n.Id
                           WHERE ns.SharedWithUserId = @userId";
            List<Notes> notes = (await _connection.QueryAsync<Notes>(sql, new { userId })).ToList();
            return notes;
        }
        // Key: NoteId, Value: list of display names the note is shared with
        public async Task<Dictionary<long, List<string>>> GetSharedToMapForOwner(string ownerUserId)
        {
            // For all notes owned by this user that have been shared, return noteId -> friend display names
            // (NoteId, DisplayName of the user the note is shared with)
            string sql = @"SELECT ns.NoteId, COALESCE(up.DisplayName, 'Unknown') AS DisplayName
                           FROM note_shares ns
                           INNER JOIN notes n ON n.Id = ns.NoteId
                           INNER JOIN user_profiles up ON up.UserId = ns.SharedWithUserId
                           WHERE n.UserId = @ownerUserId";

            var rows = await _connection.QueryAsync<(long NoteId, string DisplayName)>(sql, new { ownerUserId });

            Dictionary<long, List<string>> map = new();
            foreach (var row in rows)
            {
                //if map does not contain key create it
                if (!map.ContainsKey(row.NoteId))
                    map[row.NoteId] = new List<string>();
                //add display name to the correct key
                map[row.NoteId].Add(row.DisplayName);
            }
            return map;
        }
    }
}
