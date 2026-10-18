namespace TicketBooking.Web.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketBooking.Web.Services;
using TicketBooking.Web.ViewModels.Booking;

public class BookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IMovieService _movieService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(
        IBookingService bookingService,
        IMovieService movieService,
        ILogger<BookingController> logger)
    {
        _bookingService = bookingService;
        _movieService = movieService;
        _logger = logger;
    }

    [HttpGet]
    [Route("booking/seats/{showtimeId:int}")]
    public async Task<IActionResult> SeatMap(int showtimeId)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction("Login", "Account", new { returnUrl = $"/booking/seats/{showtimeId}" });
        }

        var viewModel = await _movieService.GetSeatMapAsync(showtimeId);
        if (viewModel == null)
        {
            return NotFound();
        }

        return View(viewModel);
    }

    [HttpPost]
    [Authorize]
    [Route("booking/hold-seats")]
    public async Task<IActionResult> HoldSeats([FromBody] HoldSeatsRequest request)
    {
        if (request == null || request.SeatIds == null || request.SeatIds.Count == 0)
        {
            return Json(new { success = false, errorMessage = "Please select at least one seat." });
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return Json(new { success = false, errorMessage = "Session expired. Please sign in again." });
        }

        var (success, error, bookingRef, expiresAtUtc) = await _bookingService.HoldMovieSeatsAsync(
            userId, request.ShowtimeId, request.SeatIds);

        if (!success || string.IsNullOrEmpty(bookingRef))
        {
            return Json(new { success = false, errorMessage = error ?? "Failed to reserve seats." });
        }

        return Json(new
        {
            success = true,
            bookingReference = bookingRef,
            expiresAtUtc = expiresAtUtc?.ToString("o"),
            redirectUrl = Url.Action("Checkout", "Booking", new { reference = bookingRef })
        });
    }

    [HttpGet]
    [Authorize]
    [Route("booking/checkout/{reference}")]
    public async Task<IActionResult> Checkout(string reference)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var summary = await _bookingService.GetCheckoutSummaryAsync(reference, userId);
        if (summary == null)
        {
            TempData["ErrorMessage"] = "Booking reservation not found or access unauthorized.";
            return RedirectToAction("Index", "Movies");
        }

        if (summary.RemainingSeconds <= 0)
        {
            TempData["ErrorMessage"] = "Your reservation hold has expired. Please select your seats again.";
            return RedirectToAction("Index", "Movies");
        }

        return View(summary);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [Route("booking/process-payment")]
    public async Task<IActionResult> ProcessPayment(ProcessPaymentRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var (success, error, txnRef) = await _bookingService.ProcessPaymentAndConfirmAsync(
            request.BookingReference, userId, request.PaymentMode, request.SimulationOutcome);

        if (!success)
        {
            TempData["ErrorMessage"] = error ?? "Payment processing failed. Your held seats have been released.";
            return RedirectToAction("Checkout", new { reference = request.BookingReference });
        }

        return RedirectToAction("Confirmation", new { reference = request.BookingReference });
    }

    [HttpGet]
    [Authorize]
    [Route("booking/confirmation/{reference}")]
    public async Task<IActionResult> Confirmation(string reference)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var confirmation = await _bookingService.GetConfirmationDetailsAsync(reference, userId);
        if (confirmation == null)
        {
            return NotFound();
        }

        return View(confirmation);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [Route("booking/cancel/{reference}")]
    public async Task<IActionResult> Cancel(string reference)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var (success, error) = await _bookingService.CancelBookingAsync(reference, userId);
        if (!success)
        {
            TempData["ErrorMessage"] = error ?? "Unable to cancel booking.";
        }
        else
        {
            TempData["SuccessMessage"] = $"Booking {reference} has been successfully cancelled and inventory released.";
        }

        return RedirectToAction("MyBookings", "Account");
    }
}
