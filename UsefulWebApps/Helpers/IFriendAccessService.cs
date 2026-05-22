using Microsoft.AspNetCore.Mvc.Rendering;

namespace UsefulWebApps.Helpers
{
    public interface IFriendAccessService
    {
        Task<IEnumerable<SelectListItem>> GetFriendsSelectListAsync(string userId);
        Task<bool> AreFriendsAsync(string userId, string otherUserId);
    }
}
