using prext;

namespace Tests.IntervalFitterTests;

public class ValidateIntervalsTests
{
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
        (List<Interval>? intervals, int k) = TestDataLoader.LoadIntervalsFromDataSet(dataSetName, campType);
        Assert.True(IntervalFitter.ValidateIntervals(intervals, k));
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
        (List<Interval>? intervals, int k) = TestDataLoader.LoadIntervalsFromDataSet(dataSetName, campType);
        
        Random random = new Random();
        int randomIdx = random.Next(0, intervals.Count);
        
        intervals.Insert(0, intervals[randomIdx]);
        
        Assert.False(IntervalFitter.ValidateIntervals(intervals, k));
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
        (List<Interval>? intervals, int k) = TestDataLoader.LoadIntervalsFromDataSet(dataSetName, campType);
        
        Random random = new Random();
        int randomIdx = random.Next(1, intervals.Count);
        
        intervals[randomIdx].Color = k - 1 + randomIdx;
        randomIdx = random.Next(0, intervals.Count);
        intervals[randomIdx].Color = 0 - randomIdx;
        
        Assert.False(IntervalFitter.ValidateIntervals(intervals, k));
    }
}