namespace TicketBooking.Web.Services;

using TicketBooking.Web.Data.Repositories;
using TicketBooking.Web.Models;
using TicketBooking.Web.ViewModels.Booking;
using TicketBooking.Web.ViewModels.Movies;

public interface IMovieService
{
    Task<MovieListViewModel> GetMovieListAsync(
        string? city, string? status, string? genre, string? language, string? search, CancellationToken cancellationToken = default);
    Task<MovieDetailsViewModel?> GetMovieDetailsAsync(
        int movieId, DateTime? date, string? city, CancellationToken cancellationToken = default);
    Task<SeatMapViewModel?> GetSeatMapAsync(int showtimeId, CancellationToken cancellationToken = default);
}

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IShowtimeRepository _showtimeRepository;
    private readonly ILogger<MovieService> _logger;

    public MovieService(
        IMovieRepository movieRepository,
        IShowtimeRepository showtimeRepository,
        ILogger<MovieService> logger)
    {
        _movieRepository = movieRepository;
        _showtimeRepository = showtimeRepository;
        _logger = logger;
    }

    public async Task<MovieListViewModel> GetMovieListAsync(
        string? city, string? status, string? genre, string? language, string? search, CancellationToken cancellationToken = default)
    {
        var targetCity = string.IsNullOrWhiteSpace(city) ? "Rajkot" : city.Trim();
        var targetStatus = string.IsNullOrWhiteSpace(status) ? "CurrentlyShowing" : status.Trim();

        var movies = await _movieRepository.GetMoviesAsync(
            targetCity, targetStatus, genre, language, search, cancellationToken);

        var genres = await _movieRepository.GetDistinctGenresAsync(cancellationToken);
        var languages = await _movieRepository.GetDistinctLanguagesAsync(cancellationToken);
        var cities = await _movieRepository.GetAllCitiesAsync(cancellationToken);

        return new MovieListViewModel
        {
            Movies = movies,
            SelectedCity = targetCity,
            SelectedGenre = genre,
            SelectedLanguage = language,
            SelectedStatus = targetStatus,
            SearchQuery = search,
            AvailableGenres = genres,
            AvailableLanguages = languages,
            AvailableCities = cities
        };
    }

    public async Task<MovieDetailsViewModel?> GetMovieDetailsAsync(
        int movieId, DateTime? date, string? city, CancellationToken cancellationToken = default)
    {
        var movie = await _movieRepository.GetByIdAsync(movieId, cancellationToken);
        if (movie == null) return null;

        var targetCity = string.IsNullOrWhiteSpace(city) ? "Rajkot" : city.Trim();
        var targetDate = date?.Date ?? DateTime.Today;

        // Build 7-day schedule strip
        var availableDates = new List<DateTime>();
        for (int i = 0; i < 7; i++)
        {
            availableDates.Add(DateTime.Today.AddDays(i));
        }

        var showtimes = await _showtimeRepository.GetShowtimesByMovieAndDateAsync(
            movieId, targetDate, targetCity, cancellationToken);

        var groupedTheatres = showtimes
            .GroupBy(s => new { s.TheatreId, s.TheatreName, s.TheatreAddress })
            .Select(g => new TheatreShowtimesGroup
            {
                TheatreId = g.Key.TheatreId,
                TheatreName = g.Key.TheatreName ?? "Multiplex",
                Address = g.Key.TheatreAddress ?? targetCity,
                Slots = g.Select(s => new ShowtimeSlotDto
                {
                    ShowtimeId = s.ShowtimeId,
                    StartTime = s.StartTime,
                    ScreenName = s.ScreenName ?? "Screen 1",
                    ScreenType = s.ScreenType ?? "2D",
                    LowestPrice = s.SilverPrice
                }).OrderBy(slot => slot.StartTime).ToList()
            })
            .OrderBy(t => t.TheatreName)
            .ToList();

        return new MovieDetailsViewModel
        {
            Movie = movie,
            SelectedCity = targetCity,
            SelectedDate = targetDate,
            AvailableDates = availableDates,
            Theatres = groupedTheatres
        };
    }

    public async Task<SeatMapViewModel?> GetSeatMapAsync(int showtimeId, CancellationToken cancellationToken = default)
    {
        var showtime = await _showtimeRepository.GetByIdAsync(showtimeId, cancellationToken);
        if (showtime == null) return null;

        var movie = await _movieRepository.GetByIdAsync(showtime.MovieId, cancellationToken);
        var seats = await _showtimeRepository.GetSeatMatrixWithStatusAsync(showtimeId, cancellationToken);

        var rowsGrouped = seats
            .GroupBy(s => s.RowLabel)
            .OrderBy(g => g.Key)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(s => s.SeatNumber).ToList()
            );

        return new SeatMapViewModel
        {
            ShowtimeId = showtimeId,
            MovieTitle = showtime.MovieTitle ?? (movie?.Title ?? "Movie"),
            Certificate = movie?.Certificate ?? "UA",
            Language = movie?.Language ?? "Hindi",
            TheatreName = showtime.TheatreName ?? "Multiplex",
            ScreenName = showtime.ScreenName ?? "Audi 1",
            ScreenType = showtime.ScreenType ?? "2D",
            ShowDateTime = showtime.StartTime,
            SilverPrice = showtime.SilverPrice,
            GoldPrice = showtime.GoldPrice,
            PlatinumPrice = showtime.PlatinumPrice,
            RowsGrouped = rowsGrouped
        };
    }
}
