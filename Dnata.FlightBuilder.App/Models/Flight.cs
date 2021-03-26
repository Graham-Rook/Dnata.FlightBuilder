using System;
using System.Collections.Generic;

namespace Dnata.FlightBuilder.App.Models
{
    public class Flight
    {
        public IList<Segment> Segments { get; set; }
    }

    public class Segment
    {
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
    }
}
