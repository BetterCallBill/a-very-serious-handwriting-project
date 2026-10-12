namespace EventBooking.Api.Services;

public class EventService
{
    private readonly List<Event> _events = [
        new Event {
            Id = Guid.NewGuid(),
            Title = "aaa",
            StartTime = DateTime.Now,
            Location = "syd",
            Capacity = 10
        },
        new Event {
            Id = Guid.NewGuid(),
            Title = "bbb",
            StartTime = DateTime.Now,
            Location = "syd",
            Capacity = 99
        }
    ];

    public IEnumerable<Event> GetAll()
    {
        return [.. _events];
    }

    public Event? GetById(Guid id)
    {
        var existingEvent = _events.Find(x => x.Id == id);

        return existingEvent;
    }
}
