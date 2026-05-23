using Ganss.Xss;
using Ical.Net;
using Ical.Net.DataTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UsefulWebApps.Helpers;
using UsefulWebApps.Helpers.Calendar;
using UsefulWebApps.Models.Calendar;
using UsefulWebApps.Models.ViewModels.Calendar;
using UsefulWebApps.Repository.IRepository;
using Microsoft.AspNetCore.SignalR;
using UsefulWebApps.Hubs;

namespace UsefulWebApps.Controllers
{
    /*
     * The Goal of it from a UI perspective is this. 
     * Allow users to make events. 
     * Public events are seen by everyone --- only editable by owner or admin. 
     * Private events are only seen by the user who made them unless that user shared the private event with friends -- private events only editable by owner or admin. 
     * Once a private event is shared you cannot make it public until you unshare it. 
     * Shared private events can only be edited by the owner or admin. 
     * 
     */
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class CalendarController : Controller
    {
        private HtmlSanitizer sanitizer = new HtmlSanitizer();
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFriendAccessService _friendAccessService;
        private readonly ICalendarRecurrenceService _calendarRecurrenceService;
        private readonly ICalendarDisplayService _calendarDisplayService;
        private readonly IHubContext<AppHub> _hubContext;

        public CalendarController(IUnitOfWork unitOfWork, IFriendAccessService friendAccessService, ICalendarRecurrenceService calendarRecurrenceService, ICalendarDisplayService calendarDisplayService, IHubContext<AppHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _friendAccessService = friendAccessService;
            _calendarRecurrenceService = calendarRecurrenceService;
            _calendarDisplayService = calendarDisplayService;
            _hubContext = hubContext;
        }
        
        public async Task<IActionResult> Index(int? year, int? month)
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            DateTime firstOfMonth = new DateTime(
                year ?? DateTime.Today.Year,
                month ?? DateTime.Today.Month,
                1);

            CalendarMonthVM vm = _calendarDisplayService.BuildCalendarMonth(firstOfMonth);
            await _calendarDisplayService.LoadEventsForMonth(vm, userId);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UnshareEvent(long eventId)
        {
            string? userId = User.GetUserId();
            string? role = User.GetUserRole();
            if (string.IsNullOrEmpty(userId)) return NotFound();
            if (string.IsNullOrEmpty(role)) return NotFound();

            CalendarEvents calendarEvent = await _unitOfWork.CalendarEvents.GetById(eventId);
            if (calendarEvent.UserId != userId && role != "Admin")
            {
                TempData["warning"] = "You can only manage sharing on your own events.";
                return RedirectToAction("EditEvent", new { id = eventId });
            }

            bool success = await _unitOfWork.CalendarEventShares.UnshareCalendarEvent(eventId);
            TempData[success ? "success" : "error"] = success
                ? "Event unshared."
                : "Could not unshare event. It may not have been shared with anyone.";

            return RedirectToAction("EditEvent", new { id = eventId });
        }
        public async Task<IActionResult> ShareEvent(long? id)
        {
            if (id == null || id == 0) return NotFound();

            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            CalendarEvents calendarEvent = await _unitOfWork.CalendarEvents.GetById(id);

            if(calendarEvent.IsPrivate == false)
            {
                TempData["warning"] = "You can only share private events.";
                return RedirectToAction("EditEvent", new { id });
            }
            // Only the owner can share their note
            if (calendarEvent.UserId != userId)
            {
                TempData["warning"] = "You can only share your own events.";
                return RedirectToAction("Index");
            }
            IEnumerable<SelectListItem> friendsList = await _friendAccessService.GetFriendsSelectListAsync(userId);
            ShareEventVM vm = new()
            {
                EventId = calendarEvent.Id,
                FriendsList = friendsList
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ShareEvent(ShareEventVM vm)
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            if (!await _friendAccessService.AreFriendsAsync(userId, vm.SelectedFriendUserId))
            {
                TempData["warning"] = "You can only share events with friends.";
                return RedirectToAction("EditEvent", new { id = vm.EventId });
            }
            //verify the event belongs to the current user
            CalendarEvents calendarEvent = await _unitOfWork.CalendarEvents.GetById(vm.EventId);
            if (calendarEvent.UserId != userId)
            {
                TempData["warning"] = "You can only share your own events.";
                return RedirectToAction("Index");
            }
            if (!calendarEvent.IsPrivate)
            {
                TempData["warning"] = "You can only share private events.";
                return RedirectToAction("EditEvent", new { id = vm.EventId });
            }
            bool success = await _unitOfWork.CalendarEventShares.ShareCalendarEvent(vm.EventId, vm.SelectedFriendUserId);
            if (success)
            {
                await _hubContext.Clients.User(vm.SelectedFriendUserId)
                    .SendAsync("ReceiveNotification",
                        $"{User.Identity?.Name} shared calendar event '{calendarEvent.Title}' with you.");
            }
            TempData[success ? "success" : "error"] = success
                ? "Event shared successfully."
                : "Could not share event. It may already be shared with this friend.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvent(long? id)
        {
            string? userId = User.GetUserId();
            string? role = User.GetUserRole();
            if (string.IsNullOrEmpty(userId)) return NotFound();
            if (string.IsNullOrEmpty(role)) return NotFound();

            if (id == null || id == 0)
            {
                return NotFound();
            }
            CalendarEvents calendarEvent = await _unitOfWork.CalendarEvents.GetById(id);
            if (calendarEvent.Id == 0)
            {
                return NotFound();
            }
            if (userId != calendarEvent.UserId && role != "Admin")
            {
                TempData["warning"] = "You can only delete events that belong to you";
                return RedirectToAction("Index");
            }
            bool success = await _unitOfWork.CalendarEvents.Delete(id);
            if (success)
            {
                TempData["success"] = "Event deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Delete event error. Try again.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditEvent(long? id)
        {
            string? userId = User.GetUserId();
            string? role = User.GetUserRole();
            if (string.IsNullOrEmpty(userId)) return NotFound();
            if (string.IsNullOrEmpty(role)) return NotFound();

            if (id == null || id == 0)
            {
                return NotFound();
            }
            CalendarEvents calendarEvent = await _unitOfWork.CalendarEvents.GetById(id);
            CalendarEventsVM calendarEventVM;
            if(userId != calendarEvent.UserId && role != "Admin")
            {
                TempData["warning"] = "You can only edit events that belong to you";
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(calendarEvent.RRule))
            {
                calendarEventVM = new()
                {
                    Event = calendarEvent,
                    IsRecurring = string.IsNullOrEmpty(calendarEvent.RRule) ? false : true,
                    SharedWithFriends = await _unitOfWork.CalendarEventShares.GetSharedFriendNamesForEvent(calendarEvent.Id, calendarEvent.UserId)
                };
            }
            else
            {
                RecurrencePattern pattern = _calendarRecurrenceService.GetRecurrencePattern(calendarEvent);
                calendarEventVM = new()
                {
                    Event = calendarEvent,
                    IsRecurring = string.IsNullOrEmpty(calendarEvent.RRule) ? false : true,
                    Frequency = _calendarRecurrenceService.GetFrequencyFromPattern(pattern),
                    Interval = pattern.Interval,
                    DaysOfWeek = pattern.Frequency == FrequencyType.Weekly ? _calendarRecurrenceService.GetDayOfWeekFromPattern(pattern) : string.Empty,
                    RecurrenceEndDate = pattern.Until?.Value,
                    SharedWithFriends = await _unitOfWork.CalendarEventShares.GetSharedFriendNamesForEvent(calendarEvent.Id, calendarEvent.UserId)

                };
            }
            return View(calendarEventVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(CalendarEventsVM vm)
        {
            string? userId = User.GetUserId();
            string? role = User.GetUserRole();
            if (string.IsNullOrEmpty(userId)) return NotFound();
            if (string.IsNullOrEmpty(role)) return NotFound();

            vm.Event.Description = sanitizer.Sanitize(vm.Event.Description);
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Edit event error. Try again.";
                return View(vm);
            }

            // --- logical validation ---
            if (vm.Event.EndDate < vm.Event.StartDate)
            {
                TempData["warning"] = "End date cannot be before start date. Try again.";
                return View(vm);
            }

            CalendarEvents existingEvent = await _unitOfWork.CalendarEvents.GetById(vm.Event.Id);
            if (existingEvent.Id == 0)
            {
                TempData["error"] = "Edit event error. Try again.";
                return RedirectToAction("Index");
            }
            if (userId != existingEvent.UserId && role != "Admin")
            {
                TempData["warning"] = "You can only edit events that belong to you";
                return RedirectToAction("Index");
            }
            if (existingEvent.IsPrivate != vm.Event.IsPrivate && await _unitOfWork.CalendarEventShares.HasShares(vm.Event.Id))
            {
                TempData["warning"] = "You cannot change privacy while this event is shared. Unshare it first.";
                return RedirectToAction("EditEvent", new { id = vm.Event.Id });
            }

            //create CalendarEvents object
            CalendarEvents calEvent = new CalendarEvents
            {
                Id = vm.Event.Id,
                UserId = existingEvent.UserId,
                Title = vm.Event.Title,
                Description = vm.Event.Description,
                StartDate = vm.Event.StartDate,
                EndDate = vm.Event.EndDate,
                IsAllDay = vm.Event.IsAllDay,
                IsPrivate = vm.Event.IsPrivate,

            };

            // --- recurrence handling ---
            if (vm.IsRecurring)
            {
                calEvent.RRule = _calendarRecurrenceService.BuildRRule(vm, calEvent.StartDate);
            }
            else
            {
                calEvent.RRule = null;
            }

            bool success = await _unitOfWork.CalendarEvents.Update(calEvent);
            if (success)
            {
                TempData["success"] = "Event edited successfully.";
            }
            else
            {
                TempData["error"] = "Edit event error. Try again.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CreateEvent(DateTime? startDate, DateTime? endDate)
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            DateTime selectedStartDate = (DateTime)(startDate == null ? DateTime.Today : startDate);
            DateTime selectedEndDate = (DateTime)(endDate == null ? DateTime.Today : endDate);

            CalendarEventsVM calendarEvents = new()
            {
                Event = new CalendarEvents
                {
                    UserId = userId,
                    StartDate = selectedStartDate,
                    EndDate = selectedEndDate
                }
            };
            return View(calendarEvents);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(CalendarEventsVM vm)
        {
            vm.Event.Description = sanitizer.Sanitize(vm.Event.Description);
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Create event error. Try again.";
                return View(vm);
            }

            // --- logical validation ---
            if (vm.Event.EndDate < vm.Event.StartDate)
            {
                TempData["warning"] = "End date cannot be before start date. Try again.";
                return View(vm);
            }

            //create CalendarEvents object
            CalendarEvents calEvent = new CalendarEvents
            {
                UserId = vm.Event.UserId,
                Title = vm.Event.Title,
                Description = vm.Event.Description,
                StartDate = vm.Event.StartDate,
                EndDate = vm.Event.EndDate,
                IsAllDay = vm.Event.IsAllDay,
                IsPrivate = vm.Event.IsPrivate

            };

            // --- recurrence handling ---
            if (vm.IsRecurring)
            {
                calEvent.RRule = _calendarRecurrenceService.BuildRRule(vm, calEvent.StartDate);
            }

            bool success = await _unitOfWork.CalendarEvents.Add(calEvent);
            if (success)
            {
                TempData["success"] = "Event created successfully.";
            }
            else
            {
                TempData["error"] = "Create event error. Try again.";
            }
            return RedirectToAction("Index");
        }
    }
}
