using AngleSharp.Dom;
using Ganss.Xss;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization.DataTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using UsefulWebApps.Models.Calendar;
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
        public IActionResult Index(int? year, int? month)
        {
            DateTime firstOfMonth = new DateTime(
                year ?? DateTime.Today.Year,
                month ?? DateTime.Today.Month,
                1);

            CalendarMonthVM vm = BuildCalendarMonth(firstOfMonth);
            return View(vm);
        }

        public IActionResult CreateEvent()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
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
                TempData["error"] = "End date cannot be before start date. Try again.";
                return View(vm);
            }

            //create CalendarEvents object
            CalendarEvents calEvent = new CalendarEvents 
            { 
                UserId = vm.IsPrivateEvent == true ? vm.Event.UserId : null,
                Title = vm.Event.Title,
                Description = vm.Event.Description,
                StartDate = vm.Event.StartDate,
                EndDate = vm.Event.EndDate,
                IsAllDay = vm.Event.IsAllDay,

            };

            // --- recurrence handling ---
            if (vm.IsRecurring)
            {
                calEvent.RRule = BuildRRule(vm, calEvent.StartDate);
            }
            Console.WriteLine(calEvent.RRule);
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
            var pattern = new RecurrencePattern
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
            var serializer = new RecurrencePatternSerializer();
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
