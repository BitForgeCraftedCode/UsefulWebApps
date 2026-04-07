using UsefulWebApps.Models.Friends;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IFriendshipsRepository : IRepository<Friendships>
    {
        //any Friendships specific database methods here
        Task<Friendships?> GetExisting(string userId1, string userId2);
        Task<(List<UserProfiles> profiles, List<Friendships> friendships)> GetPendingRequestsWithProfiles(string addresseeUserId);
        Task<bool> UpdateStatus(ulong friendshipId, FriendshipStatus status);
    }
}
