using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Web
{
    public static class DateTimeUtils
    {
        public static TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

        public static TimeSpan GetTimeZoneOffset(DateTime time)
        {
            return TimeZone.GetUtcOffset(time);
        }
        public static DateTimeOffset ToLocal(DateTimeOffset datetime)
        {
            return TimeZoneInfo.ConvertTime(datetime, TimeZone);
        }

        public static DateTimeOffset CreateLocal(DateTime date, TimeSpan time)
        {
            var datetime = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
            return new DateTimeOffset(datetime, GetTimeZoneOffset(datetime));
        }
        public static DateTimeOffset CreateLocal(DateTimeOffset date, TimeSpan time)
        {
            date = ToLocal(date);
            return CreateLocal(date.Date, time);
        }
    }
}
