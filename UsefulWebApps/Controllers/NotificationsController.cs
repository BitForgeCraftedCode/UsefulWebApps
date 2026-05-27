using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsefulWebApps.Helpers;
using UsefulWebApps.Models.Notifications;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class NotificationsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            List<Notifications> notifications = (await _unitOfWork.Notifications.GetAllWhere("UserId", userId))
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            bool success = await _unitOfWork.Notifications.DeleteAllWhere("UserId", userId);
            if (success)
            {
                TempData["success"] = "Notifications deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Delete notifications error. Try again.";
            return RedirectToAction("Index");
        }
    }
}
