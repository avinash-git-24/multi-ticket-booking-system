namespace TicketBooking.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using TicketBooking.Web.Services;

public class MoviesController : Controller
{
    private readonly IMovieService _movieService;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
    {
        _movieService = movieService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? genre, string? language, string? status = "CurrentlyShowing", string? search = null, string? city = null)
    {
        var targetCity = city ?? Request.Cookies["SelectedCity"] ?? "Rajkot";

        var viewModel = await _movieService.GetMovieListAsync(
            targetCity, status, genre, language, search);

        return View(viewModel);
    }

    [HttpGet]
    [Route("movies/details/{id:int}")]
    [Route("movies/{id:int}")]
    public async Task<IActionResult> Details(int id, DateTime? date = null, string? city = null)
    {
        var targetCity = city ?? Request.Cookies["SelectedCity"] ?? "Rajkot";

        var viewModel = await _movieService.GetMovieDetailsAsync(id, date, targetCity);
        if (viewModel == null)
        {
            return NotFound();
        }

        return View(viewModel);
    }
}
