using Microsoft.AspNetCore.Mvc;
using UsefulWebApps.Helpers;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationBellViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string? userId = UserClaimsPrincipal?.GetUserId();
            int unreadCount = 0;

            if (!string.IsNullOrEmpty(userId))
            {
                unreadCount = await _unitOfWork.Notifications.GetUnreadCount(userId);
            }

            return View(unreadCount);
        }
    }
}
