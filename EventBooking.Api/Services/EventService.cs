using EventBooking.Api.Contracts;
using EventBooking.Api.Validation;

namespace EventBooking.Api.Services;

public class EventService
{
    private readonly object _syncRoot = new();

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
        lock (_syncRoot)
        {
            return _events.Select(Copy).ToArray();
        }
    }

    public Event? GetById(Guid id)
    {
        lock (_syncRoot)
        {
            var existingEvent = _events.Find(x => x.Id == id);
            return existingEvent is null ? null : Copy(existingEvent);
        }
    }

    public bool Delete(Guid id)
    {
        lock (_syncRoot)
        {
            var existingEvent = _events.Find(x => x.Id == id);

            if (existingEvent is null)
            {
                return false;
            }

            return _events.Remove(existingEvent);
        }
    }

    public (Event? CreatedEvent, string? Error) Create(EventRequest request, DateTimeOffset now)
    {
        var validate = EventRequestValidator.Validate(request, now);

        if (validate != null)
        {
            return (null, validate);
        }

        var createdEvent = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title!.Trim(),
            Location = request.Location!.Trim(),
            StartTime = request.StartTime,
            Capacity = request.Capacity
        };

        lock (_syncRoot)
        {
            _events.Add(createdEvent);
            return (Copy(createdEvent), null);
        }
    }

    public (Event? UpdatedEvent, string? Error) Update(Guid id, EventRequest request, DateTimeOffset now)
    {
        lock (_syncRoot)
        {
            var existingEvent = _events.Find(x => x.Id == id);

            if (existingEvent is null)
            {
                return (null, null);
            }

            var validate = EventRequestValidator.Validate(request, now);

            if (validate != null)
            {
                return (null, validate);
            }

            existingEvent.Title = request.Title!.Trim();
            existingEvent.Location = request.Location!.Trim();
            existingEvent.StartTime = request.StartTime;
            existingEvent.Capacity = request.Capacity;

            return (Copy(existingEvent), null);
        }
    }

    private static Event Copy(Event source)
    {
        return new Event
        {
            Id = source.Id,
            Title = source.Title,
            Location = source.Location,
            StartTime = source.StartTime,
            Capacity = source.Capacity
        };
    }
}
