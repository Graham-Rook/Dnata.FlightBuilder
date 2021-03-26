using System;

namespace Dnata.FlightBuilder.App.Models
{
    public class FilterRequestModel
    {
        public double? GroundedHours { get; set; }
        public DateTime? DepartureDateStart { get; set; }
    }
}
