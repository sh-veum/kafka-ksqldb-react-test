using System.Globalization;

namespace KafkaAuction.Utilities;

public static class Sorter
{
    public static List<T> SortByDate<T>(List<T> items, Func<T, string> dateSelector)
    {
        return [.. items.OrderBy(item =>
        {
            if (DateTime.TryParseExact(dateSelector(item), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }
            throw new FormatException("Invalid date format.");
        })];
    }

}
