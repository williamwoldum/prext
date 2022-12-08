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
    
    public static bool ValidateIntervals(List<Interval> intervals, int k)
    {
        if (intervals.Any(interval => interval.ColorIdx < 0 || interval.ColorIdx >= k))
            return false;

        List<(int, bool, int)> endpoints = IntervalParser.IntervalsToEndpoints(intervals);
        bool[] usedColorLookUp = new bool[k];

        foreach ((_, bool isStart, int intervalIdx) in endpoints)
        {
            if (isStart)
            {
                if (usedColorLookUp[intervals[intervalIdx].ColorIdx]) return false;
                usedColorLookUp[intervals[intervalIdx].ColorIdx] = true;
            }
            else
            {
                usedColorLookUp[intervals[intervalIdx].ColorIdx] = false;
            }
        }

        return true;
    }
}