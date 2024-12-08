using System;

namespace WebTracNghiemTiengAnhTHPT
{
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public static DateTime ConvertToLocalTime(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZone);
        }

        public static DateTime ConvertToUtcTime(DateTime localDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, TimeZone);
        }
    }
}