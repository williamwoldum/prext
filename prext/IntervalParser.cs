namespace prext;

public static class IntervalParser
{
    public static List<(int, bool, int)> IntervalsToEndpoints(List<Interval> intervals)
    {
        List<(int, bool, int)> endpoints = new();
        for (int i = 0; i < intervals.Count; i++)
        {
            endpoints.Add((intervals[i].StartTime, true, i));
            endpoints.Add((intervals[i].EndTime, false, i));
        }

        endpoints.Sort();
        return endpoints;
    }
}