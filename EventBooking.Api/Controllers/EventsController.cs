using EventBooking.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    // REVIEW: This shared list is temporary in-memory storage, not safe concurrent storage.
    // readonly prevents replacing the list reference; it does not protect reads or writes.
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
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new
            {
                Error = "Title must contain at least one non-whitespace character."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            return BadRequest(new
            {
                Error = "Location must contain at least one non-whitespace character."
            });
        }

        if (request.Capacity <= 0)
        {
            return BadRequest(new
            {
                Error = "Event capcity must greater than 0."
            });
        }

        // REVIEW: Use one time convention in the model, sample data, and both write actions.
        // Consider DateTimeOffset with explicit offsets and comparison against UTC now;
        // comparing DateTime clock values with different kinds can misclassify future times.
        if (request.StartTime <= DateTime.UtcNow)
        {
            return BadRequest(new
            {
                Error = "Start Time must be a future time."
            });
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

    // REVIEW: Target PUT /events/{id}: add a route template and a matching Guid parameter.
    // Look up the event using the route ID. If the body keeps an ID, define how a mismatch
    // is rejected; do not let a conflicting body ID select a different event.
    [HttpPut("{id:guid}")]
    public ActionResult<Event> UpdateEvent(Guid id, [FromBody] EventRequest request)
    {
        var existingEvent = Events.FirstOrDefault(x => x.Id == id);

        // REVIEW: This guard correctly prevents dereferencing a missing event.
        // A missing resource needs 404 Not Found, not 400 Bad Request.
        if (existingEvent is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new
            {
                Error = "Title must contain at least one non-whitespace character."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            return BadRequest(new
            {
                Error = "Location must contain at least one non-whitespace character."
            });
        }

        if (request.Capacity <= 0)
        {
            return BadRequest(new
            {
                Error = "Event capcity must greater than 0."
            });
        }

        // REVIEW: Use one time convention in the model, sample data, and both write actions.
        // Consider DateTimeOffset with explicit offsets and comparison against UTC now;
        // comparing DateTime clock values with different kinds can misclassify future times.
        if (request.StartTime <= DateTimeOffset.UtcNow)
        {
            return BadRequest(new
            {
                Error = "Start Time must be a future time."
            });
        }

        // The route ID identifies the event. The body ID is ignored.
        // All validation finishes before we change the stored object.
        existingEvent.Title = request.Title.Trim();
        existingEvent.Location = request.Location.Trim();
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