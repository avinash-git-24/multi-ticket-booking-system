# Interface Contracts: Services and Repositories

This document formalizes the C# interface contracts between the presentation layer, business services, and ADO.NET data access repositories.

---

## 1. Database Connection Factory

```csharp
namespace TicketBooking.Web.Data;

public interface IDbConnectionFactory
{
    Task<System.Data.Common.DbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    System.Data.Common.DbConnection CreateConnection();
}
```

---

## 2. Authentication and User Domain

```csharp
namespace TicketBooking.Web.Services;

public interface IUserService
{
    Task<(bool Success, string Error, UserDto? User)> RegisterAsync(RegisterRequestDto request);
    Task<(bool Success, string Error, UserDto? User)> AuthenticateAsync(string email, string password);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<bool> UpdateProfileAsync(int userId, string fullName, string phoneNumber);
}

namespace TicketBooking.Web.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByEmailAsync(string email);
    Task<int> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<IReadOnlyList<Role>> GetRolesByUserIdAsync(int userId);
}
```

---

## 3. Movie and Showtime Domain

```csharp
namespace TicketBooking.Web.Services;

public interface IMovieService
{
    Task<IReadOnlyList<MovieCardDto>> GetNowShowingMoviesAsync(string? city = null, string? genre = null, string? language = null, string? search = null);
    Task<IReadOnlyList<MovieCardDto>> GetComingSoonMoviesAsync(string? city = null, string? genre = null, string? language = null);
    Task<MovieDetailsDto?> GetMovieDetailsAsync(int movieId);
    Task<IReadOnlyList<ShowtimeGroupDto>> GetShowtimesAsync(int movieId, DateTime date, string? city = null);
    Task<ShowtimeSeatMapDto?> GetSeatMapAsync(int showtimeId);
    Task<int> CreateMovieAsync(CreateMovieDto dto);
    Task<bool> UpdateMovieAsync(UpdateMovieDto dto);
    Task<bool> DeleteMovieAsync(int movieId);
}

namespace TicketBooking.Web.Data.Repositories;

public interface IMovieRepository
{
    Task<Movie?> GetByIdAsync(int movieId);
    Task<IReadOnlyList<Movie>> GetMoviesAsync(string? city, string? status, string? genre, string? language, string? search);
    Task<int> CreateAsync(Movie movie);
    Task<bool> UpdateAsync(Movie movie);
    Task<bool> DeleteAsync(int movieId);
    Task<IReadOnlyList<string>> GetDistinctGenresAsync();
    Task<IReadOnlyList<string>> GetDistinctLanguagesAsync();
}

public interface IShowtimeRepository
{
    Task<Showtime?> GetByIdAsync(int showtimeId);
    Task<IReadOnlyList<ShowtimeDetails>> GetShowtimesByMovieAsync(int movieId, DateTime date, string? city);
    Task<IReadOnlyList<ShowtimeSeatStatus>> GetSeatsWithStatusAsync(int showtimeId);
    Task<int> CreateShowtimeAsync(Showtime showtime);
    Task<bool> DeleteShowtimeAsync(int showtimeId);
}
```

---

## 4. Event and Ticket Category Domain

```csharp
namespace TicketBooking.Web.Services;

public interface IEventService
{
    Task<IReadOnlyList<EventCardDto>> GetEventsAsync(string? category = null, string? city = null, string? search = null);
    Task<EventDetailsDto?> GetEventDetailsAsync(int eventId);
    Task<IReadOnlyList<TicketCategoryDto>> GetTicketCategoriesAsync(int eventId);
    Task<int> CreateEventAsync(CreateEventDto dto);
    Task<bool> UpdateEventAsync(UpdateEventDto dto);
}

namespace TicketBooking.Web.Data.Repositories;

public interface IEventRepository
{
    Task<EventItem?> GetByIdAsync(int eventId);
    Task<IReadOnlyList<EventItem>> GetEventsAsync(string? category, string? city, string? search);
    Task<IReadOnlyList<TicketCategory>> GetCategoriesByEventIdAsync(int eventId);
    Task<int> CreateAsync(EventItem item);
    Task<bool> UpdateAsync(EventItem item);
    Task<int> AddTicketCategoryAsync(TicketCategory category);
}
```

---

## 5. Booking and Inventory Engine

```csharp
namespace TicketBooking.Web.Services;

public interface IBookingService
{
    Task<(bool Success, string Error, string? BookingReference, DateTime? ExpiresAtUtc)> HoldMovieSeatsAsync(int userId, int showtimeId, IReadOnlyList<int> seatIds);
    Task<(bool Success, string Error, string? BookingReference, DateTime? ExpiresAtUtc)> HoldEventTicketsAsync(int userId, int eventId, int ticketCategoryId, int quantity);
    Task<CheckoutSummaryDto?> GetCheckoutSummaryAsync(string bookingReference, int userId);
    Task<(bool Success, string Error)> ProcessPaymentAndConfirmAsync(string bookingReference, int userId, PaymentSimulationMode mode);
    Task<BookingConfirmationDto?> GetConfirmationAsync(string bookingReference, int userId);
    Task<IReadOnlyList<UserBookingSummaryDto>> GetUserBookingsAsync(int userId);
    Task<(bool Success, string Error)> CancelBookingAsync(string bookingReference, int userId);
    Task<int> CleanupExpiredHoldsAsync();
}

namespace TicketBooking.Web.Data.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByReferenceAsync(string reference);
    Task<IReadOnlyList<Booking>> GetByUserIdAsync(int userId);
    Task<IReadOnlyList<BookingItem>> GetItemsByBookingIdAsync(int bookingId);
    Task<string> CreateMovieHoldAsync(int userId, int showtimeId, IReadOnlyList<int> seatIds, decimal basePrice, decimal fee, decimal tax, TimeSpan holdDuration);
    Task<string> CreateEventHoldAsync(int userId, int eventId, int categoryId, int quantity, decimal basePrice, decimal fee, decimal tax, TimeSpan holdDuration);
    Task<bool> ConfirmBookingAsync(int bookingId, string transactionId, string paymentMode);
    Task<bool> MarkPaymentFailedAsync(int bookingId, string reason);
    Task<bool> CancelBookingAndReleaseInventoryAsync(int bookingId, string cancellationReason);
    Task<IReadOnlyList<int>> ReleaseExpiredHoldsAsync();
}
```

---

## 6. Simulated Payment Service

```csharp
namespace TicketBooking.Web.Services;

public enum PaymentSimulationMode
{
    Success,
    FailureInsufficientFunds,
    FailureNetworkTimeout,
    FailureCardDeclined
}

public interface IPaymentService
{
    Task<(bool Success, string TransactionId, string Message)> ExecutePaymentSimulationAsync(
        string bookingReference, 
        decimal amount, 
        PaymentSimulationMode mode, 
        CancellationToken cancellationToken = default);
}
```
