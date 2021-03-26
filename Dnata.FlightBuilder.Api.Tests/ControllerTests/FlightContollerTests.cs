using Dnata.FlightBuilder.Api.Controllers;
using Dnata.FlightBuilder.Api.Interfaces;
using Dnata.FlightBuilder.Api.Models;
using Dnata.FlightBuilder.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dnata.FlightBuilder.Api.Tests.ControllerTests
{
    [TestFixture]
    class FlightContollerTests
    {
        private class Resources : IDisposable
        {
            public readonly FlightController Controller;
            public readonly Mock<IFlightBuilder> FlightBuilder;


            public Resources()
            {
                FlightBuilder = new Mock<IFlightBuilder>();

                Controller = new FlightController(FlightBuilder.Object);
            }

            public void Dispose()
            { }
        }

        /// <summary>
        /// Test Case for different combinations of the request and days from range
        /// </summary>
        /// <param name="expectedFlights">This will be the expected Flights count to assert against</param>
        /// <param name="expectedSegments">This will be the expected segments count to assert against</param>
        /// <param name="daysFromRange">Setup the days from range</param>
        /// <param name="departureDateDayRange">departure date range</param>
        /// <param name="groundedHours">grounded hours range</param>
        [TestCase(3, 7, 3, 0, 2)]// expect 3 flights; 7 segments; days from range:3 ;  departureDateDayRange: 0 for today; grounded hour range: 2
        [TestCase(0, 0, -3, 0, 2)]//expect 0 flights; 0 segments; days from range:-3 ; departureDateDayRange: 0 for today; grounded hour range: 2    
        [TestCase(5, 12, 3, 0, 3)]//expect 5 flights; 12 segments; days from range:3 ; departureDateDayRange: 0 for today; grounded hour range: 3    
        [TestCase(3, 7, 0, -3, 2)]//expect 3 flights; 7 segments; days from range:0 ; departureDateDayRange: -3 for 3 days ago; grounded hour range: 2    
        public void FindFlights(int expectedFlights, int expectedSegments, int daysFromRange, int departureDateDayRange, double groundedHours)
        {
            using var resources = new Resources();

            //Given
            var daysFromNow = DateTime.Now.AddDays(daysFromRange);

            //setup expected Data to be returned by the mocked interface
            var expectedFlightData = new List<Flight>
            {
                //standard flight with 1 segment
                TestHelper.CreateFlight(daysFromNow, daysFromNow.AddHours(2)),

                //standard flight with 2 segemets
                TestHelper.CreateFlight(daysFromNow, daysFromNow.AddHours(2), daysFromNow.AddHours(3), daysFromNow.AddHours(5)),

                //standard flight with 4 segemets and with the same arrival and departure dates
                TestHelper.CreateFlight(daysFromNow, daysFromNow.AddHours(2), daysFromNow.AddHours(3), daysFromNow.AddHours(5),daysFromNow, daysFromNow.AddHours(2), daysFromNow.AddHours(3), daysFromNow.AddHours(5)),

                // A flight that departs before it arrives
                TestHelper.CreateFlight(daysFromNow, daysFromNow.AddHours(-6)),

                // A flight with 1 segment and more than 2 hours ground time
                TestHelper.CreateFlight(daysFromNow, daysFromNow.AddHours(2), daysFromNow.AddHours(5), daysFromNow.AddHours(6)),

                // Another flight with 2 segemets and more than 2 hours ground time
                TestHelper.CreateFlight(daysFromNow, daysFromNow.AddHours(2), daysFromNow.AddHours(3), daysFromNow.AddHours(4), daysFromNow.AddHours(6), daysFromNow.AddHours(7))
            };

            //setup data to return
            resources.FlightBuilder.Setup(x => x.GetFlights()).Returns(expectedFlightData);


            var requestModel = new FilterRequestModel
            {
                DepartureDateStart = DateTime.Now.AddDays(departureDateDayRange),
                GroundedHours = groundedHours
            };
            

            //When
            var result = resources.Controller.FindFlights(requestModel) as OkObjectResult;

            //Then
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StatusCode == 200);

            //Verify the validation result
            var validResult = result.Value as ValidationResult;
            Assert.IsNotNull(validResult);
            Assert.IsTrue(validResult.IsValid);

            //Flights result
            var flights = validResult.Result as List<Flight>;
            var flightSegments = flights.SelectMany(x => x.Segments).ToList();

            Assert.AreEqual(expectedFlights, flights.Count);
            Assert.AreEqual(expectedSegments, flightSegments.Count);

            //verify the call the the FlightBuilder Interface
            resources.FlightBuilder.Verify(x => x.GetFlights(), Times.Once);
            resources.FlightBuilder.VerifyNoOtherCalls();
        }

        [TestCase(0, Constants.FlightFilter.FilterInvalid_GroundedHours_Code, Constants.FlightFilter.FilterInvalid_GroundedHours_Message)]
        [TestCase(-1, Constants.FlightFilter.FilterInvalid_GroundedHours_Code, Constants.FlightFilter.FilterInvalid_GroundedHours_Message)]
        public void FindFlights_Invalid(int groundedHours, string expectedErrorCode, string expectedErrorMessage)
        {
            using var resources = new Resources();            

            var requestModel = new FilterRequestModel
            {
                DepartureDateStart = DateTime.Now,
                GroundedHours = groundedHours
            };

            //When
            var result = resources.Controller.FindFlights(requestModel) as BadRequestObjectResult;

            //Then
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StatusCode == 400);

            //Verify the validation result
            var validResult = result.Value as ValidationResult;

            Assert.IsNotNull(validResult);
            Assert.IsFalse(validResult.IsValid);

            var validationMessage = validResult.Messages[0];
            Assert.AreEqual(expectedErrorCode, validationMessage.Code);
            Assert.AreEqual(expectedErrorMessage, validationMessage.MessageText);

            //verify the call to the FlightBuilder was not made
            resources.FlightBuilder.Verify(x => x.GetFlights(), Times.Never);
            resources.FlightBuilder.VerifyNoOtherCalls();
        }
    }
}
