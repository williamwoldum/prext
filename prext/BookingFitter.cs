using System.Runtime.InteropServices;

namespace prext;


using System.Threading.Tasks;



public static class BookingFitter
{
    public static async Task<List<Booking>> Prext(List<Booking> bookings, int k, int timeoutInSecs=2)
    {
        var task = Task.Run(() => PrextNoTimeout(bookings, k));
        
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

    private static List<Booking> PrextNoTimeout(List<Booking> bookings, int k)
    {
        if (!ValidateBookings(bookings, k))
            throw new ArgumentException("Supplied bookings are invalid", nameof(bookings));

        bookings = PrepareBookingColoringsForRecoloring(bookings);
        

        List<(int, bool, int)> endpoints = BookingParser.BookingsToEndpoints(bookings);


        List<Booking>[] preColored = new List<Booking>[k];
        for (int c = 0; c < k; c++)
            preColored[c] = new List<Booking>();

        foreach (var booking in bookings.Where(booking => !booking.Movable))
            preColored[booking.Color].Add(booking);

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
            Booking booking = bookings[eIdx];

            switch (eIsStart)
            {
                // Start point of pre-colored interval
                case true when !booking.Movable:
                {
                    if (direction == -1)
                    {
                        ColorClass cc = booking.Cc!;
                        booking.Cc = null;
                        cc.Bookings.RemoveAt(cc.Bookings.Count - 1);
                        preColoredIdx[booking.Color]--;
                        i--;
                    }
                    else
                    {
                        preColoredIdx[booking.Color]++;
                        ColorClass cc = colors[booking.Color];

                        if (cc.Colors.Count > cc.Bookings.Count)
                        {
                            cc.Bookings.Add(booking);
                            booking.Cc = cc;
                            i++;
                            continue;
                        }

                        preColoredIdx[booking.Color]--;
                        i--;
                        direction = -1;
                    }

                    continue;
                }
                
                // Start point of movable interval
                case true when booking.Movable:
                {
                    int j = 0;
                    if (direction == -1)
                    {
                        (j, ccZero, _, _) = states[^1];
                        states.RemoveAt(states.Count - 1);
                        ColorClass cc = ccs[j];
                        booking.Cc = null;
                        cc.NumMovables--;
                        cc.Bookings.RemoveAt(cc.Bookings.Count - 1);
                        j++;
                        direction = 1;
                    }

                    try
                    {
                        while (j < ccs.Count)
                        {
                            ColorClass cc = ccs[j];
                            if (cc.Colors.Count > cc.Bookings.Count)
                            {
                                cc.Bookings.Add(booking);
                                cc.NumMovables++;
                                states.Add((j, ccZero, null, null));
                                if (cc == ccZero)
                                {
                                    ccZero = null;
                                }

                                booking.Cc = cc;
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
                case false when !booking.Movable:
                {
                    if (direction == 1)
                    {
                        ColorClass? cc = booking.Cc;
                        int bookingIdx = cc!.Bookings.IndexOf(booking);
                        cc.Bookings.RemoveAt(bookingIdx);

                        if (cc.Colors.Count > 1)
                        {
                            if (ccZero != null && ccZero != cc)
                            {
                                ccZero.Colors.Add(booking.Color);
                                colors[booking.Color] = ccZero;
                                int cIdx = cc.Colors.IndexOf(booking.Color);
                                cc.Colors.RemoveAt(cIdx);
                                states.Add((0, ccZero, bookingIdx, cIdx));
                            }
                            else if (ccZero != cc)
                            {
                                ColorClass ccNew = new ColorClass { Colors = new List<int> { booking.Color } };
                                ccs.Add(ccNew);
                                colors[booking.Color] = ccNew;
                                ccZero = ccNew;
                                int cIdx = cc.Colors.IndexOf(booking.Color);
                                cc.Colors.RemoveAt(cIdx);
                                states.Add((1, ccZero, bookingIdx, cIdx));
                            }
                            else
                            {
                                states.Add((2, ccZero, bookingIdx, -1));
                            }

                            i++;
                        }
                        else
                        {
                            states.Add((3, ccZero, bookingIdx, -1));
                            i++;
                        }

                        continue;
                    }

                    else
                    {
                        (int j, ccZero, int? bookingIdx, int? cIdx) = states[^1];
                        states.RemoveAt(states.Count - 1);
                        ColorClass cc = booking.Cc!;

                        switch (j)
                        {
                            case 0:
                                cc.Colors.Insert((int)cIdx!, booking.Color);
                                colors[booking.Color] = cc;
                                ccZero!.Colors.RemoveAt(ccZero.Colors.Count - 1);
                                break;
                            case 1:
                                cc.Colors.Insert((int)cIdx!, booking.Color);
                                colors[booking.Color] = cc;
                                ccs.RemoveAt(ccs.Count - 1);
                                ccZero = null;
                                break;
                        }

                        cc.Bookings.Insert((int)bookingIdx!, booking);
                        i--;
                        continue;
                    }
                }

                // End point of movable interval
                default:
                {
                    if (direction == 1)
                    {
                        ColorClass? cc = booking.Cc;
                        int bookingIdx = cc!.Bookings.IndexOf(booking);
                        cc.Bookings.RemoveAt(bookingIdx);
                        cc.NumMovables--;
                        if (cc.NumMovables == 0)
                        {
                            if (ccZero != null)
                            {
                                ccZero.Colors.AddRange(cc.Colors);
                                ccZero.Bookings.AddRange(cc.Bookings);

                                foreach (Booking b in cc.Bookings)
                                {
                                    b.Cc = ccZero;
                                }

                                foreach (int c in cc.Colors)
                                {
                                    colors[c] = ccZero;
                                }

                                int ccIdx = ccs.IndexOf(cc);
                                ccs.RemoveAt(ccIdx);
                                states.Add((0, ccZero, bookingIdx, ccIdx));
                            }
                            else
                            {
                                states.Add((1, ccZero, bookingIdx, -1));
                                ccZero = cc;
                            }
                        }
                        else
                        {
                            states.Add((2, ccZero, bookingIdx, -1));
                        }

                        i++;
                        continue;
                    }
                    else
                    {
                        (int j, ColorClass? ccZeroOld, int? bookingIdx, int? ccIdx) = states[^1];
                        states.RemoveAt(states.Count - 1);
                        ColorClass? cc = booking.Cc;
                        if (j == 0)
                        {
                            ccs.Insert((int)ccIdx!, cc!);
                            foreach (int c in cc!.Colors)
                            {
                                colors[c] = cc;
                                ccZero!.Colors.Remove(c);
                            }

                            foreach (Booking b in cc.Bookings)
                            {
                                b.Cc = cc;
                                ccZero!.Bookings.Remove(b);
                            }
                        }

                        ccZero = ccZeroOld;
                        cc!.NumMovables++;
                        cc.Bookings.Insert((int)bookingIdx!, booking);
                        i--;
                        continue;
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////

        if (i < 0) throw new ArgumentException("Bookings can not be fitted", nameof(bookings));

        while (i > 0)
        {
            i--;
            (_, bool eIsStart, int eIdx) = endpoints[i];
            Booking booking = bookings[eIdx];

            switch (eIsStart)
            {
                // Start point of pre-colored interval
                case true when !booking.Movable:
                    booking.Cc!.Bookings.RemoveAt(booking.Cc.Bookings.Count - 1);
                    continue;

                // Start point of movable interval
                case true when booking.Movable:
                {
                    (int j, ccZero, _, _) = states[^1];
                    states.RemoveAt(states.Count - 1);
                    ccs[j].NumMovables--;
                    ccs[j].Bookings.RemoveAt(ccs[j].Bookings.Count - 1);
                    continue;
                }

                // End point of pre-colored interval
                case false when !booking.Movable:
                {
                    (int j, ccZero, int? bookingIdx, int? cIdx) = states[^1];
                    states.RemoveAt(states.Count - 1);
                    if (j == 0)
                    {
                        booking.Cc!.Colors.Insert((int)cIdx!, booking.Color);
                        colors[booking.Color] = booking.Cc;
                        ccZero!.Colors.RemoveAt(ccZero.Colors.Count - 1);
                    }
                    else if (j == 1)
                    {
                        booking.Cc!.Colors.Insert((int)cIdx!, booking.Color);
                        colors[booking.Color] = booking.Cc;
                        ccs.RemoveAt(ccs.Count - 1);
                        ccZero = null;
                    }

                    booking.Cc!.Bookings.Insert((int)bookingIdx!, booking);
                    continue;
                }

                // End point of movable interval
                default:
                {
                    (int j, ColorClass? ccZeroOld, int? bookingIdx, int? ccIdx) = states[^1];
                    states.RemoveAt(states.Count - 1);
                    ColorClass? cc = booking.Cc;
                    if (j == 0)
                    {
                        ccs.Insert((int)ccIdx!, cc!);
                        foreach (int c in cc!.Colors)
                        {
                            colors[c] = cc;
                            int cIdx = ccZero!.Colors.IndexOf(c);
                            ccZero.Colors.RemoveAt(cIdx);
                        }

                        foreach (Booking b in cc.Bookings)
                        {
                            b.Cc = cc;
                            ccZero!.Bookings.Remove(b);
                        }
                    }

                    ccZero = ccZeroOld;
                    cc!.NumMovables++;
                    cc.Bookings.Insert((int)bookingIdx!, booking);

                    List<int> usedColors = (from b in cc.Bookings where b.Color != -1 select b.Color).ToList();

                    foreach (var c in cc.Colors.Where(c => !usedColors.Contains(c)))
                    {
                        booking.Color = c;
                        break;
                    }

                    continue;
                }
            }
        }

        return bookings;
    }
    
    private static List<Booking> PrepareBookingColoringsForRecoloring(List<Booking> bookings)
    {
        bookings.Sort((b1, b2) => b1.StartDate.CompareTo(b2.StartDate));
        
        foreach (Booking booking in bookings)
        {
            booking.Color = booking.Movable ? -1 : booking.OrigColor;
        }

        return bookings;
    }

    public static bool ValidateBookings(List<Booking> bookings, int k)
    {
        if (bookings.Any(booking => booking.Color < 0 || booking.Color >= k))
        {
            return false;
        }

        List<(int, bool, int)> endpoints = BookingParser.BookingsToEndpoints(bookings);
        bool[] colorMap = new bool[k];

        foreach ((_, bool isStart, int bookingIdx) in endpoints)
        {
            if (isStart)
            {
                if (colorMap[bookings[bookingIdx].Color]) return false;
                colorMap[bookings[bookingIdx].Color] = true;
            }
            else
            {
                colorMap[bookings[bookingIdx].Color] = false;
            }
        }

        return true;
    }
}