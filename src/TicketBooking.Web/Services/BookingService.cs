namespace TicketBooking.Web.Services;

using TicketBooking.Web.Data.Repositories;
using TicketBooking.Web.ViewModels.Booking;

public interface IBookingService
{
    Task<(bool Success, string? Error, string? BookingReference, DateTime? ExpiresAtUtc)> HoldMovieSeatsAsync(
        int userId, int showtimeId, IReadOnlyList<int> seatIds, CancellationToken cancellationToken = default);

    Task<(bool Success, string? Error, string? BookingReference, DateTime? ExpiresAtUtc)> HoldEventTicketsAsync(
        int userId, int eventId, int ticketCategoryId, int quantity, CancellationToken cancellationToken = default);

    Task<CheckoutViewModel?> GetCheckoutSummaryAsync(
        string bookingReference, int userId, CancellationToken cancellationToken = default);

    Task<(bool Success, string? Error, string? TransactionRef)> ProcessPaymentAndConfirmAsync(
        string bookingReference, int userId, string paymentMode, string simulationOutcome, CancellationToken cancellationToken = default);

    Task<ConfirmationViewModel?> GetConfirmationDetailsAsync(
        string bookingReference, int userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserBookingItemSummary>> GetUserBookingsAsync(
        int userId, CancellationToken cancellationToken = default);

    Task<(bool Success, string? Error)> CancelBookingAsync(
        string bookingReference, int userId, CancellationToken cancellationToken = default);

    Task<int> CleanupExpiredHoldsAsync(CancellationToken cancellationToken = default);
}

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IShowtimeRepository _showtimeRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository bookingRepository,
        IShowtimeRepository showtimeRepository,
        IEventRepository eventRepository,
        IPaymentService paymentService,
        IConfiguration configuration,
        ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _showtimeRepository = showtimeRepository;
        _eventRepository = eventRepository;
        _paymentService = paymentService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<(bool Success, string? Error, string? BookingReference, DateTime? ExpiresAtUtc)> HoldMovieSeatsAsync(
        int userId, int showtimeId, IReadOnlyList<int> seatIds, CancellationToken cancellationToken = default)
    {
        if (seatIds == null || seatIds.Count == 0)
        {
            return (false, "Please select at least one seat to proceed.", null, null);
        }

        if (seatIds.Count > 10)
        {
            return (false, "A maximum of 10 seats can be reserved per transaction.", null, null);
        }

        var showtime = await _showtimeRepository.GetByIdAsync(showtimeId, cancellationToken);
        if (showtime == null || !showtime.IsActive)
        {
            return (false, "The selected showtime is not available.", null, null);
        }

        // Fetch seat definitions to recalculate base price on the server
        var seatMatrix = await _showtimeRepository.GetSeatMatrixWithStatusAsync(showtimeId, cancellationToken);
        var selectedSeatDetails = seatMatrix.Where(s => seatIds.Contains(s.SeatId)).ToList();

        if (selectedSeatDetails.Count != seatIds.Count)
        {
            return (false, "One or more selected seats are invalid for this screen.", null, null);
        }

        // SERVER AUTHORITY: Calculate base price strictly from server tier prices
        decimal basePrice = 0;
        foreach (var seat in selectedSeatDetails)
        {
            basePrice += seat.SeatTier switch
            {
                "Silver" => showtime.SilverPrice,
                "Gold" => showtime.GoldPrice,
                "Platinum" => showtime.PlatinumPrice,
                _ => showtime.GoldPrice
            };
        }

        // Server-side calculation of fee and taxes from configuration
        var feePercent = _configuration.GetValue<decimal>("BookingRules:ConvenienceFeePercent", 10.0m);
        var gstPercent = _configuration.GetValue<decimal>("BookingRules:GstPercent", 18.0m);
        var holdDurationMinutes = _configuration.GetValue<int>("BookingRules:HoldDurationMinutes", 8);

        var convenienceFee = Math.Round(basePrice * (feePercent / 100m), 2);
        var taxAmount = Math.Round(convenienceFee * (gstPercent / 100m), 2);

        return await _bookingRepository.CreateMovieSeatHoldAsync(
            userId, showtimeId, seatIds, basePrice, convenienceFee, taxAmount, 
            TimeSpan.FromMinutes(holdDurationMinutes), cancellationToken);
    }

    public async Task<(bool Success, string? Error, string? BookingReference, DateTime? ExpiresAtUtc)> HoldEventTicketsAsync(
        int userId, int eventId, int ticketCategoryId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
        {
            return (false, "Please select at least 1 ticket to proceed.", null, null);
        }

        if (quantity > 10)
        {
            return (false, "A maximum of 10 tickets can be reserved per transaction.", null, null);
        }

        var eventItem = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (eventItem == null || eventItem.Status != "Upcoming")
        {
            return (false, "The selected event is no longer available for booking.", null, null);
        }

        var category = await _eventRepository.GetCategoryByIdAsync(ticketCategoryId, cancellationToken);
        if (category == null || category.EventId != eventId)
        {
            return (false, "The selected ticket tier is invalid for this event.", null, null);
        }

        if (quantity > category.MaxPerBooking)
        {
            return (false, $"The maximum allowed quantity for {category.CategoryName} is {category.MaxPerBooking}.", null, null);
        }

        if (category.RemainingCapacity < quantity)
        {
            return (false, $"Only {category.RemainingCapacity} tickets remain for {category.CategoryName}.", null, null);
        }

        // SERVER AUTHORITY: Calculate base price strictly from database tier price
        var basePrice = category.Price * quantity;
        var feePercent = _configuration.GetValue<decimal>("BookingRules:ConvenienceFeePercent", 10.0m);
        var gstPercent = _configuration.GetValue<decimal>("BookingRules:GstPercent", 18.0m);
        var holdDurationMinutes = _configuration.GetValue<int>("BookingRules:HoldDurationMinutes", 8);

        var convenienceFee = Math.Round(basePrice * (feePercent / 100m), 2);
        var taxAmount = Math.Round(convenienceFee * (gstPercent / 100m), 2);

        return await _bookingRepository.CreateEventTicketHoldAsync(
            userId, eventId, ticketCategoryId, quantity, basePrice, convenienceFee, taxAmount,
            TimeSpan.FromMinutes(holdDurationMinutes), cancellationToken);
    }

    public async Task<CheckoutViewModel?> GetCheckoutSummaryAsync(
        string bookingReference, int userId, CancellationToken cancellationToken = default)
    {
        return await _bookingRepository.GetCheckoutSummaryAsync(bookingReference, userId, cancellationToken);
    }

    public async Task<(bool Success, string? Error, string? TransactionRef)> ProcessPaymentAndConfirmAsync(
        string bookingReference, int userId, string paymentMode, string simulationOutcome, CancellationToken cancellationToken = default)
    {
        var summary = await _bookingRepository.GetCheckoutSummaryAsync(bookingReference, userId, cancellationToken);
        if (summary == null)
        {
            return (false, "Booking reservation not found or unauthorized.", null);
        }

        if (summary.ExpiresAtUtc <= DateTime.UtcNow)
        {
            await _bookingRepository.ReleaseBookingHoldAsync(bookingReference, "Reservation hold expired before payment completion.", cancellationToken);
            return (false, "Your 8-minute reservation time expired. The held seats have been released back to inventory.", null);
        }

        // Execute payment simulation
        var (paymentOk, txnId, message) = await _paymentService.ExecutePaymentSimulationAsync(
            bookingReference, summary.TotalAmount, simulationOutcome, cancellationToken);

        if (!paymentOk)
        {
            // Payment failed or timed out: release inventory
            await _bookingRepository.ReleaseBookingHoldAsync(bookingReference, message, cancellationToken);
            _logger.LogWarning("Payment simulation failed for {Ref}: {Msg}", bookingReference, message);
            return (false, message, null);
        }

        // Payment succeeded: confirm booking
        return await _bookingRepository.ConfirmMovieBookingAsync(bookingReference, userId, paymentMode, cancellationToken);
    }

    public async Task<ConfirmationViewModel?> GetConfirmationDetailsAsync(
        string bookingReference, int userId, CancellationToken cancellationToken = default)
    {
        return await _bookingRepository.GetConfirmationDetailsAsync(bookingReference, userId, cancellationToken);
    }

    public async Task<IReadOnlyList<UserBookingItemSummary>> GetUserBookingsAsync(
        int userId, CancellationToken cancellationToken = default)
    {
        return await _bookingRepository.GetUserBookingsAsync(userId, cancellationToken);
    }

    public async Task<(bool Success, string? Error)> CancelBookingAsync(
        string bookingReference, int userId, CancellationToken cancellationToken = default)
    {
        var cutoffHours = _configuration.GetValue<int>("BookingRules:CancellationCutoffHours", 2);
        return await _bookingRepository.CancelBookingAndRestoreSeatsAsync(bookingReference, userId, cutoffHours, cancellationToken);
    }

    public async Task<int> CleanupExpiredHoldsAsync(CancellationToken cancellationToken = default)
    {
        return await _bookingRepository.CleanupExpiredHoldsAsync(cancellationToken);
    }
}
