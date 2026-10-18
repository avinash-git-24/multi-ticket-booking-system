namespace TicketBooking.Web.ViewModels.Movies;

using TicketBooking.Web.Models;

public class MovieListViewModel
{
    public IReadOnlyList<Movie> Movies { get; set; } = Array.Empty<Movie>();
    public string SelectedCity { get; set; } = "Rajkot";
    public string? SelectedGenre { get; set; }
    public string? SelectedLanguage { get; set; }
    public string? SelectedStatus { get; set; } = "CurrentlyShowing";
    public string? SearchQuery { get; set; }

    public IReadOnlyList<string> AvailableGenres { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> AvailableLanguages { get; set; } = Array.Empty<string>();
    public IReadOnlyList<City> AvailableCities { get; set; } = Array.Empty<City>();
}

public class MovieDetailsViewModel
{
    public Movie Movie { get; set; } = new();
    public string SelectedCity { get; set; } = "Rajkot";
    public DateTime SelectedDate { get; set; } = DateTime.Today;
    public List<DateTime> AvailableDates { get; set; } = new();
    public List<TheatreShowtimesGroup> Theatres { get; set; } = new();
}

public class TheatreShowtimesGroup
{
    public int TheatreId { get; set; }
    public string TheatreName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<ShowtimeSlotDto> Slots { get; set; } = new();
}

public class ShowtimeSlotDto
{
    public int ShowtimeId { get; set; }
    public DateTime StartTime { get; set; }
    public string ScreenName { get; set; } = string.Empty;
    public string ScreenType { get; set; } = "2D";
    public decimal LowestPrice { get; set; }
}
