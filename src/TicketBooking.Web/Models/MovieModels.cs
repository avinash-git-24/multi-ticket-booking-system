namespace TicketBooking.Web.Models;

public class Movie
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string Certificate { get; set; } = "UA";
    public string? PosterUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public string Status { get; set; } = "CurrentlyShowing";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Presentation helpers for BookMyShow UI
    public string? RatingDisplay { get; set; }
    public string? VotesDisplay { get; set; }
    public string? LikesDisplay { get; set; }
}

public class Theatre
{
    public int TheatreId { get; set; }
    public string TheatreName { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string? CityName { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Screen> Screens { get; set; } = new();
}

public class Screen
{
    public int ScreenId { get; set; }
    public int TheatreId { get; set; }
    public string? TheatreName { get; set; }
    public string ScreenName { get; set; } = string.Empty;
    public string ScreenType { get; set; } = "2D";
    public int TotalSeats { get; set; } = 60;
    public bool IsActive { get; set; } = true;
}

public class Seat
{
    public int SeatId { get; set; }
    public int ScreenId { get; set; }
    public string RowLabel { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public string SeatTier { get; set; } = "Gold";
    public bool IsActive { get; set; } = true;
}

public class Showtime
{
    public int ShowtimeId { get; set; }
    public int MovieId { get; set; }
    public string? MovieTitle { get; set; }
    public int ScreenId { get; set; }
    public string? ScreenName { get; set; }
    public string? ScreenType { get; set; }
    public int TheatreId { get; set; }
    public string? TheatreName { get; set; }
    public string? TheatreAddress { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal SilverPrice { get; set; } = 150.00m;
    public decimal GoldPrice { get; set; } = 220.00m;
    public decimal PlatinumPrice { get; set; } = 300.00m;
    public bool IsActive { get; set; } = true;
}

public class ShowtimeSeat
{
    public long ShowtimeSeatId { get; set; }
    public int ShowtimeId { get; set; }
    public int SeatId { get; set; }
    public string RowLabel { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public string SeatTier { get; set; } = "Gold";
    public string Status { get; set; } = "Available";
    public DateTime? HoldExpiresAtUtc { get; set; }
    public int? BookingId { get; set; }
    public decimal Price { get; set; }
}
