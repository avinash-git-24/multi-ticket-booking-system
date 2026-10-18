namespace TicketBooking.Web.Data.Repositories;

using TicketBooking.Web.Models;

public interface IEventRepository
{
    Task<EventItem?> GetByIdAsync(int eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventItem>> GetEventsAsync(string? category, string? city, string? search, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketCategory>> GetCategoriesByEventIdAsync(int eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventSession>> GetSessionsByEventIdAsync(int eventId, CancellationToken cancellationToken = default);
    Task<TicketCategory?> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetDistinctCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetDistinctCitiesAsync(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(EventItem item, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(EventItem item, CancellationToken cancellationToken = default);
    Task<int> AddTicketCategoryAsync(TicketCategory category, CancellationToken cancellationToken = default);
}
