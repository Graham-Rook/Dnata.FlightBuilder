using Dnata.FlightBuilder.App.Classes;
using Dnata.FlightBuilder.App.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
