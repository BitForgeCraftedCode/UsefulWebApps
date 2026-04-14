using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface INoteSharesRepository : IRepository<NoteShares>
    {
        //any NoteShares model specific database methods here
        Task<bool> ShareNote(long noteId, string sharedWithUserId);
        Task<bool> UnshareNote(long noteId);
        Task<List<Notes>> GetNotesSharedWithUser(string userId);
        // Returns noteId -> list of friend DisplayNames this note is shared to
        Task<Dictionary<long, List<string>>> GetSharedToMapForOwner(string ownerUserId);
    }
}
