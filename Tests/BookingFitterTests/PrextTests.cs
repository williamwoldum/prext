using prext;

namespace Tests.BookingFitterTests;

public class PrextTests
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
    public async Task PrextValidDomain(string dataSetName, string campType, int numBookingsExpected)
    {
        (List<Booking>? bookings, int k) = TestDataLoader.LoadBookingsFromDataSet(dataSetName, campType);
        bookings = await BookingFitter.Prext(bookings, k, 15);
        
        Assert.True(bookings != null);
        Assert.Equal(bookings!.Count, numBookingsExpected);

        Assert.True(BookingFitter.ValidateBookings(bookings, k));
    }
    
    [Theory]
    [InlineData("nordsoe", "Panoramaplads")]
    public async Task PrextInValidDomainTimeout(string dataSetName, string campType)
    {
        (List<Booking>? bookings, int k) = TestDataLoader.LoadBookingsFromDataSet(dataSetName, campType);
        await Assert.ThrowsAsync<TimeoutException>( async () => await BookingFitter.Prext(bookings, k));
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
    public async Task PrextInValidDomainOverlappingBookings(string dataSetName, string campType)
    {
        (List<Booking>? bookings, int k) = TestDataLoader.LoadBookingsFromDataSet(dataSetName, campType);
        
        Random random = new Random();
        int randomIdx = random.Next(0, bookings.Count);
        
        bookings.Insert(0, bookings[randomIdx]);
        
        await Assert.ThrowsAsync<ArgumentException>( async () => await BookingFitter.Prext(bookings, k, 15));
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
        (List<Booking>? bookings, int k) = TestDataLoader.LoadBookingsFromDataSet(dataSetName, campType);
        
        Random random = new Random();
        int randomIdx = random.Next(0, bookings.Count);
        
        bookings[randomIdx].Color = k + randomIdx;
        randomIdx = random.Next(0, bookings.Count);
        bookings[randomIdx].Color = -1 - randomIdx;
        
        await Assert.ThrowsAsync<ArgumentException>( async () => await BookingFitter.Prext(bookings, k, 15));
    }
}