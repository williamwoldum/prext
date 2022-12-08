using prext;

namespace Tests.BookingFitterTests;

public class ValidateBookingsTests
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
    public void ValidateBookingsValidDomain(string dataSetName, string campType)
    {
        (List<Booking>? bookings, int k) = TestDataLoader.LoadBookingsFromDataSet(dataSetName, campType);
        Assert.True(BookingFitter.ValidateBookings(bookings, k));
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
    public void ValidateBookingsInValidDomainOverlappingBookings(string dataSetName, string campType)
    {
        (List<Booking>? bookings, int k) = TestDataLoader.LoadBookingsFromDataSet(dataSetName, campType);
        
        Random random = new Random();
        int randomIdx = random.Next(0, bookings.Count);
        
        bookings.Insert(0, bookings[randomIdx]);
        
        Assert.False(BookingFitter.ValidateBookings(bookings, k));
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
    public void ValidateBookingsInValidDomainTooFewColors(string dataSetName, string campType)
    {
        (List<Booking>? bookings, int k) = TestDataLoader.LoadBookingsFromDataSet(dataSetName, campType);
        
        Random random = new Random();
        int randomIdx = random.Next(0, bookings.Count);
        
        bookings[randomIdx].Color = k - 1 + randomIdx;
        randomIdx = random.Next(0, bookings.Count);
        bookings[randomIdx].Color = 0 - randomIdx;
        
        Assert.False(BookingFitter.ValidateBookings(bookings, k));
    }
}