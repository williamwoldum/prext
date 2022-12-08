using System.Globalization;
using prext;

namespace Tests.BookingFitterTests;

public static class TestDataLoader
{
    public static (List<Booking>, int) LoadBookingsFromDataSet(string dataSetName, string campType)
    {
        List<Booking> bookings = new();
        Dictionary<int, int> colorMap = new();
        int nextColor = 0;

        string filePath = @$"..\..\..\BookingFitterTests\TestData\{dataSetName}.csv";
        bool isKolding = dataSetName.Equals("kolding");

        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var row = reader.ReadLine()?.Split(',');
                if (row == null) return (new(), 0);
                if (isKolding && row[6].Equals(campType) || !isKolding && row[7].Equals(campType))
                    bookings.Add(BookingFromRow(row, colorMap, isKolding, ref nextColor));
            }
        }

        int k = colorMap.Count;
        
        bookings = SanitizeBookings(bookings, k);

        return (bookings, k);
    }

    private static Booking BookingFromRow(string[] row, Dictionary<int, int> colorMap, bool isKolding,
        ref int nextColor)
    {
        Booking booking = new Booking();

        int idx = Convert.ToInt32(!isKolding);
        booking.Id = int.Parse(row[idx++]);
        booking.StartDate = DateToOrdinal(DateStrToDateTime(row[idx++]));
        booking.EndDate = DateToOrdinal(DateStrToDateTime(row[idx++]));

        if (booking.StartDate > booking.EndDate)
            (booking.StartDate, booking.EndDate) = (booking.EndDate, booking.StartDate);

        booking.Movable = !row[idx++].Contains("ikke flytbar");

        int c = int.Parse(row[idx++]);

        if (!colorMap.ContainsKey(c))
        {
            colorMap[c] = nextColor++;
        }

        booking.OrigColor = colorMap[c];
        booking.Color = booking.OrigColor;

        return booking;
    }

    private static DateTime DateStrToDateTime(string dateStr)
    {
        dateStr = dateStr.Replace("/", "-");
        dateStr = dateStr.Split(" ")[0];
        return DateTime.Parse(dateStr, new CultureInfo("da-DK"));
    }

    private static List<Booking> SanitizeBookings(List<Booking> bookings, int k)
    {
        List<(int, bool, int)> endpoints = BookingParser.BookingsToEndpoints(bookings);
        Booking?[] colors = new Booking?[k];

        List<Booking> filtered = new();

        foreach (var endpoint in endpoints)
        {
            Booking booking = bookings[endpoint.Item3];

            if (endpoint.Item2)
            {
                colors[booking.OrigColor] ??= booking;
            }
            else if (colors[booking.OrigColor] != null && colors[booking.OrigColor]!.Equals(booking))
            {
                filtered.Add(booking);
                colors[booking.OrigColor] = null;
            }
        }

        return filtered;
    }
    
    private static int DateToOrdinal(DateTime date)
    {
        DateTime epoc = new DateTime(1, 1, 1);
        return (int)(date - epoc).TotalDays + 1;
    }
}