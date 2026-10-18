namespace TicketBooking.Web.Services;

using TicketBooking.Web.Data.Repositories;
using TicketBooking.Web.Models;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventService> _logger;

    public EventService(IEventRepository eventRepository, ILogger<EventService> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<EventCardDto>> GetEventsAsync(
        string? category = null, 
        string? city = null, 
        string? search = null, 
        CancellationToken cancellationToken = default)
    {
        var items = await _eventRepository.GetEventsAsync(category, city, search, cancellationToken);
        return items.Select(e => new EventCardDto
        {
            EventId = e.EventId,
            Title = e.Title,
            Category = e.Category,
            CityName = e.CityName,
            VenueName = e.VenueName,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            BannerUrl = e.BannerUrl,
            StartingPrice = e.StartingPrice,
            TotalRemainingTickets = e.TotalRemainingTickets,
            Status = e.Status
        }).ToList();
    }

    public async Task<EventDetailsDto?> GetEventDetailsAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var item = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (item == null) return null;

        var sessions = await _eventRepository.GetSessionsByEventIdAsync(eventId, cancellationToken);
        var categories = await _eventRepository.GetCategoriesByEventIdAsync(eventId, cancellationToken);

        return new EventDetailsDto
        {
            EventId = item.EventId,
            Title = item.Title,
            Description = item.Description,
            Category = item.Category,
            CityName = item.CityName,
            VenueName = item.VenueName,
            VenueAddress = item.VenueAddress,
            BannerUrl = item.BannerUrl,
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Status = item.Status,
            Sessions = sessions.Select(s => new EventSessionDto
            {
                SessionId = s.SessionId,
                SessionTitle = s.SessionTitle,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            }).ToList(),
            Categories = categories.Select(c => new TicketCategoryDto
            {
                CategoryId = c.CategoryId,
                SessionId = c.SessionId,
                CategoryName = c.CategoryName,
                Price = c.Price,
                TotalCapacity = c.TotalCapacity,
                RemainingCapacity = c.RemainingCapacity,
                MaxPerBooking = c.MaxPerBooking,
                Perks = c.Perks
            }).ToList()
        };
    }

    public async Task<IReadOnlyList<TicketCategoryDto>> GetTicketCategoriesAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var categories = await _eventRepository.GetCategoriesByEventIdAsync(eventId, cancellationToken);
        return categories.Select(c => new TicketCategoryDto
        {
            CategoryId = c.CategoryId,
            SessionId = c.SessionId,
            CategoryName = c.CategoryName,
            Price = c.Price,
            TotalCapacity = c.TotalCapacity,
            RemainingCapacity = c.RemainingCapacity,
            MaxPerBooking = c.MaxPerBooking,
            Perks = c.Perks
        }).ToList();
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _eventRepository.GetDistinctCategoriesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetCitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _eventRepository.GetDistinctCitiesAsync(cancellationToken);
    }

    public async Task<int> CreateEventAsync(CreateEventDto dto, CancellationToken cancellationToken = default)
    {
        var item = new EventItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            CityId = dto.CityId,
            VenueId = dto.VenueId,
            BannerUrl = dto.BannerUrl,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = "Upcoming"
        };

        var id = await _eventRepository.CreateAsync(item, cancellationToken);
        _logger.LogInformation("Created new event ID {EventId} titled {Title}", id, item.Title);
        return id;
    }

    public async Task<bool> UpdateEventAsync(UpdateEventDto dto, CancellationToken cancellationToken = default)
    {
        var item = new EventItem
        {
            EventId = dto.EventId,
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            CityId = dto.CityId,
            VenueId = dto.VenueId,
            BannerUrl = dto.BannerUrl,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status
        };

        var updated = await _eventRepository.UpdateAsync(item, cancellationToken);
        _logger.LogInformation("Updated event ID {EventId}: success = {Success}", dto.EventId, updated);
        return updated;
    }
}
