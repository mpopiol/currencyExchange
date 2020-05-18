using System;
using System.Collections.Generic;

namespace CurrencyExchange.Core.Helpers
{
    public static class DateHelper
    {
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static DateTime GetRecentWorkDay(this DateTime date)
            => date.DayOfWeek switch
            {
                DayOfWeek.Saturday => date.AddDays(-1),
                DayOfWeek.Sunday => date.AddDays(-2),
                _ => date
            };
    }
}