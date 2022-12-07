using prext;

//(4, 193)      "25 m2 6pers"               //Good
//(2, 2)        "2 pers. Hytte m. udstyr"   //Good
//(3, 62)       "Campingvogn fortelt"       //Bad
//(17, 3119)    "25 m2 Luksushytte"         //Bad
(List<Booking>? bookings, int k) = BookingParser.LoadBookings(true, "15 m2 4pers");

bookings = BookingFitter.PrextNoPruning(bookings, k);

Console.WriteLine(bookings!.Count);
Console.WriteLine(BookingFitter.BookingsValidator(bookings, k));
