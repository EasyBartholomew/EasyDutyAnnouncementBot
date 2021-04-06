using System;

namespace EasyDutyAnnouncementBot.BL.Models
{
    public static class DateTimeHelper
    {
        public static DateTime GetNextDate(this DateTime date, DayOfWeek day)
        {
            if (date.DayOfWeek == day)
                return date;

            return date.AddDays(Math.Abs(date.DayOfWeek - day));
        }

        public static string ToLocalStringFormat(this DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }
    }
}