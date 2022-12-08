namespace prext;

using System.Threading.Tasks;

public static class IntervalFitter
{
    public static async Task<List<Interval>> Prext(List<Interval> intervals, int k, int timeoutInSecs=2)
    {
        var task = Task.Run(() => PrextNoTimeout(intervals, k));
        
        try
        {
            if (await Task.WhenAny(task, Task.Delay(timeoutInSecs * 1000)) == task)
            {
                return task.Result;
            }
            throw new TimeoutException("Timed out");
        }
        
        catch (AggregateException ae)
        {
            foreach (var ex in ae.InnerExceptions)
            {
                throw ex;
            }
        }

        throw new Exception();
    }

    private static List<Interval> PrextNoTimeout(List<Interval> intervals, int k)
    {
        if (!ValidateIntervals(intervals, k))
            throw new ArgumentException("Supplied intervals are invalid", nameof(intervals));

        intervals = PrepareIntervalColoringsForRecoloring(intervals);
        
        intervals.Sort((b1, b2) => b1.StartTime.CompareTo(b2.StartTime));
        List<(int, bool, int)> endpoints = IntervalParser.IntervalsToEndpoints(intervals);


        List<Interval>[] preColored = new List<Interval>[k];
        for (int c = 0; c < k; c++)
            preColored[c] = new List<Interval>();

        foreach (var interval in intervals.Where(interval => !interval.Movable))
            preColored[interval.Color].Add(interval);

        int[] preColoredIdx = new int[k];


        List<ColorClass> ccs = new List<ColorClass> { new ColorClass { Colors = Enumerable.Range(0, k).ToList() } };

        ColorClass? ccZero = ccs[0];

        ColorClass[] colors = new ColorClass[k];
        for (int c = 0; c < k; c++)
            colors[c] = ccZero;


        List<(int, ColorClass?, int?, int?)> states = new List<(int, ColorClass?, int?, int?)>();

        int direction = 1;
        int i = 0;

        ////////////////////////////////////////////////////////////////////////////////

        while (true)
        {
            if (i >= endpoints.Count || i < 0) break;

            (_, bool eIsStart, int eIdx) = endpoints[i];
            Interval interval = intervals[eIdx];

            switch (eIsStart)
            {
                // Start point of pre-colored interval
                case true when !interval.Movable:
                {
                    if (direction == -1)
                    {
                        ColorClass cc = interval.Cc!;
                        interval.Cc = null;
                        cc.Intervals.RemoveAt(cc.Intervals.Count - 1);
                        preColoredIdx[interval.Color]--;
                        i--;
                    }
                    else
                    {
                        preColoredIdx[interval.Color]++;
                        ColorClass cc = colors[interval.Color];

                        if (cc.Colors.Count > cc.Intervals.Count)
                        {
                            cc.Intervals.Add(interval);
                            interval.Cc = cc;
                            i++;
                            continue;
                        }

                        preColoredIdx[interval.Color]--;
                        i--;
                        direction = -1;
                    }

                    continue;
                }
                
                // Start point of movable interval
                case true when interval.Movable:
                {
                    int j = 0;
                    if (direction == -1)
                    {
                        (j, ccZero, _, _) = states[^1];
                        states.RemoveAt(states.Count - 1);
                        ColorClass cc = ccs[j];
                        interval.Cc = null;
                        cc.NumMovables--;
                        cc.Intervals.RemoveAt(cc.Intervals.Count - 1);
                        j++;
                        direction = 1;
                    }

                    try
                    {
                        while (j < ccs.Count)
                        {
                            ColorClass cc = ccs[j];
                            if (cc.Colors.Count > cc.Intervals.Count)
                            {
                                cc.Intervals.Add(interval);
                                cc.NumMovables++;
                                states.Add((j, ccZero, null, null));
                                if (cc == ccZero)
                                {
                                    ccZero = null;
                                }

                                interval.Cc = cc;
                                throw new Exception();
                            }

                            j++;
                        }
                    }
                    catch (Exception e)
                    {
                        i++;
                        continue;
                    }

                    i--;
                    direction = -1;
                    continue;
                }

                // End point of pre-colored interval
                case false when !interval.Movable:
                {
                    if (direction == 1)
                    {
                        ColorClass? cc = interval.Cc;
                        int intervalIdx = cc!.Intervals.IndexOf(interval);
                        cc.Intervals.RemoveAt(intervalIdx);

                        if (cc.Colors.Count > 1)
                        {
                            if (ccZero != null && ccZero != cc)
                            {
                                ccZero.Colors.Add(interval.Color);
                                colors[interval.Color] = ccZero;
                                int cIdx = cc.Colors.IndexOf(interval.Color);
                                cc.Colors.RemoveAt(cIdx);
                                states.Add((0, ccZero, intervalIdx, cIdx));
                            }
                            else if (ccZero != cc)
                            {
                                ColorClass ccNew = new ColorClass { Colors = new List<int> { interval.Color } };
                                ccs.Add(ccNew);
                                colors[interval.Color] = ccNew;
                                ccZero = ccNew;
                                int cIdx = cc.Colors.IndexOf(interval.Color);
                                cc.Colors.RemoveAt(cIdx);
                                states.Add((1, ccZero, intervalIdx, cIdx));
                            }
                            else
                            {
                                states.Add((2, ccZero, intervalIdx, -1));
                            }

                            i++;
                        }
                        else
                        {
                            states.Add((3, ccZero, intervalIdx, -1));
                            i++;
                        }

                        continue;
                    }

                    else
                    {
                        (int j, ccZero, int? intervalIdx, int? cIdx) = states[^1];
                        states.RemoveAt(states.Count - 1);
                        ColorClass cc = interval.Cc!;

                        switch (j)
                        {
                            case 0:
                                cc.Colors.Insert((int)cIdx!, interval.Color);
                                colors[interval.Color] = cc;
                                ccZero!.Colors.RemoveAt(ccZero.Colors.Count - 1);
                                break;
                            case 1:
                                cc.Colors.Insert((int)cIdx!, interval.Color);
                                colors[interval.Color] = cc;
                                ccs.RemoveAt(ccs.Count - 1);
                                ccZero = null;
                                break;
                        }

                        cc.Intervals.Insert((int)intervalIdx!, interval);
                        i--;
                        continue;
                    }
                }

                // End point of movable interval
                default:
                {
                    if (direction == 1)
                    {
                        ColorClass? cc = interval.Cc;
                        int intervalIdx = cc!.Intervals.IndexOf(interval);
                        cc.Intervals.RemoveAt(intervalIdx);
                        cc.NumMovables--;
                        if (cc.NumMovables == 0)
                        {
                            if (ccZero != null)
                            {
                                ccZero.Colors.AddRange(cc.Colors);
                                ccZero.Intervals.AddRange(cc.Intervals);

                                foreach (Interval b in cc.Intervals)
                                {
                                    b.Cc = ccZero;
                                }

                                foreach (int c in cc.Colors)
                                {
                                    colors[c] = ccZero;
                                }

                                int ccIdx = ccs.IndexOf(cc);
                                ccs.RemoveAt(ccIdx);
                                states.Add((0, ccZero, intervalIdx, ccIdx));
                            }
                            else
                            {
                                states.Add((1, ccZero, intervalIdx, -1));
                                ccZero = cc;
                            }
                        }
                        else
                        {
                            states.Add((2, ccZero, intervalIdx, -1));
                        }

                        i++;
                        continue;
                    }
                    else
                    {
                        (int j, ColorClass? ccZeroOld, int? intervalIdx, int? ccIdx) = states[^1];
                        states.RemoveAt(states.Count - 1);
                        ColorClass? cc = interval.Cc;
                        if (j == 0)
                        {
                            ccs.Insert((int)ccIdx!, cc!);
                            foreach (int c in cc!.Colors)
                            {
                                colors[c] = cc;
                                ccZero!.Colors.Remove(c);
                            }

                            foreach (Interval b in cc.Intervals)
                            {
                                b.Cc = cc;
                                ccZero!.Intervals.Remove(b);
                            }
                        }

                        ccZero = ccZeroOld;
                        cc!.NumMovables++;
                        cc.Intervals.Insert((int)intervalIdx!, interval);
                        i--;
                        continue;
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////

        if (i < 0) throw new ArgumentException("Intervals can not be fitted", nameof(intervals));

        while (i > 0)
        {
            i--;
            (_, bool eIsStart, int eIdx) = endpoints[i];
            Interval interval = intervals[eIdx];

            switch (eIsStart)
            {
                // Start point of pre-colored interval
                case true when !interval.Movable:
                    interval.Cc!.Intervals.RemoveAt(interval.Cc.Intervals.Count - 1);
                    continue;

                // Start point of movable interval
                case true when interval.Movable:
                {
                    (int j, ccZero, _, _) = states[^1];
                    states.RemoveAt(states.Count - 1);
                    ccs[j].NumMovables--;
                    ccs[j].Intervals.RemoveAt(ccs[j].Intervals.Count - 1);
                    continue;
                }

                // End point of pre-colored interval
                case false when !interval.Movable:
                {
                    (int j, ccZero, int? intervalIdx, int? cIdx) = states[^1];
                    states.RemoveAt(states.Count - 1);
                    if (j == 0)
                    {
                        interval.Cc!.Colors.Insert((int)cIdx!, interval.Color);
                        colors[interval.Color] = interval.Cc;
                        ccZero!.Colors.RemoveAt(ccZero.Colors.Count - 1);
                    }
                    else if (j == 1)
                    {
                        interval.Cc!.Colors.Insert((int)cIdx!, interval.Color);
                        colors[interval.Color] = interval.Cc;
                        ccs.RemoveAt(ccs.Count - 1);
                        ccZero = null;
                    }

                    interval.Cc!.Intervals.Insert((int)intervalIdx!, interval);
                    continue;
                }

                // End point of movable interval
                default:
                {
                    (int j, ColorClass? ccZeroOld, int? intervalIdx, int? ccIdx) = states[^1];
                    states.RemoveAt(states.Count - 1);
                    ColorClass? cc = interval.Cc;
                    if (j == 0)
                    {
                        ccs.Insert((int)ccIdx!, cc!);
                        foreach (int c in cc!.Colors)
                        {
                            colors[c] = cc;
                            int cIdx = ccZero!.Colors.IndexOf(c);
                            ccZero.Colors.RemoveAt(cIdx);
                        }

                        foreach (Interval b in cc.Intervals)
                        {
                            b.Cc = cc;
                            ccZero!.Intervals.Remove(b);
                        }
                    }

                    ccZero = ccZeroOld;
                    cc!.NumMovables++;
                    cc.Intervals.Insert((int)intervalIdx!, interval);

                    List<int> usedColors = (from b in cc.Intervals where b.Color != -1 select b.Color).ToList();

                    foreach (var c in cc.Colors.Where(c => !usedColors.Contains(c)))
                    {
                        interval.Color = c;
                        break;
                    }

                    continue;
                }
            }
        }

        return intervals;
    }
    
    private static List<Interval> PrepareIntervalColoringsForRecoloring(List<Interval> intervals)
    {
        foreach (Interval interval in intervals)
        {
            interval.Color = interval.Movable ? -1 : interval.OrigColor;
        }

        return intervals;
    }

    public static bool ValidateIntervals(List<Interval> intervals, int k)
    {
        if (intervals.Any(interval => interval.Color < 0 || interval.Color >= k))
        {
            return false;
        }

        List<(int, bool, int)> endpoints = IntervalParser.IntervalsToEndpoints(intervals);
        bool[] colorMap = new bool[k];

        foreach ((_, bool isStart, int intervalIdx) in endpoints)
        {
            if (isStart)
            {
                if (colorMap[intervals[intervalIdx].Color]) return false;
                colorMap[intervals[intervalIdx].Color] = true;
            }
            else
            {
                colorMap[intervals[intervalIdx].Color] = false;
            }
        }

        return true;
    }
}