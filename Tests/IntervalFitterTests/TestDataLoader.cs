using System.Globalization;
using prext;

namespace Tests.IntervalFitterTests;

public static class TestDataLoader
{
    public static (List<Interval>, Dictionary<int, int>) LoadIntervalsFromDataSet(string dataSetName, string campType)
    {
        List<Interval> intervals = new();
        Dictionary<int, int> colorMap = new();
        
        int nextKey = 0;

        string filePath = @$"..\..\..\IntervalFitterTests\TestData\{dataSetName}.csv";
        bool isKolding = dataSetName.Equals("kolding");

        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var row = reader.ReadLine()?.Split(',');
                if (row == null) return (new(), new());
                if (isKolding && row[6].Equals(campType) || !isKolding && row[7].Equals(campType))
                    intervals.Add(IntervalFromRow(row, colorMap, isKolding, ref nextKey));
            }
        }

        int k = colorMap.Count;
        
        intervals = SanitizeIntervals(intervals, k);

        return (intervals, colorMap);
    }

    private static Interval IntervalFromRow(string[] row, Dictionary<int, int> colorMap, bool isKolding,
        ref int nextKey)
    {
        Interval interval = new Interval();

        int idx = Convert.ToInt32(!isKolding);
        interval.Id = int.Parse(row[idx++]);
        interval.StartTime = DateToOrdinal(DateStrToDateTime(row[idx++]));
        interval.EndTime = DateToOrdinal(DateStrToDateTime(row[idx++]));

        if (interval.StartTime > interval.EndTime)
            (interval.StartTime, interval.EndTime) = (interval.EndTime, interval.StartTime);

        interval.Movable = !row[idx++].Contains("ikke flytbar");

        int c = int.Parse(row[idx]);
        
        if (!colorMap.ContainsValue(c))
            colorMap[nextKey++] = c;

        interval.Color = c;
        
        var cKey = colorMap.FirstOrDefault(x => x.Value == c).Key;
        interval.ColorIdx = cKey;

        return interval;
    }

    private static DateTime DateStrToDateTime(string dateStr)
    {
        dateStr = dateStr.Replace("/", "-");
        dateStr = dateStr.Split(" ")[0];
        return DateTime.Parse(dateStr, new CultureInfo("da-DK"));
    }

    private static List<Interval> SanitizeIntervals(List<Interval> intervals, int k)
    {
        List<(int, bool, int)> endpoints = IntervalParser.IntervalsToEndpoints(intervals);
        int?[] usedColorKeys = new int?[k];

        List<Interval> filtered = new();

        foreach ((_, bool isStart, int intervalIdx) in endpoints)
        {
            Interval interval = intervals[intervalIdx];

            if (isStart)
            {
                usedColorKeys[interval.ColorIdx] ??= intervalIdx;
            }
            else if (usedColorKeys[interval.ColorIdx] == intervalIdx)
            {
                filtered.Add(interval);
                usedColorKeys[interval.ColorIdx] = null;
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