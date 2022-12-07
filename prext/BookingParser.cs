using System.Globalization;

namespace prext;

public static class BookingParser
{
    public static (List<Booking>, int) LoadBookings(bool isKolding, string bookingType)
    {
        List<Booking> bookings = new();
        Dictionary<int, int> colorMap = new();
        int nextColor = 0;

        string filePath = isKolding
            ? @"C:\Users\William\OneDrive\Skrivebord\Eddies\prext\prext\prext\Data\kolding.csv"
            : @"C:\Users\William\OneDrive\Skrivebord\Eddies\prext\prext\prext\Data\nordsoe.csv";

        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var row = reader.ReadLine()?.Split(',');
                if (row == null) return (new(), 0);
                if (isKolding && row[6].Equals(bookingType) || !isKolding && row[7].Equals(bookingType))
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
        booking.Color = booking.Movable ? -1 : booking.OrigColor;
        booking.CreationDate = DateToOrdinal(DateStrToDateTime(row[idx]));

        return booking;
    }

    private static DateTime DateStrToDateTime(string dateStr)
    {
        return DateTime.Parse(dateStr, new CultureInfo("da-DK"));
    }

    private static List<Booking> SanitizeBookings(List<Booking> bookings, int k)
    {
        List<(int, bool, int)> endpoints = BookingsToEndpoints(bookings);
        Booking?[] colors = new Booking?[k];

        List<Booking> filtered = new();

        foreach (var endpoint in endpoints)
        {
            Booking booking = bookings[endpoint.Item3];

            if (booking.CreationDate > booking.StartDate)
            {
                booking.CreationDate = booking.StartDate;
            }

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

    public static List<(int, bool, int)> BookingsToEndpoints(List<Booking> bookings)
    {
        List<(int, bool, int)> endpoints = new();
        for (int i = 0; i < bookings.Count; i++)
        {
            endpoints.Add((bookings[i].StartDate, true, i));
            endpoints.Add((bookings[i].EndDate, false, i));
        }

        endpoints.Sort();
        return endpoints;
    }

    private static int DateToOrdinal(DateTime date)
    {
        DateTime epoc = new DateTime(1, 1, 1);
        return (int)(date - epoc).TotalDays + 1;
    }
}