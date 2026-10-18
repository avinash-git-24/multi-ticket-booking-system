namespace TicketBooking.Web.Controllers;

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TicketBooking.Web.Models;
using TicketBooking.Web.Services;
using TicketBooking.Web.ViewModels.Home;

public class HomeController : Controller
{
    private readonly IMovieService _movieService;
    private readonly IEventService _eventService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IMovieService movieService,
        IEventService eventService,
        ILogger<HomeController> logger)
    {
        _movieService = movieService;
        _eventService = eventService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var city = Request.Cookies["SelectedCity"] ?? "Rajkot";

        // Fetch dynamic movies from database via MovieService
        var showingResult = await _movieService.GetMovieListAsync(city: null, status: "CurrentlyShowing", genre: null, language: null, search: null);
        var upcomingResult = await _movieService.GetMovieListAsync(city: null, status: "Upcoming", genre: null, language: null, search: null);
        var allMovies = showingResult.Movies.Concat(upcomingResult.Movies).DistinctBy(m => m.MovieId).ToList();

        // Pin 10 BookMyShow Rajkot movies (MovieId 13 to 22) first, followed by previous movies
        allMovies = allMovies
            .OrderBy(m => m.MovieId >= 13 && m.MovieId <= 22 ? m.MovieId - 12 : 100 + m.MovieId)
            .ToList();

        foreach (var movie in allMovies)
        {
            switch (movie.MovieId)
            {
                case 13:
                    movie.LikesDisplay = "189K+ Likes";
                    break;
                case 14:
                    movie.RatingDisplay = "8.8/10";
                    movie.VotesDisplay = "133K+ Votes";
                    break;
                case 15:
                    movie.RatingDisplay = "7.3/10";
                    movie.VotesDisplay = "3.4K+ Votes";
                    break;
                case 16:
                    movie.RatingDisplay = "9.2/10";
                    movie.VotesDisplay = "4.3K+ Votes";
                    break;
                case 17:
                    movie.RatingDisplay = "7.3/10";
                    movie.VotesDisplay = "2K+ Votes";
                    break;
                case 18:
                    movie.RatingDisplay = "9.6/10";
                    movie.VotesDisplay = "295K+ Votes";
                    break;
                case 19:
                    movie.RatingDisplay = "9/10";
                    movie.VotesDisplay = "18.5K+ Votes";
                    break;
                case 20:
                    movie.RatingDisplay = "8.5/10";
                    movie.VotesDisplay = "45.4K+ Votes";
                    break;
                case 21:
                    movie.RatingDisplay = "9.5/10";
                    movie.VotesDisplay = "2.2K+ Votes";
                    break;
                case 22:
                    movie.RatingDisplay = "8.2/10";
                    movie.VotesDisplay = "390+ Votes";
                    break;
                case 1:
                    movie.RatingDisplay = "8.4/10";
                    movie.VotesDisplay = "142K+ Votes";
                    break;
                case 2:
                    movie.RatingDisplay = "9.4/10";
                    movie.VotesDisplay = "38K+ Votes";
                    break;
                case 3:
                    movie.RatingDisplay = "8.7/10";
                    movie.VotesDisplay = "210K+ Votes";
                    break;
                case 4:
                    movie.RatingDisplay = "8.6/10";
                    movie.VotesDisplay = "315K+ Votes";
                    break;
                case 5:
                    movie.RatingDisplay = "8.1/10";
                    movie.VotesDisplay = "120K+ Votes";
                    break;
                case 6:
                    movie.LikesDisplay = "450K+ Likes";
                    break;
                default:
                    movie.RatingDisplay = "8.5/10";
                    movie.VotesDisplay = "10K+ Votes";
                    break;
            }
        }

        // Fetch dynamic events from database via EventService
        var allEvents = await _eventService.GetEventsAsync();

        var viewModel = new HomeIndexViewModel
        {
            RecommendedMovies = allMovies.Count > 0 ? allMovies : showingResult.Movies,
            PremiereMovies = upcomingResult.Movies.Count > 0 ? upcomingResult.Movies : showingResult.Movies.Take(4).ToList(),
            MusicEvents = allEvents.Where(e => e.Category.Equals("Concert", StringComparison.OrdinalIgnoreCase)).ToList(),
            ComedyEvents = allEvents.Where(e => e.Category.Equals("Comedy", StringComparison.OrdinalIgnoreCase)).ToList(),
            SportsEvents = allEvents.Where(e => e.Category.Equals("Sports", StringComparison.OrdinalIgnoreCase)).ToList(),
            TheatreEvents = allEvents.Where(e => e.Category.Equals("Theatre", StringComparison.OrdinalIgnoreCase)).ToList(),
            WorkshopEvents = allEvents.Where(e => e.Category.Equals("Workshop", StringComparison.OrdinalIgnoreCase)).ToList(),
            PopularEvents = allEvents.OrderByDescending(e => e.StartingPrice).Take(6).ToList(),
            OutdoorEvents = allEvents.Where(e => e.Category.Equals("Concert", StringComparison.OrdinalIgnoreCase) || e.Category.Equals("Sports", StringComparison.OrdinalIgnoreCase)).Take(6).ToList(),
            SelectedCity = city
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
