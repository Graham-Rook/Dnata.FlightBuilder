using Dnata.FlightBuilder.Api.Interfaces;
using Dnata.FlightBuilder.Api.Models;
using Dnata.FlightBuilder.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Dnata.FlightBuilder.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : ControllerBase
    {
        private IFlightBuilder _flightBuider;

        public FlightController(IFlightBuilder flightBuilder)
        {
            _flightBuider = flightBuilder;
        }

        [HttpPost("FindFlights")]
        public IActionResult FindFlights([FromBody] FilterRequestModel requestModel)
        {
            var validresult = IsFilterValid(requestModel);

            if (validresult.IsValid)
            {
                //fetch flight data
                var flights = _flightBuider.GetFlights();

                //filter flights that have not already departed based on provided departure date, if null use now
                var query = flights.Where(x => !x.Segments.Any(x => x.DepartureDate < (requestModel.DepartureDateStart ?? DateTime.Now)));

                //fliter flights that have arrival before departed dates, I assume these are invalid flights so we always want to filter these out
                query = query.Where(x => !x.Segments.Any(x => x.ArrivalDate < x.DepartureDate));

                //flights that have not grounded longer than combined grounded hours range provide, default to 2 if not provided
                query = query.Where(x => x.Segments.Count > 1 ? (
                                            x.Segments.Select((i, index) =>
                                            {
                                                return new { hours = x.Segments.Skip(index + 1).FirstOrDefault()?.DepartureDate.Subtract(i.ArrivalDate).TotalHours };
                                            }).Select(x => x.hours).Sum() <= (requestModel.GroundedHours ?? 2)
                                        ) : true);
                
                validresult.Result = query.ToList();

                return Ok(validresult);
            }

            return BadRequest(validresult);
        }


        #region private

        /// <summary>
        /// Validate the Filter Request Model
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private ValidationResult IsFilterValid(FilterRequestModel requestModel)
        {
            var validResult = new ValidationResult();

            if (requestModel.GroundedHours <= 0)
                validResult.Invalidate(Constants.FlightFilter.FilterInvalid_GroundedHours_Code, Constants.FlightFilter.FilterInvalid_GroundedHours_Message);

            return validResult;
        }

        #endregion
    }
}
