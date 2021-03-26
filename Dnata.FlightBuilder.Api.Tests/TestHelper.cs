using Dnata.FlightBuilder.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dnata.FlightBuilder.Api.Tests
{
    public class TestHelper
    {

        internal static Flight CreateFlight(params DateTime[] dates)
        {
            if (dates.Length % 2 != 0)
                throw new ArgumentException("You must pass an even number of dates,", "dates");

            var departureDates = dates.Where((date, index) => index % 2 == 0);
            var arrivalDates = dates.Where((date, index) => index % 2 == 1);

            var segments = departureDates
                .Zip(arrivalDates, (departureDate, arrivalDate) => new Segment { DepartureDate = departureDate, ArrivalDate = arrivalDate })
                .ToList();

            return new Flight { Segments = segments };
        }

    }
}
