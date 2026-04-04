using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            return View();
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

        public async Task<IActionResult> Requests()
        {
            return View();
        }

        public async Task<IActionResult> MyProfile()
        {
            return View();
        }

        public async Task<IActionResult> EditMyProfile()
        {
            return View();
        }
    }
}
