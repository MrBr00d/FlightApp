using System.Data;
using FlightTracker.Models;
using Microsoft.Data.Sqlite;

namespace FlightTracker.Data;

public class SqliteManager
{
    private string _connstring = "Data Source=data.db";
    public void InitializeDatabase()
    {
        Console.WriteLine("Initializing database...");
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection(_connstring);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
        CREATE TABLE IF NOT EXISTS "flightinfo" (
        "id"	        INTEGER NOT NULL UNIQUE,
        "flightnumber"  TEXT NOT NULL,
        "origin"	    TEXT NOT NULL,
        "dest"	        TEXT NOT NULL,
        "deptime"	    TEXT NOT NULL,
        "arrtime"	    TEXT NOT NULL,
        "status"        TEXT,
        "airplanecode"	TEXT,
        PRIMARY KEY("id" AUTOINCREMENT)
        );
        """;
        command.ExecuteNonQuery();

        command.CommandText = """
        CREATE TABLE IF NOT EXISTS "airplanes" (
        id INTEGER NOT NULL UNIQUE,
        name TEXT NOT NULL,
        PRIMARY KEY("id")
        );
        """;
        command.ExecuteNonQuery();
        
        Console.WriteLine("Initialization complete.");
    }

    public List<Flight> GetFlights(int topFlights, FlightType flightType, string airport)
    {
        using var conn = new Microsoft.Data.Sqlite.SqliteConnection(_connstring);
        var cmd = conn.CreateCommand();
        switch(flightType)
        {
            case FlightType.Arrival:
                cmd.CommandText = "SELECT flightnumber, origin, dest, deptime, arrtime, status, airplanecode FROM flightinfo WHERE dest = $airport ORDER BY arrtime LIMIT $topFlights;";
                break;

            case FlightType.Deprture:
                cmd.CommandText = "SELECT flightnumber, origin, dest, deptime, arrtime, status, airplanecode FROM flightinfo WHERE origin = $airport ORDER BY deptime LIMIT $topFlights;";
                break;
        }
        cmd.Parameters.AddWithValue("$topFlights", topFlights);
        cmd.Parameters.AddWithValue("$airport", airport);
        conn.Open();
        var reader = cmd.ExecuteReader();

        List<Flight> flights = []; // Init empty list
        while (reader.Read())
        {
            FlightStatus status; // parse enum first so i have a var i can use later on when i instantiate the flight class
            if (!reader.IsDBNull(5))
            {
                if (Enum.TryParse<FlightStatus>(reader.GetString(5), out var result))
                {
                    status = result;
                }
                else
                {
                    status = FlightStatus.OnTime;
                }
            }
            else
            {
                status = FlightStatus.OnTime;
            }
            
            Flight flight = new Flight
            {
                FlightNumber = reader.GetString(0),
                Origin = reader.GetString(1),
                Dest = reader.GetString(2),
                Deptime = reader.GetDateTime(3),
                Arrtime = reader.GetDateTime(4),
                Status = status,
                Airplanecode = reader.IsDBNull(6) ? null : reader.GetString(6)
            };
            flights.Add(flight);
        }
        return flights;
    }
}