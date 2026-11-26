namespace FlightTracker.Models;

public enum FlightStatus
{
    OnTime,
    Delayed,
    Cancelled
}

public class Flight
{
    public required string FlightNumber {get;set;}
    public required string Origin {get;set;}
    public required string Dest {get;set;}
    public DateTime Deptime {get;set;}
    public DateTime Arrtime {get;set;}
    public string? Airplanecode {get;set;}
    public FlightStatus? Status {get;set;}

}