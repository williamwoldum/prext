using prext;

namespace Tests;

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
    
    //////////////////////////////////////////////////////////
    
    [Theory]
    [InlineData("kolding", "10 m2 4pers u/udstyr")]
    [InlineData("kolding", "15 m2 4pers")]
    [InlineData("kolding", "17 m2 4pers")]
    [InlineData("kolding", "25 m2 6pers")]
    [InlineData("kolding", "25 m2 Luksushytte")]
    [InlineData("kolding", "Teltpladser")]
    [InlineData("kolding", "Campingvogn fortelt")]
    [InlineData("kolding", "Elplads")]
    [InlineData("kolding", "2 pers. Hytte m. udstyr")]
    
    [InlineData("nordsoe", "Hytte 08m2")]
    [InlineData("nordsoe", "Hytte 12m2")]
    [InlineData("nordsoe", "Hytte 16m2")]
    [InlineData("nordsoe", "Hytte 20m2")]
    [InlineData("nordsoe", "Hytte 25m2")]
    [InlineData("nordsoe", "Teltplads")]
    [InlineData("nordsoe", "Komfortplads")]
    [InlineData("nordsoe", "El-plads")]
    [InlineData("nordsoe", "Panoramaplads")]
    public void ValidateIntervalsValidDomain(string dataSetName, string campType)
    {
        (List<Interval> intervals, Dictionary<int, int> colorMap) = TestDataLoader.LoadIntervalsFromDataSet(dataSetName, campType);
        Assert.True(IntervalParser.ValidateIntervals(intervals, colorMap.Count));
    }
    
    [Theory]
    [InlineData("kolding", "10 m2 4pers u/udstyr")]
    [InlineData("kolding", "15 m2 4pers")]
    [InlineData("kolding", "17 m2 4pers")]
    [InlineData("kolding", "25 m2 6pers")]
    [InlineData("kolding", "25 m2 Luksushytte")]
    [InlineData("kolding", "Teltpladser")]
    [InlineData("kolding", "Campingvogn fortelt")]
    [InlineData("kolding", "Elplads")]
    [InlineData("kolding", "2 pers. Hytte m. udstyr")]
    
    [InlineData("nordsoe", "Hytte 08m2")]
    [InlineData("nordsoe", "Hytte 12m2")]
    [InlineData("nordsoe", "Hytte 16m2")]
    [InlineData("nordsoe", "Hytte 20m2")]
    [InlineData("nordsoe", "Hytte 25m2")]
    [InlineData("nordsoe", "Teltplads")]
    [InlineData("nordsoe", "Komfortplads")]
    [InlineData("nordsoe", "El-plads")]
    [InlineData("nordsoe", "Panoramaplads")]
    public void ValidateIntervalsInValidDomainOverlappingIntervals(string dataSetName, string campType)
    {
        (List<Interval> intervals, Dictionary<int, int> colorMap) = TestDataLoader.LoadIntervalsFromDataSet(dataSetName, campType);
        
        Random random = new Random();
        int randomIdx = random.Next(0, intervals.Count);
        
        intervals.Insert(0, intervals[randomIdx]);
        
        Assert.False(IntervalParser.ValidateIntervals(intervals, colorMap.Count));
    }
    
    [Theory]
    [InlineData("kolding", "10 m2 4pers u/udstyr")]
    [InlineData("kolding", "15 m2 4pers")]
    [InlineData("kolding", "17 m2 4pers")]
    [InlineData("kolding", "25 m2 6pers")]
    [InlineData("kolding", "25 m2 Luksushytte")]
    [InlineData("kolding", "Teltpladser")]
    [InlineData("kolding", "Campingvogn fortelt")]
    [InlineData("kolding", "Elplads")]
    [InlineData("kolding", "2 pers. Hytte m. udstyr")]
    
    [InlineData("nordsoe", "Hytte 08m2")]
    [InlineData("nordsoe", "Hytte 12m2")]
    [InlineData("nordsoe", "Hytte 16m2")]
    [InlineData("nordsoe", "Hytte 20m2")]
    [InlineData("nordsoe", "Hytte 25m2")]
    [InlineData("nordsoe", "Teltplads")]
    [InlineData("nordsoe", "Komfortplads")]
    [InlineData("nordsoe", "El-plads")]
    [InlineData("nordsoe", "Panoramaplads")]
    public void ValidateIntervalsInValidDomainTooFewColors(string dataSetName, string campType)
    {
        (List<Interval> intervals, Dictionary<int, int> colorMap) = TestDataLoader.LoadIntervalsFromDataSet(dataSetName, campType);
        
        Random random = new Random();
        int randomIdx = random.Next(1, intervals.Count);
        
        intervals[randomIdx].ColorIdx = colorMap.Count - 1 + randomIdx;
        randomIdx = random.Next(0, intervals.Count);
        intervals[randomIdx].ColorIdx = 0 - randomIdx;
        
        Assert.False(IntervalParser.ValidateIntervals(intervals, colorMap.Count));
    }
}