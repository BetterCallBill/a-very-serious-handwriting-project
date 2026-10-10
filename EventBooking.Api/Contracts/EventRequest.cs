namespace EventBooking.Api.Contracts;

public class EventRequest
{
    public string? Title { get; set; }
    public string? Location { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public int Capacity { get; set; }
}