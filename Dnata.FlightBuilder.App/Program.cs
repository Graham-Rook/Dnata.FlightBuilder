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

namespace Dnata.FlightBuilder.App
{
    class Program
    {
        static void Main(string[] args)
        {             
            //setup services and configuration
            var apiHelper = BuildServiceProvider().GetService<IApiHelper>();

            var requestModel = new FilterRequestModel
            {
                DepartureDateStart = DateTime.Now,
                GroundedHours = 2
            };

            Console.WriteLine("Search flights with default criteria (today's date and 2 hours for the grounded range) (Y/N)?");

            var response = Console.ReadLine();

            switch (response.ToLower())
            {
                case "y"://default flight filter search                   
                    ExecuteFlightSearch(apiHelper, requestModel);
                    break;
                case "n"://custom input
                    Console.WriteLine($"Please enter grounded range:");
                    var groundedRange = 0.0;
                    double.TryParse(Console.ReadLine(), out groundedRange);

                    Console.WriteLine($"Please enter date (yyyy-MM-dd):");
                    var enteredDate = DateTime.Now;
                    DateTime.TryParse(Console.ReadLine(), out enteredDate);

                    requestModel.DepartureDateStart = enteredDate;
                    requestModel.GroundedHours = groundedRange;

                    ExecuteFlightSearch(apiHelper, requestModel);
                    break;
                default:
                    Console.WriteLine("Invalid Option");
                    break;
            }
        }

        /// <summary>
        /// Build Service Provider
        /// </summary>
        /// <returns></returns>
        private static IServiceProvider BuildServiceProvider()
        {
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
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Execute Flight Search
        /// </summary>
        /// <param name="apiHelper"></param>
        /// <param name="requestModel"></param>
        private static void ExecuteFlightSearch(IApiHelper apiHelper, FilterRequestModel requestModel)
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

                        if (s + 1 % 2 != 0)
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
