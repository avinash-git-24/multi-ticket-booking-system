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
