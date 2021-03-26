using Dnata.FlightBuilder.App.Interfaces;
using Dnata.FlightBuilder.App.Models;
using Dnata.FlightBuilder.App.Shared;
using System.Collections.Generic;
using System.Net.Http;

namespace Dnata.FlightBuilder.App.Classes
{
    public class FlightHandler
    {
        private IApiHelper _apiHelper;

        public FlightHandler(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public ValidationResult GetFilteredFlights(FilterRequestModel requestModel)
        {
           return _apiHelper.Process<ValidationResult>("api/Flight/FindFlights", requestModel, HttpMethod.Post);
        }

    }
}
