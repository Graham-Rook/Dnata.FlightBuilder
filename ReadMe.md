## Project Description
Dnata Flight Search Console application with API
In the solution there is:
1.	An API project, this implements all the logic for searching the flights with the required rules indicated in the test.
	- Running this project will open the web browser with a swagger doc exposing the endpoint for testing purposes.
2.	An API Test Project – This includes all the tests for the API.
3.	A Console App, this app connects to the API at runtime to retrieve the search results.
4.	An App Test Project – This includes the tests for the console app.

## Prerequisites:
1. .net Framework 5.0 is required to run the project (the latest version of Visual Studio includes this)

## Executing the code
1. Clone the repo
2. Build the solution
3. Start the project 

PS: I have setup the solution configuration to run as multiple start up projects so that both the API and console app start up.  
If this was not ported across in the settings for any reason, then the console app may fail trying to connect to the API, it just means we must start the APi before running the console app.

## Assumptions
I made the assumption that perhaps we may want to filter flights by altering the criteria, so I included a filter request model that will allow us to change the Date and the grounded hours range.
-	Grounded Hours will default to 2 hours if null is provided.
-	Date will default to today’s date if null is provided.
 
- I also included validation that if the grounded range is 0 or less then it will return a failed result.
 
- I imagine we could get quite creative regarding the request filter, and implement a generic filter interface if we wanted to perform some sort of dynamic filtering at runtime.
- I also made the original FlightBuilder class (renamed to FlightBuilderHelper) implement an interface so that it could also have additional implementations if we wanted to make it more generic.
