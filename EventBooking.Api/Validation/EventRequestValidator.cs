using EventBooking.Api.Contracts;

namespace EventBooking.Api.Validation;

public static class EventRequestValidator
{
    public static string? Validate(EventRequest request, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return "Title must contain at least one non-whitespace character.";
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            return "Location must contain at least one non-whitespace character.";
        }

        if (request.Capacity <= 0)
        {
            return "Event capacity must be greater than 0.";
        }

        if (request.StartTime <= now)
        {
            return "Start Time must be a future time.";
        }

        return null;
    }
}
