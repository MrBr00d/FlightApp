using FlightTracker.Data;
using FlightTracker.Models;

SqliteManager mgr = new();
mgr.InitializeDatabase();

// Accept input airport
Console.WriteLine("Enter 3 character airport code");
string currentArport = Console.ReadLine();
if (currentArport == null)
{
    throw new ArgumentException("Enter an airportcode");
}

Console.Clear();

FlightType flightType = FlightType.Deprture;
while (true)
{
    Console.WriteLine("===================================================");
    Console.WriteLine($"{flightType.ToString()} flights for {currentArport}");
    Console.WriteLine("===================================================");

    List<Flight> flights = mgr.GetFlights(topFlights: 5, flightType: flightType, airport: currentArport);
    if (flightType == FlightType.Deprture)
    {
        Console.WriteLine("Flightnumber \t Destination \t Departure time \t Status");
        foreach (Flight flight in flights)
        {
            Console.WriteLine($"{flight.FlightNumber} \t\t {flight.Dest} \t\t {flight.Deptime} \t {flight.Status}");
        }
    }
    else // FlightType.Arrival
    {
        Console.WriteLine("Flightnumber \t Origin \t Arrival time \t\t Status");
        foreach (Flight flight in flights)
        {
            Console.WriteLine($"{flight.FlightNumber} \t\t {flight.Origin} \t\t {flight.Arrtime} \t {flight.Status}");
        }
    }

    Thread.Sleep(5000);
    flightType = Toggle(flightType);
    Console.Clear();
}

static FlightType Toggle(FlightType current) =>
    current == FlightType.Deprture ? FlightType.Arrival : FlightType.Deprture;