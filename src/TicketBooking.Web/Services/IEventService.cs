namespace TicketBooking.Web.Services;

using TicketBooking.Web.Models;

public interface IEventService
{
    Task<IReadOnlyList<EventCardDto>> GetEventsAsync(string? category = null, string? city = null, string? search = null, CancellationToken cancellationToken = default);
    Task<EventDetailsDto?> GetEventDetailsAsync(int eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketCategoryDto>> GetTicketCategoriesAsync(int eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetCitiesAsync(CancellationToken cancellationToken = default);
    Task<int> CreateEventAsync(CreateEventDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateEventAsync(UpdateEventDto dto, CancellationToken cancellationToken = default);
}
