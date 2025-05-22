namespace PetStoreProject.Helpers
{
    public class DateTimeHelpers
    {
        public static DateTime GetFirstDayOfWeek(DateTime date)
        {
            DayOfWeek firstDay = DayOfWeek.Monday;
            int diff = (7 + (date.DayOfWeek - firstDay)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime GetLastDayOfWeek(DateTime date)
        {
            DayOfWeek lastDay = DayOfWeek.Sunday;
            int diff = (7 - (date.DayOfWeek - lastDay)) % 7;
            return date.AddDays(diff).Date;
        }
    }
}
