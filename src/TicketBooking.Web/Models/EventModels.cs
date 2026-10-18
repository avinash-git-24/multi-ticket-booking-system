namespace TicketBooking.Web.Models;

public class Venue
{
    public int VenueId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
    public string CityName { get; set; } = string.Empty;
}

public class EventItem
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int CityId { get; set; }
    public int? VenueId { get; set; }
    public string? BannerUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Upcoming";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    // Flattened relational fields
    public string CityName { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
    public string VenueAddress { get; set; } = string.Empty;
    public decimal StartingPrice { get; set; }
    public int TotalRemainingTickets { get; set; }
}

public class EventSession
{
    public int SessionId { get; set; }
    public int EventId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive { get; set; } = true;
}

public class TicketCategory
{
    public int CategoryId { get; set; }
    public int EventId { get; set; }
    public int? SessionId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int TotalCapacity { get; set; }
    public int RemainingCapacity { get; set; }
    public int MaxPerBooking { get; set; } = 10;
    public string? Perks { get; set; }
}

public class EventCardDto
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? BannerUrl { get; set; }
    public decimal StartingPrice { get; set; }
    public int TotalRemainingTickets { get; set; }
    public string Status { get; set; } = "Upcoming";
}

public class EventDetailsDto
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
    public string VenueAddress { get; set; } = string.Empty;
    public string? BannerUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Upcoming";
    public IReadOnlyList<EventSessionDto> Sessions { get; set; } = [];
    public IReadOnlyList<TicketCategoryDto> Categories { get; set; } = [];
}

public class EventSessionDto
{
    public int SessionId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class TicketCategoryDto
{
    public int CategoryId { get; set; }
    public int? SessionId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int TotalCapacity { get; set; }
    public int RemainingCapacity { get; set; }
    public int MaxPerBooking { get; set; } = 10;
    public string? Perks { get; set; }
}

public class CreateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int CityId { get; set; }
    public int? VenueId { get; set; }
    public string? BannerUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class UpdateEventDto
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int CityId { get; set; }
    public int? VenueId { get; set; }
    public string? BannerUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Upcoming";
}
