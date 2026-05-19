using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Security.Claims;
using UsefulWebApps.Helpers;
using UsefulWebApps.IdentityModels;
using UsefulWebApps.Models.Friends;
using UsefulWebApps.Models.MyRecipes;
using UsefulWebApps.Models.ViewModels.Friends;
using UsefulWebApps.Models.ViewModels.MyRecipes;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class FriendsController : Controller
    {
        private IWebHostEnvironment _environment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public FriendsController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            string? userId = User.GetUserId();
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
            List<UserProfiles> userProfiles =  (await _unitOfWork.UserProfiles.GetAll()).ToList();
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

            string? requesterId = User.GetUserId();

            if (string.IsNullOrEmpty(requesterId))
            {
                TempData["error"] = "Unable to identify current user.";
                return RedirectToAction("People");
            }
            // Prevent sending a friend request to yourself
            if (requesterId == userProfile.UserId)
            {
                TempData["warning"] = "You cannot send a friend request to yourself.";
                return RedirectToAction("People");
            }
            // Check if a friendship/request already exists in either direction
            Friendships? existing = await _unitOfWork.Friendships.GetExisting(requesterId, userProfile.UserId);
            if (existing != null)
            {
                if (existing.Status == FriendshipStatus.Accepted)
                {
                    TempData["warning"] = "You are already friends with this user.";
                    return RedirectToAction("People");
                }
                if (existing.Status == FriendshipStatus.Pending)
                {
                    TempData["warning"] = "A friend request already exists with this user.";
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
            };

            bool success = await _unitOfWork.Friendships.Add(friendship);

            TempData[success ? "success" : "error"] = success
                ? "Friend request sent!"
                : "Could not send friend request. Please try again.";

            return RedirectToAction("People");
        }

        public async Task<IActionResult> Requests()
        {
            string? addresseeUserId = User.GetUserId();
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
        public async Task<IActionResult> AcceptFriendRequest(long friendshipId)
        {
            bool success = await _unitOfWork.Friendships.UpdateStatus(friendshipId, FriendshipStatus.Accepted);
            TempData[success ? "success" : "error"] = success ? "Friend request accepted!" : "Something went wrong.";
            return RedirectToAction("Requests");
        }

        [HttpPost]
        public async Task<IActionResult> DeclineFriendRequest(long friendshipId)
        {
            bool success = await _unitOfWork.Friendships.UpdateStatus(friendshipId, FriendshipStatus.Declined);
            TempData[success ? "success" : "error"] = success ? "Friend request declined." : "Something went wrong.";
            return RedirectToAction("Requests");
        }

        public async Task<IActionResult> MyProfile()
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();
            UserProfiles userProfile = await _unitOfWork.UserProfiles.GetByUserId(userId);

            return View(userProfile);
        }

        public async Task<IActionResult> EditMyProfile(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            UserProfiles userProfile = await _unitOfWork.UserProfiles.GetByUserId(id);
            EditUserProfileVM vm = new() 
            { 
                UserProfile = userProfile,
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> EditMyProfile(EditUserProfileVM vm)
        {
            if(vm.ImageFile == null)
            {
                TempData["error"] = "Select a new user profile picture.";
                return View(vm);
            }
            if (ModelState.IsValid)
            {
                string? oldFilePathDb = vm.UserProfile.AvatarPath;

                (bool Success, string? FilePathDb, string? ErrorMessage) result = await ImageUpload.ProcessAndSaveImageAsync(
                    vm.ImageFile,
                    this._environment.WebRootPath, 
                    $"images/users/{vm.UserProfile.UserId}", 
                    maxWidth: 256, 
                    preserveOriginalFormat: true
                );

                if (!result.Success)
                {
                    TempData["error"] = $"Update user profile error. {result.ErrorMessage}";
                    return View(vm);
                }
                // delete old image AFTER new one succeeds
                if (!string.IsNullOrEmpty(oldFilePathDb))
                {
                    string oldImageStoragePath = Path.Combine(this._environment.WebRootPath, oldFilePathDb.TrimStart('/'));
                    if (System.IO.File.Exists(oldImageStoragePath))
                    {
                        System.IO.File.Delete(oldImageStoragePath);
                    }
                }
                vm.UserProfile.AvatarPath = result.FilePathDb;
                bool success = await _unitOfWork.UserProfiles.Update(vm.UserProfile);
                if (success)
                {
                    TempData["success"] = "User profile updated successfully";
                    return RedirectToAction("MyProfile");
                }
                else
                {
                    TempData["error"] = "Update user profile error. Please try again.";
                    return View(vm);
                }
            }
            TempData["error"] = "Update user profile error. Please try again.";
            return View(vm);
        }
        
    }
}
