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
                (bool Success, string? FilePathDb, string? ErrorMessage) result = await ProcessAndSaveImageAsync(vm.ImageFile, vm.UserProfile.UserId);
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
        private async Task<(bool Success, string? FilePathDb, string? ErrorMessage)> ProcessAndSaveImageAsync(IFormFile imageFile, string userId)
        {
            // max image size 10MB
            const int maxFileSize = 10 * 1024 * 1024;

            if (imageFile.Length == 0 ||
                imageFile.Length > maxFileSize ||
                !imageFile.ContentType.StartsWith("image/"))
            {
                return (false, null, "Invalid image file. Please try again.");
            }
            // Determine format from content type
            /*
             * switch expression
             * The cast (IImageEncoder) on the first arm is needed because the compiler infers the tuple type from all arms together. 
             * Since PngEncoder, GifEncoder, etc. are all different types, the compiler needs a hint that they should all be treated 
             * as their shared interface IImageEncoder. Once the first arm establishes that type, the rest don't need the explicit cast.
             */

            (string extension, IImageEncoder encoder) = imageFile.ContentType switch
            {
                "image/png" => ("png", (IImageEncoder)new PngEncoder()),
                "image/gif" => ("gif", new GifEncoder()),
                "image/webp" => ("webp", new WebpEncoder { Quality = 75 }),
                _ => ("jpg", new JpegEncoder { Quality = 75 }) // default jpeg for jpg, bmp, tiff, etc
            };

            // generate unique file name
            string fileName = $"{Guid.NewGuid()}.{extension}";
            string directory = Path.Combine(this._environment.WebRootPath, $"images/users/{userId}/");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            //get filepath for physical storage location
            string storageFilePath = Path.Combine(directory, fileName);
            //get filepath for database
            string filePathDb = $"/images/users/{userId}/{fileName}";
            try
            {
                //resize the image then save to storage location
                using (Image image = await Image.LoadAsync(imageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.AutoOrient());
                    if (image.Width > 256)
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(256, 0), // 0 for height auto
                            Mode = ResizeMode.Max
                        }));
                    }
                    //upload image -- copy/save image to wwwroot
                    await image.SaveAsync(storageFilePath, encoder);
                }
            }
            catch
            {
                return (false, null, "Invalid or corrupted image. Please try again.");
            }

            return (true, filePathDb, null);
        }
    }
}
