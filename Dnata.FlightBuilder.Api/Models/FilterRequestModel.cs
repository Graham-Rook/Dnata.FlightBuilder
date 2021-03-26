using System;

namespace Dnata.FlightBuilder.Api.Models
{
    public class FilterRequestModel
    {
        public double? GroundedHours { get; set; }
        public DateTime? DepartureDateStart { get; set; }

    }
}
