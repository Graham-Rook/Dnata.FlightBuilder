using Dnata.FlightBuilder.Api.Models;
using System.Collections.Generic;

namespace Dnata.FlightBuilder.Api.Interfaces
{
    public interface IFlightBuilder
    {
        IList<Flight> GetFlights();
    }
}
