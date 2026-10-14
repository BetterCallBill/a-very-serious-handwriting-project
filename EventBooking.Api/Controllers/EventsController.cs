using EventBooking.Api.Contracts;
using EventBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    private readonly EventService _eventService;

    public EventsController(EventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet(Name = "Get Events")]
    public IEnumerable<Event> Get()
    {
        return _eventService.GetAll();
    }

    [HttpGet("{id}", Name = "Get Event")]
    public ActionResult<Event> GetEventById(Guid id)
    {
        var result = _eventService.GetById(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost(Name = "Create Event")]
    public ActionResult<Event> Post([FromBody] EventRequest request)
    {
        var (createdEvent, error) = _eventService.Create(request, DateTimeOffset.UtcNow);

        if (error is not null)
        {
            return BadRequest(new { Error = error });
        }

        return CreatedAtAction(
            nameof(GetEventById),
            new { id = createdEvent!.Id },
            createdEvent);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<Event> UpdateEvent(Guid id, [FromBody] EventRequest request)
    {
        var (updatedEvent, error) = _eventService.Update(id, request, DateTimeOffset.UtcNow);

        if (error is not null)
        {
            return BadRequest(new { Error = error });
        }

        if (updatedEvent is null)
        {
            return NotFound();
        }

        return Ok(updatedEvent);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEvent(Guid id)
    {
        if (!_eventService.Delete(id))
        {
            return NotFound();
        }

        return NoContent();
    }
}
