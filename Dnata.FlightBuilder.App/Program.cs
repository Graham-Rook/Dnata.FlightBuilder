using Dnata.FlightBuilder.App.Classes;
using Dnata.FlightBuilder.App.Helpers;
using Dnata.FlightBuilder.App.Interfaces;
using Dnata.FlightBuilder.App.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Dnata.FlightBuilder.App
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Setup
            //setup configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            //setup dependency injection
            var services = new ServiceCollection();
            //add http client
            services.AddHttpClient<IApiHelper, ApiHelper>(client =>
            {
                client.BaseAddress = new Uri(config["BaseAddress"]);
            }).AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(300)));

            //setup service provider
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            var apiHelper = serviceProvider.GetService<IApiHelper>();
            #endregion

            Console.WriteLine("Search flights with default criteria (today's date and 2 hours for the grounded range) (Y/N)?");

            var response = Console.ReadLine();

            switch (response.ToLower())
            {
                case "y"://default flight filter search
                    var defaultModel = new FilterRequestModel
                    {
                        DepartureDateStart = DateTime.Now,
                        GroundedHours = 2
                    };
                    ExecuteFlights(apiHelper, defaultModel);
                    break;
                case "n"://custom input

                    Console.WriteLine($"Please enter grounded range:");
                    var groundedRange = 0.0;
                    double.TryParse(Console.ReadLine(), out groundedRange);

                    Console.WriteLine($"Please enter date (yyyy-MM-dd):");
                    var enteredDate = DateTime.Now;
                    DateTime.TryParse(Console.ReadLine(), out enteredDate);

                    var model = new FilterRequestModel
                    {
                        DepartureDateStart = enteredDate,
                        GroundedHours = groundedRange
                    };
                    ExecuteFlights(apiHelper, model);
                    break;
                default:
                    Console.WriteLine("Invalid Option");
                    break;
            }
        }

        /// <summary>
        /// Execute Flights
        /// </summary>
        /// <param name="apiHelper"></param>
        /// <param name="requestModel"></param>
        private static void ExecuteFlights(IApiHelper apiHelper, FilterRequestModel requestModel)
        {
            var flightHandler = new FlightHandler(apiHelper);

            var flightResult = flightHandler.GetFilteredFlights(requestModel);

            if (flightResult.IsValid)
            {
                var flights = JsonConvert.DeserializeObject<List<Flight>>(flightResult.Result.ToString());
                Console.WriteLine("--------------------------------------");
                Console.WriteLine($"{flights.Count} flight(s) found....");

                for (int i = 0; i < flights.Count; i++)
                {
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine($"Flight {i + 1} Details:");

                    var segments = flights[i].Segments;
                    for (int s = 0; s < segments.Count; s++)
                    {
                        Console.WriteLine("======================================");
                        Console.WriteLine($"Departure: {segments[s].DepartureDate.ToString("yyyy-MM-dd :HH:mm")}");
                        Console.WriteLine($"Arrival: {segments[s].ArrivalDate.ToString("yyyy-MM-dd :HH:mm")}");
                        Console.WriteLine("======================================");

                        if (s+1 % 2 != 0)
                        {
                            Console.WriteLine("            __|__            ");
                            Console.WriteLine("     --@--@--(_)--@--@--     ");
                        }

                    }
                   
                }
            }
            else
            {
                Console.WriteLine("Invalid Flight Search.....");
                flightResult.Messages.ForEach(m =>
                {
                    Console.WriteLine(m.MessageText);
                });

            }
        }
    }
}
