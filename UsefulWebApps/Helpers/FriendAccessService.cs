using Microsoft.AspNetCore.Mvc.Rendering;
using UsefulWebApps.Models.Friends;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Helpers
{
    public class FriendAccessService : IFriendAccessService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FriendAccessService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SelectListItem>> GetFriendsSelectListAsync(string userId)
        {
            // Get friends to populate dropdown
            (List<UserProfiles> profiles, List<Friendships> friendships) result = await _unitOfWork.Friendships.GetFriendsWithProfiles(userId);
            return result.profiles.Select(p => new SelectListItem
            {
                //?? null-coalescing operator
                Text = p.DisplayName ?? p.UserId,
                Value = p.UserId
            });
        }

        public async Task<bool> AreFriendsAsync(string userId, string otherUserId)
        {
            Friendships? friendship = await _unitOfWork.Friendships.GetExisting(userId, otherUserId);
            return friendship != null && friendship.Status == FriendshipStatus.Accepted;
        }
    }
}
