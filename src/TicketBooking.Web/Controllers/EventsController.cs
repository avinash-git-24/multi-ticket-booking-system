namespace TicketBooking.Web.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketBooking.Web.Models;
using TicketBooking.Web.Services;
using TicketBooking.Web.ViewModels.Events;

public class EventsController : Controller
{
    private readonly IEventService _eventService;
    private readonly IBookingService _bookingService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(
        IEventService eventService,
        IBookingService bookingService,
        ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? category, string? city, string? search)
    {
        var dbCategory = category switch
        {
            "Music Shows" => "Concert",
            "Performances" => "Theatre",
            "Comedy Shows" => "Comedy",
            "Workshops" => "Workshop",
            _ => category
        };

        var events = await _eventService.GetEventsAsync(dbCategory, city, search);
        var categories = await _eventService.GetCategoriesAsync();
        var cities = await _eventService.GetCitiesAsync();

        var viewModel = new EventListViewModel
        {
            Events = events,
            SelectedCategory = category,
            SelectedCity = city,
            SearchQuery = search,
            Categories = categories,
            Cities = cities
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var eventDetails = await _eventService.GetEventDetailsAsync(id);
        if (eventDetails == null)
        {
            return NotFound();
        }

        var viewModel = new EventDetailsViewModel
        {
            Event = eventDetails,
            SelectedCategoryId = eventDetails.Categories.FirstOrDefault()?.CategoryId ?? 0,
            SelectedQuantity = 1,
            SelectedSessionId = eventDetails.Sessions.FirstOrDefault()?.SessionId
        };

        if (TempData["ErrorMessage"] != null)
        {
            viewModel.ErrorMessage = TempData["ErrorMessage"]?.ToString();
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HoldTickets(HoldEventTicketsRequest request)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToAction("Login", "Account", new { returnUrl = $"/events/details/{request.EventId}" });
        }

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please select a valid ticket tier and quantity.";
            return RedirectToAction(nameof(Details), new { id = request.EventId });
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var (success, error, bookingRef, _) = await _bookingService.HoldEventTicketsAsync(
            userId, request.EventId, request.CategoryId, request.Quantity);

        if (!success)
        {
            TempData["ErrorMessage"] = error ?? "Failed to reserve tickets.";
            return RedirectToAction(nameof(Details), new { id = request.EventId });
        }

        return RedirectToAction("Checkout", "Booking", new { reference = bookingRef });
    }
}
