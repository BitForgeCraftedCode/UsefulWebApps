using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization.DataTypes;
using UsefulWebApps.Models.Calendar;
using UsefulWebApps.Models.ViewModels.Calendar;

namespace UsefulWebApps.Helpers.Calendar
{
    public class CalendarRecurrenceService : ICalendarRecurrenceService
    {
        public RecurrencePattern GetRecurrencePattern(CalendarEvents calendarEvent)
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

        public string GetDayOfWeekFromPattern(RecurrencePattern pattern)
        {

            DayOfWeek dayOfWeek = DayOfWeek.Monday;
            foreach (var day in pattern.ByDay)
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

        public string GetFrequencyFromPattern(RecurrencePattern pattern)
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
        public string BuildRRule(CalendarEventsVM vm, DateTime startDate)
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
    }
}
