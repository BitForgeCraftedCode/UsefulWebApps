using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsefulWebApps.IdentityModels;
using UsefulWebApps.Models.Friends;
using UsefulWebApps.Models.ViewModels.Friends;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class FriendsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public FriendsController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            (List<UserProfiles> profiles, List<Friendships> friendships) result = await _unitOfWork.Friendships.GetFriendsWithProfiles(userId);
            FriendsVM vm = new()
            {
                UserProfiles = result.profiles,
                Friendships = result.friendships
            };
            return View(vm);
        }

        public async Task<IActionResult> People()
        {
            List<UserProfiles> userProfiles =  (List<UserProfiles>)await _unitOfWork.UserProfiles.GetAll();
            UserProfilesVM userProfilesVM = new()
            {
                UserProfiles = userProfiles,
            };
            return View(userProfilesVM);
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(UserProfiles userProfile)
        {
            if (!ModelState.IsValid) 
            {
                TempData["error"] = "Friend request error.";
                return RedirectToAction("People");
            }
            ClaimsPrincipal currentUser = this.User;
            string? requesterId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requesterId))
            {
                TempData["error"] = "Unable to identify current user.";
                return RedirectToAction("People");
            }
            // Prevent sending a friend request to yourself
            if (requesterId == userProfile.UserId)
            {
                TempData["error"] = "You cannot send a friend request to yourself.";
                return RedirectToAction("People");
            }
            // Check if a friendship/request already exists in either direction
            Friendships? existing = await _unitOfWork.Friendships.GetExisting(requesterId, userProfile.UserId);
            if (existing != null)
            {
                if (existing.Status == FriendshipStatus.Accepted)
                {
                    TempData["error"] = "You are already friends with this user.";
                    return RedirectToAction("People");
                }
                if (existing.Status == FriendshipStatus.Pending)
                {
                    TempData["error"] = "A friend request already exists with this user.";
                    return RedirectToAction("People");
                }
                if (existing.Status == FriendshipStatus.Declined)
                {
                    bool reRequest = await _unitOfWork.Friendships.UpdateStatus(existing.Id, FriendshipStatus.Pending);
                    TempData[reRequest ? "success" : "error"] = reRequest
                        ? "Friend request sent!"
                        : "Could not send friend request. Please try again.";
                    return RedirectToAction("People");
                }
            }
            Friendships friendship = new()
            {
                RequesterUserId = requesterId,
                AddresseeUserId = userProfile.UserId,
                Status = FriendshipStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            bool success = await _unitOfWork.Friendships.Add(friendship);

            TempData[success ? "success" : "error"] = success
                ? "Friend request sent!"
                : "Could not send friend request. Please try again.";

            return RedirectToAction("People");
        }

        public async Task<IActionResult> Requests()
        {
            ClaimsPrincipal currentUser = this.User;
            string? addresseeUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(addresseeUserId)) return NotFound();

            (List<UserProfiles> profiles, List<Friendships> friendships) result = await _unitOfWork.Friendships.GetPendingRequestsWithProfiles(addresseeUserId);

            RequestsVM vm = new() 
            { 
                UserProfiles = result.profiles,
                PendingRequests = result.friendships
            };
            return View(vm);
            
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(ulong friendshipId)
        {
            bool success = await _unitOfWork.Friendships.UpdateStatus(friendshipId, FriendshipStatus.Accepted);
            TempData[success ? "success" : "error"] = success ? "Friend request accepted!" : "Something went wrong.";
            return RedirectToAction("Requests");
        }

        [HttpPost]
        public async Task<IActionResult> DeclineFriendRequest(ulong friendshipId)
        {
            bool success = await _unitOfWork.Friendships.UpdateStatus(friendshipId, FriendshipStatus.Declined);
            TempData[success ? "success" : "error"] = success ? "Friend request declined." : "Something went wrong.";
            return RedirectToAction("Requests");
        }

        public async Task<IActionResult> MyProfile()
        {
            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();
            UserProfiles userProfile = await _unitOfWork.UserProfiles.GetByUserId(userId);

            return View(userProfile);
        }

        public async Task<IActionResult> EditMyProfile()
        {
            return View();
        }
    }
}
