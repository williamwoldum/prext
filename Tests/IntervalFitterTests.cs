using prext;

namespace Tests;

public class IntervalFitterTests
{
    [Theory]
    [InlineData("kolding", "10 m2 4pers u/udstyr", 72)]
    [InlineData("kolding", "15 m2 4pers", 1140)]
    [InlineData("kolding", "17 m2 4pers", 455)]
    [InlineData("kolding", "25 m2 6pers", 193)]
    [InlineData("kolding", "25 m2 Luksushytte", 3119)]
    [InlineData("kolding", "Teltpladser", 121)]
    [InlineData("kolding", "Campingvogn fortelt", 62)]
    [InlineData("kolding", "Elplads", 7912)]
    [InlineData("kolding", "2 pers. Hytte m. udstyr", 2)]
    
    [InlineData("nordsoe", "Hytte 08m2", 604)]
    [InlineData("nordsoe", "Hytte 12m2", 827)]
    [InlineData("nordsoe", "Hytte 16m2", 811)]
    [InlineData("nordsoe", "Hytte 20m2", 1016)]
    [InlineData("nordsoe", "Hytte 25m2", 1586)]
    [InlineData("nordsoe", "Teltplads", 1416)]
    [InlineData("nordsoe", "Komfortplads", 2659)]
    [InlineData("nordsoe", "El-plads", 7474)]
    public async Task PrextValidDomain(string dataSetName, string campType, int numIntervalsExpected)
    {
        (List<Interval>? intervals, Dictionary<int, int> colorMap) = TestDataLoader.LoadIntervalsFromDataSet(dataSetName, campType);
        intervals = await IntervalFitter.FitIntervals(intervals, colorMap, 15);
        
        Assert.True(intervals != null);
        Assert.Equal(intervals!.Count, numIntervalsExpected);

        Assert.True(IntervalParser.ValidateIntervals(intervals, colorMap.Count));
    }
    
    [Theory]
    [InlineData("nordsoe", "Panoramaplads")]
    public async Task PrextInValidDomainTimeout(string dataSetName, string campType)
    {
        (List<Interval>? intervals, Dictionary<int, int> colorMap) = TestDataLoader.LoadIntervalsFromDataSet(dataSetName, campType);
        await Assert.ThrowsAsync<TimeoutException>( async () => await IntervalFitter.FitIntervals(intervals, colorMap));
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
    public async Task PrextInValidDomainOverlappingIntervals(string dataSetName, string campType)
    {
        (List<Interval>? intervals, Dictionary<int, int> colorMap) = TestDataLoader.LoadIntervalsFromDataSet(dataSetName, campType);
        
        Random random = new Random();
        int randomIdx = random.Next(0, intervals.Count);
        
        intervals.Insert(0, intervals[randomIdx]);
        
        await Assert.ThrowsAsync<ArgumentException>( async () => await IntervalFitter.FitIntervals(intervals, colorMap, 15));
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
    public async Task PrextInValidDomainTooFewColors(string dataSetName, string campType)
    {
        (List<Interval>? intervals, Dictionary<int, int> colorMap) = TestDataLoader.LoadIntervalsFromDataSet(dataSetName, campType);
        
        Random random = new Random();
        int randomIdx = random.Next(0, intervals.Count);
        
        intervals[randomIdx].ColorIdx = colorMap.Count + randomIdx;
        randomIdx = random.Next(0, intervals.Count);
        intervals[randomIdx].ColorIdx = -1 - randomIdx;
        
        await Assert.ThrowsAsync<ArgumentException>( async () => await IntervalFitter.FitIntervals(intervals, colorMap, 15));
    }
}