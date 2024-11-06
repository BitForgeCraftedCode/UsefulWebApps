using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsefulWebApps.IdentityModels;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        //only used to clear user account data
        private readonly IUnitOfWork _unitOfWork;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;

            _unitOfWork = unitOfWork;
        }

        public IActionResult Index() { return View(); }

        public IActionResult AccessDenied() { return View(); }

        [Authorize(Roles = "Admin")]
        public IActionResult Register() { return View(); }

        //register a StandardUser
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Register(Register userRegInfo)
        {
            if (!ModelState.IsValid) { return View(); }
          
            IdentityUser user = new IdentityUser
            {
                Email = userRegInfo.Email.Trim(),
                UserName = userRegInfo.UserName.Trim(),
            };
            IdentityResult result = await _userManager.CreateAsync(user, userRegInfo.Password.Trim());
            if (result.Succeeded)
            {

                if (!await _roleManager.RoleExistsAsync("StandardUser"))
                {
                    IdentityRole standerUserRole = new IdentityRole("StandardUser");
                    await _roleManager.CreateAsync(standerUserRole);
                }
                await _userManager.AddToRoleAsync(user, "StandardUser");
                TempData["success"] = "User registered successfully";
                return RedirectToAction("Manage", "Account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                TempData["error"] = "Register user error. Please try again.";
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RegisterAdmin() { return View(); }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(Register userRegInfo)
        {
            if (!ModelState.IsValid) { return View(); }

            IdentityUser user = new IdentityUser
            {
                Email = userRegInfo.Email.Trim(),
                UserName = userRegInfo.UserName.Trim()
            };
            IdentityResult result = await _userManager.CreateAsync(user, userRegInfo.Password.Trim());
            if (result.Succeeded)
            {

                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    IdentityRole adminUserRole = new IdentityRole("Admin");
                    await _roleManager.CreateAsync(adminUserRole);
                }
                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["success"] = "Admin user registered successfully";
                return RedirectToAction("Manage", "Account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                TempData["error"] = "Register admin user error. Please try again.";
                return View();
            }
        }

        public IActionResult Login() { return View(); }

        [HttpPost]
        public async Task<IActionResult> Login(Credential userLoginInfo)
        {
            if (!ModelState.IsValid) { return View(); }

            var result = await _signInManager.PasswordSignInAsync(
                    userLoginInfo.UserName.Trim(),
                    userLoginInfo.Password.Trim(),
                    userLoginInfo.RememberMe,
                    false
                );
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to login");
                }
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ChangePassword() { return View(); }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(Register userInfo)
        {
            if (!ModelState.IsValid) { return View(); }

            IdentityUser user = await _userManager.FindByEmailAsync(userInfo.Email.Trim());
            if (user == null) 
            {
                TempData["error"] = "Change password error. Please try again.";
                return View(); 
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(token)) 
            {
                TempData["error"] = "Change password error. Please try again.";
                return View(); 
            }
            await _userManager.ResetPasswordAsync(user, token, userInfo.Password.Trim());
            TempData["success"] = "Password changed successfully";
            return RedirectToAction("Manage", "Account");

        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser() { return View(); }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(RemoveUser userInfo)
        {
            if (!ModelState.IsValid) { return View(); }
            IdentityUser user = await _userManager.FindByEmailAsync(userInfo.Email.Trim());
            if (user == null) 
            {
                TempData["error"] = "Delete user error. Please try again.";
                return View(); 
            };
            await _userManager.DeleteAsync(user);
            TempData["success"] = "Deleted user successfully";
            return RedirectToAction("Manage", "Account");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Manage() { return View(); }


        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUserData() { return View(); }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteUserData(DeleteUserData userInfo)
        {
            if (!ModelState.IsValid) { return View(); }
            IdentityUser user = await _userManager.FindByEmailAsync(userInfo.Email.Trim());
            IdentityUser admin = await _userManager.FindByEmailAsync(userInfo.AdminEmail.Trim());
            if (user == null || admin == null) 
            {
                TempData["error"] = "User data clean up error. Please try again.";
                return View(); 
            };
            bool success = await _unitOfWork.ManageAccountData.DeleteUserData(user, admin);
            if (success) 
            {
                TempData["success"] = "User data cleaned up successfully";
                return RedirectToAction("Manage", "Account");
            }
            else
            {
                TempData["error"] = "User data clean up error. Please try again.";
                return View();
            }
        }
    }
}
