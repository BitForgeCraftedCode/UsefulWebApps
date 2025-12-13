namespace UsefulWebApps.Models.ViewModels.Calendar
{
    public class CalendarMonthVM
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public List<CalendarDayVM> Days { get; set; } = new();
    }
}
