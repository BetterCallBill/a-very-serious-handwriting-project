namespace EventBooking.Api;

public class Event
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Location { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public int Capacity { get; set; }
}
