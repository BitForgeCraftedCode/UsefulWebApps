using Ganss.Xss;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization.DataTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using UsefulWebApps.Helpers;
using UsefulWebApps.Models.Calendar;
using UsefulWebApps.Models.Friends;
using UsefulWebApps.Models.ViewModels.Calendar;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class CalendarController : Controller
    {
        private HtmlSanitizer sanitizer = new HtmlSanitizer();
        private readonly IUnitOfWork _unitOfWork;

        public CalendarController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        private async Task<IEnumerable<SelectListItem>> GetFriendsSelectListAsync(string userId)
        {
            // Get friends to populate dropdown
            (List<UserProfiles> profiles, List<Friendships> friendships) result = await _unitOfWork.Friendships.GetFriendsWithProfiles(userId);
            return result.profiles.Select(p => new SelectListItem
            {
                //?? null-coalescing operator
                Text = p.DisplayName ?? p.UserId,
                Value = p.UserId
            });
        }

        private async Task<bool> AreFriendsAsync(string userId, string otherUserId)
        {
            Friendships? friendship = await _unitOfWork.Friendships.GetExisting(userId, otherUserId);
            return friendship != null && friendship.Status == FriendshipStatus.Accepted;
        }
        public async Task<IActionResult> Index(int? year, int? month)
        {
            DateTime firstOfMonth = new DateTime(
                year ?? DateTime.Today.Year,
                month ?? DateTime.Today.Month,
                1);

            CalendarMonthVM vm = BuildCalendarMonth(firstOfMonth);
            await LoadEventsForMonth(vm);
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
            IEnumerable<SelectListItem> friendsList = await GetFriendsSelectListAsync(userId);
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

            if (!await AreFriendsAsync(userId, vm.SelectedFriendUserId))
            {
                TempData["warning"] = "You can only share events with friends.";
                return RedirectToAction("Index");
            }
            //verify the event belongs to the current user
            CalendarEvents calendarEvent = await _unitOfWork.CalendarEvents.GetById(vm.EventId);
            if (calendarEvent.UserId != userId)
            {
                TempData["warning"] = "You can only share your own events.";
                return RedirectToAction("Index");
            }
            bool success = await _unitOfWork.CalendarEventShares.ShareCalendarEvent(vm.EventId, vm.SelectedFriendUserId);
            TempData[success ? "success" : "error"] = success
                ? "Event shared successfully."
                : "Could not share event. It may already be shared with this friend.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvent(long? id, string? userID)
        {
            string? userId = User.GetUserId();
            string? role = User.GetUserRole();
            if (string.IsNullOrEmpty(userId)) return NotFound();
            if (string.IsNullOrEmpty(role)) return NotFound();

            if (id == null || id == 0)
            {
                return NotFound();
            }
            if (userId != userID && role != "Admin")
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
                RecurrencePattern pattern = GetRecurrencePattern(calendarEvent);
                calendarEventVM = new()
                {
                    Event = calendarEvent,
                    IsRecurring = string.IsNullOrEmpty(calendarEvent.RRule) ? false : true,
                    Frequency = GetFrequencyFromPattern(pattern),
                    Interval = pattern.Interval,
                    DaysOfWeek = pattern.Frequency == FrequencyType.Weekly ? GetDayOfWeekFromPattern(pattern) : string.Empty,
                    RecurrenceEndDate = pattern.Until?.Value,
                    SharedWithFriends = await _unitOfWork.CalendarEventShares.GetSharedFriendNamesForEvent(calendarEvent.Id, calendarEvent.UserId)

                };
            }
            return View(calendarEventVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(CalendarEventsVM vm)
        {
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

            if (existingEvent.IsPrivate != vm.Event.IsPrivate && await _unitOfWork.CalendarEventShares.HasShares(vm.Event.Id))
            {
                TempData["warning"] = "You cannot change privacy while this event is shared. Unshare it first.";
                return RedirectToAction("EditEvent", new { id = vm.Event.Id });
            }

            //create CalendarEvents object
            CalendarEvents calEvent = new CalendarEvents
            {
                Id = vm.Event.Id,
                UserId = vm.Event.UserId,
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
                calEvent.RRule = BuildRRule(vm, calEvent.StartDate);
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

        public async Task<IActionResult> CreateEvent()
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            CalendarEventsVM calendarEvents = new()
            {
                Event = new CalendarEvents
                {
                    UserId = userId,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today
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
                calEvent.RRule = BuildRRule(vm, calEvent.StartDate);
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

        private RecurrencePattern GetRecurrencePattern(CalendarEvents calendarEvent)
        {
            // --- Serialize only RRULE ---
            RecurrencePatternSerializer serializer = new RecurrencePatternSerializer();
            RecurrencePattern pattern;
            using (TextReader tr = new StringReader(calendarEvent.RRule))
            {
                pattern = (RecurrencePattern)serializer.Deserialize(tr);
            }
            return pattern;
        }

        private async Task LoadEventsForMonth(CalendarMonthVM vm)
        {
            string? userId = User.GetUserId();

            //[ rangeStart , rangeEnd )
            //inclusive start, exclusive end
            //TakeWhileBefore method is exclusive
            DateTime rangeStart = vm.Days.First().Date.Date;
            DateTime rangeEnd = vm.Days.Last().Date.Date.AddDays(1);

            List<CalendarEvents> events = await _unitOfWork.CalendarEvents.GetUserCalendarEventsForDateRange(rangeStart, rangeEnd, userId);
            List<CalendarEvents> eventsSharedWithMe = await _unitOfWork.CalendarEventShares.GetCalendarEventsSharedWithUserForDateRange(rangeStart, rangeEnd, userId);
            events.AddRange(eventsSharedWithMe);
            ExpandAndAttachEvents(vm, events, rangeStart, rangeEnd);
        }

        private void ExpandAndAttachEvents(
            CalendarMonthVM vm,
            List<CalendarEvents> events,
            DateTime rangeStart,
            DateTime rangeEnd)
        {
            foreach (CalendarEvents ev in events)
            {
                if (string.IsNullOrEmpty(ev.RRule))
                {
                    AttachSingleEvent(vm, ev);
                }
                else
                {
                    AttachRecurringEvent(vm, ev, rangeStart, rangeEnd);
                }
            }
        }

        private void AttachSingleEvent(CalendarMonthVM vm, CalendarEvents ev)
        {
            foreach (CalendarDayVM day in vm.Days)
            {
                if (day.Date.Date >= ev.StartDate.Date && day.Date.Date <= ev.EndDate.Date)
                {
                    day.Events.Add(ev);
                }
            }
        }

        private void AttachRecurringEvent(
            CalendarMonthVM vm,
            CalendarEvents ev,
            DateTime rangeStart,
            DateTime rangeEnd)
        {
            // Build iCal event
            CalendarEvent calEvent = new CalendarEvent
            {
                DtStart = new CalDateTime(ev.StartDate),
                DtEnd = new CalDateTime(ev.EndDate),
                Summary = ev.Title
            };

            // Attach RRULE
            if (!string.IsNullOrWhiteSpace(ev.RRule))
            {
                calEvent.RecurrenceRules.Add(
                    new RecurrencePattern(ev.RRule)
                );
            }

            // Generate occurrences starting at rangeStart
            IEnumerable<Occurrence> occurrences = calEvent.GetOccurrences().TakeWhileBefore(new CalDateTime(rangeEnd));

            // Filter to visible range
            foreach (Occurrence occurrence in occurrences)
            {
                DateTime occurrenceDate =
                     occurrence.Period.StartTime.Value.Date;

                if (occurrenceDate < rangeStart || occurrenceDate > rangeEnd)
                    continue;

                // Match calendar day
                CalendarDayVM dayVm = vm.Days.FirstOrDefault(d => d.Date.Date == occurrenceDate.Date);

                if (dayVm != null)
                {
                    dayVm.Events.Add(ev);
                }
            }
        }
        private string GetDayOfWeekFromPattern(RecurrencePattern pattern)
        {
           
            DayOfWeek dayOfWeek = DayOfWeek.Monday;
            foreach(var day in pattern.ByDay)
            {
                if (day != null) 
                { 
                    dayOfWeek = day.DayOfWeek;
                }
            }

            string dayString = dayOfWeek switch 
            { 
                DayOfWeek.Monday => "Monday",
                DayOfWeek.Tuesday => "Tuesday",
                DayOfWeek.Wednesday => "Wednesday",
                DayOfWeek.Thursday => "Thursday",
                DayOfWeek.Friday => "Friday",
                DayOfWeek.Saturday => "Saturday",
                DayOfWeek.Sunday => "Sunday",
                _ => "Monday"
            };

            return dayString;
        }
        private string GetFrequencyFromPattern(RecurrencePattern pattern)
        {
            string freq = pattern.Frequency switch
            {
                FrequencyType.Daily => "Daily",
                FrequencyType.Weekly => "Weekly",
                FrequencyType.Monthly => "Monthly",
                FrequencyType.Yearly => "Yearly",
                _ => "Weekly"
            };
            return freq;
        }

        /*
            iCal.Net mirrors the iCalendar (RFC 5545) spec almost 1:1.

            Conceptually:

            - A Calendar contains events
            - A CalendarEvent contains:
                    - Start date (DTSTART)
                    - One or more recurrence rules (RRULE)
            - A RecurrencePattern represents only the RRULE line

            Even though we only serialize the RRULE string, iCal.Net still wants to think in terms of calendar events.
         */
        private string BuildRRule(CalendarEventsVM vm, DateTime startDate)
        {
            //This rule belongs to an event that starts on this date
            CalendarEvent calendarEvent = new CalendarEvent
            {
                DtStart = new CalDateTime(startDate)
            };
            //Creating the recurrence rule object
            RecurrencePattern pattern = new RecurrencePattern
            {
                Interval = vm.Interval > 0 ? vm.Interval : 1
            };

            // --- map ui frequency to RFC frequency  FREQ=WEEKLY ---
            pattern.Frequency = vm.Frequency switch
            {
                "Daily" => FrequencyType.Daily,
                "Weekly" => FrequencyType.Weekly,
                "Monthly" => FrequencyType.Monthly,
                "Yearly" => FrequencyType.Yearly,
                _ => FrequencyType.Weekly
            };

            // --- Weekly: Day of week ---
            if (pattern.Frequency == FrequencyType.Weekly && !string.IsNullOrEmpty(vm.DaysOfWeek))
            {
                //switch expression
                //Evaluate vm.DaysOfWeek and return a value based on its content
                DayOfWeek day = vm.DaysOfWeek switch
                {
                    "Monday" => DayOfWeek.Monday,
                    "Tuesday" => DayOfWeek.Tuesday,
                    "Wednesday" => DayOfWeek.Wednesday,
                    "Thursday" => DayOfWeek.Thursday,
                    "Friday" => DayOfWeek.Friday,
                    "Saturday" => DayOfWeek.Saturday,
                    "Sunday" => DayOfWeek.Sunday,
                    _ => startDate.DayOfWeek //default case
                };
                //creat the WeekDay object -- BYDAY=TH
                pattern.ByDay = new List<WeekDay>
                {
                    new WeekDay(day)
                };
            }

            // --- Recurrence end date (UNTIL) ---
            if (vm.RecurrenceEndDate.HasValue)
            {
                pattern.Until = new CalDateTime(vm.RecurrenceEndDate.Value);
            }
            //Attach the rule to the event
            calendarEvent.RecurrenceRules.Add(pattern);

            // --- Serialize only RRULE ---
            RecurrencePatternSerializer serializer = new RecurrencePatternSerializer();
            return serializer.SerializeToString(pattern);
 
        }
        private CalendarMonthVM BuildCalendarMonth(DateTime firstOfMonth)
        {
            int year = firstOfMonth.Year;
            int month = firstOfMonth.Month;

            int daysInMonth = DateTime.DaysInMonth(year, month);
            DayOfWeek firstDow = firstOfMonth.DayOfWeek;

            // Number of blank spaces before the first day
            int offset = (int)firstDow; // Sunday=0, Monday=1...

            CalendarMonthVM vm = new CalendarMonthVM
            {
                Year = year,
                Month = month,
            };

            // --- Fill previous month days (blank boxes at start) ---
            DateTime prevMonth = firstOfMonth.AddMonths(-1);
            int prevDays = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            for (int i = offset - 1; i >= 0; i--)
            {
                vm.Days.Add(new CalendarDayVM
                {
                    Date = new DateTime(prevMonth.Year, prevMonth.Month, prevDays - i),
                    IsCurrentMonth = false
                });
            }

            // --- Fill current month ---
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(year, month, day);

                vm.Days.Add(new CalendarDayVM
                {
                    Date = date,
                    IsCurrentMonth = true,
                    IsToday = date.Date == DateTime.Today.Date
                });
            }

            // --- Fill next month's days to complete a 6×7 grid ---
            while (vm.Days.Count < 42)
            {
                DateTime last = vm.Days.Last().Date;
                vm.Days.Add(new CalendarDayVM
                {
                    Date = last.AddDays(1),
                    IsCurrentMonth = false
                });
            }

            return vm;
        }
    }
}
