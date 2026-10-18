namespace TicketBooking.Web.ViewModels.Booking;

using TicketBooking.Web.Models;

public class SeatMapViewModel
{
    public int ShowtimeId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string Certificate { get; set; } = "UA";
    public string Language { get; set; } = string.Empty;
    public string TheatreName { get; set; } = string.Empty;
    public string ScreenName { get; set; } = string.Empty;
    public string ScreenType { get; set; } = "2D";
    public DateTime ShowDateTime { get; set; }
    public decimal SilverPrice { get; set; }
    public decimal GoldPrice { get; set; }
    public decimal PlatinumPrice { get; set; }

    public Dictionary<string, List<ShowtimeSeat>> RowsGrouped { get; set; } = new();
}

public class HoldSeatsRequest
{
    public int ShowtimeId { get; set; }
    public List<int> SeatIds { get; set; } = new();
}

public class HoldSeatsResponse
{
    public bool Success { get; set; }
    public string? BookingReference { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
}

public class CheckoutViewModel
{
    public string BookingReference { get; set; } = string.Empty;
    public string BookingType { get; set; } = "Movie";
    public string ItemTitle { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
    public string? SubVenue { get; set; }
    public DateTime EventDateTime { get; set; }
    public List<string> SeatOrTicketLabels { get; set; } = new();
    public int Quantity { get; set; }

    public decimal BaseAmount { get; set; }
    public decimal ConvenienceFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public DateTime ExpiresAtUtc { get; set; }
    public int RemainingSeconds => Math.Max(0, (int)(ExpiresAtUtc - DateTime.UtcNow).TotalSeconds);
}

public class ProcessPaymentRequest
{
    public string BookingReference { get; set; } = string.Empty;
    public string PaymentMode { get; set; } = "UPI";
    public string SimulationOutcome { get; set; } = "Success";
}

public class ConfirmationViewModel
{
    public string BookingReference { get; set; } = string.Empty;
    public string BookingType { get; set; } = "Movie";
    public string ItemTitle { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
    public string? SubVenue { get; set; }
    public DateTime EventDateTime { get; set; }
    public List<string> SeatOrTicketLabels { get; set; } = new();
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMode { get; set; } = "Simulated";
    public string TransactionReference { get; set; } = string.Empty;
    public string Status { get; set; } = "Confirmed";
    public DateTime ConfirmedAtUtc { get; set; }
}

public class UserBookingItemSummary
{
    public int BookingId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string BookingType { get; set; } = "Movie";
    public string ItemTitle { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
    public DateTime EventDateTime { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Confirmed";
    public DateTime CreatedAtUtc { get; set; }
    public bool CanCancel { get; set; }
    public List<string> SeatLabels { get; set; } = new();
}

public class MyBookingsViewModel
{
    public List<UserBookingItemSummary> UpcomingBookings { get; set; } = new();
    public List<UserBookingItemSummary> PastBookings { get; set; } = new();
    public List<UserBookingItemSummary> CancelledBookings { get; set; } = new();
}
