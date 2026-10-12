using EventBooking.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using EventBooking.Api.Validation;

namespace EventBooking.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    private static readonly List<Event> Events =
    [
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

    [HttpGet(Name = "Get Events")]
    public IEnumerable<Event> Get()
    {
        return Events;
    }

    [HttpGet("{id}", Name = "Get Event")]
    public ActionResult<Event> GetEventById(Guid id)
    {
        var result = Events.FirstOrDefault(x => x.Id == id);

        if (result != null)
            return result;

        return NotFound();
    }

    [HttpPost(Name = "Create Event")]
    public ActionResult<Event> Post([FromBody] EventRequest request)
    {
        var validate = EventRequestValidator.Validate(request, DateTimeOffset.Now);

        if (validate != null)
        {
            return BadRequest(new { Error = validate});
        }

        var newEvent = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title!.Trim(),
            Location = request.Location!.Trim(),
            StartTime = request.StartTime,
            Capacity = request.Capacity
        };

        Events.Add(newEvent);

        return CreatedAtAction(
            nameof(GetEventById),
            new { id = newEvent.Id },
            newEvent);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<Event> UpdateEvent(Guid id, [FromBody] EventRequest request)
    {
        var existingEvent = Events.FirstOrDefault(x => x.Id == id);

        if (existingEvent is null)
        {
            return NotFound();
        }

        var error = EventRequestValidator.Validate(request, DateTimeOffset.Now);

        if (error is not null)
        {
            return BadRequest(new { Error = error});
        }

        existingEvent.Title = request.Title!.Trim();
        existingEvent.Location = request.Location!.Trim();
        existingEvent.StartTime = request.StartTime;
        existingEvent.Capacity = request.Capacity;

        return Ok(existingEvent);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEvent(Guid id)
    {
        var existingEvent = Events.FirstOrDefault(e => e.Id == id);

        if (existingEvent is null)
        {
            return NotFound();
        }

        Events.Remove(existingEvent);

        return NoContent();
    }
}
