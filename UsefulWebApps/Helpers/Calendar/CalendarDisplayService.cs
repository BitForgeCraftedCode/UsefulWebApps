using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using UsefulWebApps.Models.Calendar;
using UsefulWebApps.Models.ViewModels.Calendar;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Helpers.Calendar
{
    public class CalendarDisplayService : ICalendarDisplayService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CalendarDisplayService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CalendarMonthVM BuildCalendarMonth(DateTime firstOfMonth)
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

        public async Task LoadEventsForMonth(CalendarMonthVM vm, string? userId)
        {
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

        public void ExpandAndAttachEvents(
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

        public void AttachSingleEvent(CalendarMonthVM vm, CalendarEvents ev)
        {
            foreach (CalendarDayVM day in vm.Days)
            {
                if (day.Date.Date >= ev.StartDate.Date && day.Date.Date <= ev.EndDate.Date)
                {
                    day.Events.Add(ev);
                }
            }
        }

        public void AttachRecurringEvent(
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
    }
}
