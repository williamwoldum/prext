using prext;

namespace Tests.IntervalFitterTests;

public class IntervalParserTests
{
    [Fact]
    public void IntervalsToEndpointsTestsOverlapping()
    {
        List<Interval> intervalsInput = new List<Interval>
        {
            new Interval { StartTime = 1, EndTime = 3 },
            new Interval { StartTime = 2, EndTime = 4 }
        };

        List<(int, bool, int)> endpointsExpected = new List<(int, bool, int)>
        {
            (1, true, 0),
            (2, true, 1),
            (3, false, 0),
            (4, false, 1)
        };

        List<(int, bool, int)> endpointsActual = IntervalParser.IntervalsToEndpoints(intervalsInput);

        Assert.Equal(endpointsExpected.Count, endpointsActual.Count);

        for (int i = 0; i < endpointsExpected.Count; i++)
        {
            Assert.Equal(endpointsExpected[i], endpointsActual[i]);
        }
    }
    
    [Fact]
    public void IntervalsToEndpointsTestsSharedEndpoint()
    {
        List<Interval> intervalsInput = new List<Interval>
        {
            new Interval { StartTime = 1, EndTime = 2 },
            new Interval { StartTime = 2, EndTime = 3 }
        };

        List<(int, bool, int)> endpointsExpected = new List<(int, bool, int)>
        {
            (1, true, 0),
            (2, false, 0),
            (2, true, 1),
            (3, false, 1)
        };

        List<(int, bool, int)> endpointsActual = IntervalParser.IntervalsToEndpoints(intervalsInput);

        Assert.Equal(endpointsExpected.Count, endpointsActual.Count);

        for (int i = 0; i < endpointsExpected.Count; i++)
        {
            Assert.Equal(endpointsExpected[i], endpointsActual[i]);
        }
    }
    
    [Fact]
    public void IntervalsToEndpointsTestsNoOverlap()
    {
        List<Interval> intervalsInput = new List<Interval>
        {
            new Interval { StartTime = 1, EndTime = 2 },
            new Interval { StartTime = 3, EndTime = 4 }
        };

        List<(int, bool, int)> endpointsExpected = new List<(int, bool, int)>
        {
            (1, true, 0),
            (2, false, 0),
            (3, true, 1),
            (4, false, 1)
        };

        List<(int, bool, int)> endpointsActual = IntervalParser.IntervalsToEndpoints(intervalsInput);

        Assert.Equal(endpointsExpected.Count, endpointsActual.Count);

        for (int i = 0; i < endpointsExpected.Count; i++)
        {
            Assert.Equal(endpointsExpected[i], endpointsActual[i]);
        }
    }
    
    [Fact]
    public void IntervalsToEndpointsTestsInvalid()
    {
        List<Interval> intervalsInput = new List<Interval>
        {
            new Interval { StartTime = 1, EndTime = 2 },
            new Interval { StartTime = 3, EndTime = 4 }
        };

        List<(int, bool, int)> endpointsExpected = new List<(int, bool, int)>
        {
            (1, false, 0),
            (2, true, 0),
            (3, false, 1),
            (4, true, 1)
        };

        List<(int, bool, int)> endpointsActual = IntervalParser.IntervalsToEndpoints(intervalsInput);

        Assert.Equal(endpointsExpected.Count, endpointsActual.Count);

        for (int i = 0; i < endpointsExpected.Count; i++)
        {
            Assert.NotEqual(endpointsExpected[i], endpointsActual[i]);
        }
    }
}