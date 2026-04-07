using UsefulWebApps.Models.Friends;

namespace UsefulWebApps.Models.ViewModels.Friends
{
    public class RequestsVM
    {
        public List<UserProfiles> UserProfiles { get; set; }
        public List<Friendships> PendingRequests { get; set; }
    }
}
