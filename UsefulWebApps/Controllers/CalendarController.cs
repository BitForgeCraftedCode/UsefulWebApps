using Microsoft.AspNetCore.Mvc;
using UsefulWebApps.Models.ViewModels.Calendar;

namespace UsefulWebApps.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index(int? year, int? month)
        {
            DateTime firstOfMonth = new DateTime(
                year ?? DateTime.Today.Year,
                month ?? DateTime.Today.Month,
                1);

            CalendarMonthVM vm = BuildCalendarMonth(firstOfMonth);
            return View(vm);
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
