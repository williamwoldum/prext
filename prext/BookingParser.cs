namespace prext;

public static class BookingParser
{
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