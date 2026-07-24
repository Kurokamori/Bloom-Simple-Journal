using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bloom.Common
{
    public static class Dates
    {
        public const string KeyFormat = "yyyy-MM-dd";

        public static string Today() => DateTime.Now.ToString(KeyFormat, CultureInfo.InvariantCulture);

        public static string Key(DateTime date) => date.ToString(KeyFormat, CultureInfo.InvariantCulture);

        public static DateOnly Parse(string key) =>
            DateOnly.ParseExact(key, KeyFormat, CultureInfo.InvariantCulture);

        public static bool TryParse(string key, out DateOnly date) =>
            DateOnly.TryParseExact(key, KeyFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

        public static string Pretty(DateOnly date) =>
            date.ToString("dddd, MMMM d, yyyy", CultureInfo.InvariantCulture);

        public static string Pretty(string key) =>
            TryParse(key, out DateOnly date) ? Pretty(date) : key;

        public static IEnumerable<DateOnly> Range(DateOnly start, DateOnly end)
        {
            for (DateOnly d = start; d <= end; d = d.AddDays(1))
            {
                yield return d;
            }
        }
    }
}
