using prext;

namespace Tests.BookingFitterTests;

public class BookingParserTests
{
    [Fact]
    public void BookingsToEndpointsTestsOverlapping()
    {
        List<Booking> bookingsInput = new List<Booking>
        {
            new Booking { StartDate = 1, EndDate = 3 },
            new Booking { StartDate = 2, EndDate = 4 }
        };

        List<(int, bool, int)> endpointsExpected = new List<(int, bool, int)>
        {
            (1, true, 0),
            (2, true, 1),
            (3, false, 0),
            (4, false, 1)
        };

        List<(int, bool, int)> endpointsActual = BookingParser.BookingsToEndpoints(bookingsInput);

        Assert.Equal(endpointsExpected.Count, endpointsActual.Count);

        for (int i = 0; i < endpointsExpected.Count; i++)
        {
            Assert.Equal(endpointsExpected[i], endpointsActual[i]);
        }
    }
    
    [Fact]
    public void BookingsToEndpointsTestsSharedEndpoint()
    {
        List<Booking> bookingsInput = new List<Booking>
        {
            new Booking { StartDate = 1, EndDate = 2 },
            new Booking { StartDate = 2, EndDate = 3 }
        };

        List<(int, bool, int)> endpointsExpected = new List<(int, bool, int)>
        {
            (1, true, 0),
            (2, false, 0),
            (2, true, 1),
            (3, false, 1)
        };

        List<(int, bool, int)> endpointsActual = BookingParser.BookingsToEndpoints(bookingsInput);

        Assert.Equal(endpointsExpected.Count, endpointsActual.Count);

        for (int i = 0; i < endpointsExpected.Count; i++)
        {
            Assert.Equal(endpointsExpected[i], endpointsActual[i]);
        }
    }
    
    [Fact]
    public void BookingsToEndpointsTestsNoOverlap()
    {
        List<Booking> bookingsInput = new List<Booking>
        {
            new Booking { StartDate = 1, EndDate = 2 },
            new Booking { StartDate = 3, EndDate = 4 }
        };

        List<(int, bool, int)> endpointsExpected = new List<(int, bool, int)>
        {
            (1, true, 0),
            (2, false, 0),
            (3, true, 1),
            (4, false, 1)
        };

        List<(int, bool, int)> endpointsActual = BookingParser.BookingsToEndpoints(bookingsInput);

        Assert.Equal(endpointsExpected.Count, endpointsActual.Count);

        for (int i = 0; i < endpointsExpected.Count; i++)
        {
            Assert.Equal(endpointsExpected[i], endpointsActual[i]);
        }
    }
    
    [Fact]
    public void BookingsToEndpointsTestsInvalid()
    {
        List<Booking> bookingsInput = new List<Booking>
        {
            new Booking { StartDate = 1, EndDate = 2 },
            new Booking { StartDate = 3, EndDate = 4 }
        };

        List<(int, bool, int)> endpointsExpected = new List<(int, bool, int)>
        {
            (1, false, 0),
            (2, true, 0),
            (3, false, 1),
            (4, true, 1)
        };

        List<(int, bool, int)> endpointsActual = BookingParser.BookingsToEndpoints(bookingsInput);

        Assert.Equal(endpointsExpected.Count, endpointsActual.Count);

        for (int i = 0; i < endpointsExpected.Count; i++)
        {
            Assert.NotEqual(endpointsExpected[i], endpointsActual[i]);
        }
    }
}