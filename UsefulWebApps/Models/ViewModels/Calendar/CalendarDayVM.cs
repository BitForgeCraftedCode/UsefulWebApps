namespace UsefulWebApps.Models.ViewModels.Calendar
{
    public class CalendarDayVM
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
    }
}
