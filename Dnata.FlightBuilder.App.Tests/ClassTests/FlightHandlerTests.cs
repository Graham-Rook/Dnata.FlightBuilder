using Dnata.FlightBuilder.App.Classes;
using Dnata.FlightBuilder.App.Interfaces;
using Dnata.FlightBuilder.App.Models;
using Dnata.FlightBuilder.App.Shared;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Dnata.FlightBuilder.App.Tests.ClassTests
{
    [TestFixture]
    class FlightHandlerTests
    {
        private class Resources : IDisposable
        {
            public readonly FlightHandler Handler;
            public readonly Mock<IApiHelper> ApiHelper;

            public Resources()
            {
                ApiHelper = new Mock<IApiHelper>();

                Handler = new FlightHandler(ApiHelper.Object);
            }

            public void Dispose()
            { }
        }

        [Test]
        public void FindFlights()
        {
            using var resources = new Resources();
            //Given
            var requestModel = new FilterRequestModel
            {
                DepartureDateStart = DateTime.Now,
                GroundedHours = 2
            };

            var expectedResult = new ValidationResult();

            //setup the mocked api to return a validation result
            resources.ApiHelper.Setup(x =>
                x.Process<ValidationResult>("api/Flight/FindFlights",
                                            It.Is<FilterRequestModel>(m => m.GroundedHours == requestModel.GroundedHours
                                                                            && m.DepartureDateStart == requestModel.DepartureDateStart),
                                            HttpMethod.Post)).Returns(expectedResult);

            //When
            var result = resources.Handler.GetFilteredFlights(requestModel);

            //Then
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);            

            //verify api call was made
            resources.ApiHelper.Verify(x => x.Process<ValidationResult>("api/Flight/FindFlights",
                                            It.Is<FilterRequestModel>(m => m.GroundedHours == requestModel.GroundedHours
                                                                            && m.DepartureDateStart == requestModel.DepartureDateStart),
                                            HttpMethod.Post), Times.Once);

            resources.ApiHelper.VerifyNoOtherCalls();
        }

        [Test]
        public void FindFlights_Invalid()
        {
            using var resources = new Resources();
            //Given
            var requestModel = new FilterRequestModel {
                DepartureDateStart = DateTime.Now,
                GroundedHours = 0
                };

            //setup the mocked api to return an invalid validation result
            var expectedResult = new ValidationResult {
                IsValid = false,
                Messages = new List<ValidationResultMessage> { new ValidationResultMessage { Code = "SomeCode", MessageText = "Some Message" } }
            };

            resources.ApiHelper.Setup(x =>
                x.Process<ValidationResult>("api/Flight/FindFlights",
                                            It.Is<FilterRequestModel>(m => m.GroundedHours == requestModel.GroundedHours
                                                                            && m.DepartureDateStart == requestModel.DepartureDateStart),
                                            HttpMethod.Post)).Returns(expectedResult);

            //When
            var result = resources.Handler.GetFilteredFlights(requestModel);

            //Then
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);

            Assert.AreEqual(expectedResult.Messages, result.Messages);

            //verify api call was made
            resources.ApiHelper.Verify(x => x.Process<ValidationResult>("api/Flight/FindFlights",
                                            It.Is<FilterRequestModel>(m => m.GroundedHours == requestModel.GroundedHours
                                                                            && m.DepartureDateStart == requestModel.DepartureDateStart),
                                            HttpMethod.Post), Times.Once);


            resources.ApiHelper.VerifyNoOtherCalls();
        }

    }
}
