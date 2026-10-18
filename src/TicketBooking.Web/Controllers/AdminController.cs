namespace TicketBooking.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TicketBooking.Web.Data;
using TicketBooking.Web.Services;
using TicketBooking.Web.ViewModels.Admin;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IBookingService _bookingService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IDbConnectionFactory connectionFactory,
        IBookingService bookingService,
        ILogger<AdminController> logger)
    {
        _connectionFactory = connectionFactory;
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new AdminDashboardViewModel();

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        // 1. Fetch Key Counts and Revenue
        const string countsSql = @"
            SELECT 
                (SELECT COUNT(*) FROM dbo.Movies) AS TotalMovies,
                (SELECT COUNT(*) FROM dbo.Events) AS TotalEvents,
                (SELECT COUNT(*) FROM dbo.Bookings) AS TotalBookings,
                (SELECT COUNT(*) FROM dbo.Users) AS TotalUsers,
                (SELECT ISNULL(SUM(TotalAmount), 0) FROM dbo.Bookings WHERE Status = 'Confirmed') AS TotalRevenue,
                (SELECT COUNT(*) FROM dbo.Bookings WHERE Status = 'Confirmed') AS ConfirmedBookingsCount;";

        using (var cmd = new SqlCommand(countsSql, connection))
        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
        {
            if (await reader.ReadAsync(cancellationToken))
            {
                model.TotalMovies = reader.GetInt32(reader.GetOrdinal("TotalMovies"));
                model.TotalEvents = reader.GetInt32(reader.GetOrdinal("TotalEvents"));
                model.TotalBookings = reader.GetInt32(reader.GetOrdinal("TotalBookings"));
                model.TotalUsers = reader.GetInt32(reader.GetOrdinal("TotalUsers"));
                model.TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue"));
                model.ConfirmedBookingsCount = reader.GetInt32(reader.GetOrdinal("ConfirmedBookingsCount"));
            }
        }

        // 2. Fetch Recent Bookings
        const string recentBookingsSql = @"
            SELECT TOP 10 
                b.BookingReference,
                b.BookingType,
                b.TotalQuantity,
                b.TotalAmount,
                b.Status,
                b.CreatedAtUtc,
                u.FullName AS CustomerName,
                u.Email AS CustomerEmail
            FROM dbo.Bookings b
            JOIN dbo.Users u ON b.UserId = u.UserId
            ORDER BY b.CreatedAtUtc DESC;";

        var bookingsList = new List<AdminBookingSummary>();
        using (var cmd = new SqlCommand(recentBookingsSql, connection))
        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                bookingsList.Add(new AdminBookingSummary
                {
                    BookingReference = reader.GetString(reader.GetOrdinal("BookingReference")),
                    BookingType = reader.GetString(reader.GetOrdinal("BookingType")),
                    TotalQuantity = reader.GetInt32(reader.GetOrdinal("TotalQuantity")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                    CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                    CustomerEmail = reader.GetString(reader.GetOrdinal("CustomerEmail"))
                });
            }
        }
        model.RecentBookings = bookingsList;

        // 3. Fetch Recent Users
        const string recentUsersSql = @"
            SELECT TOP 10 
                u.UserId,
                u.FullName,
                u.Email,
                u.IsActive,
                u.CreatedAtUtc,
                STRING_AGG(r.RoleName, ', ') AS Roles
            FROM dbo.Users u
            LEFT JOIN dbo.UserRoles ur ON u.UserId = ur.UserId
            LEFT JOIN dbo.Roles r ON ur.RoleId = r.RoleId
            GROUP BY u.UserId, u.FullName, u.Email, u.IsActive, u.CreatedAtUtc
            ORDER BY u.CreatedAtUtc DESC;";

        var usersList = new List<AdminUserSummary>();
        using (var cmd = new SqlCommand(recentUsersSql, connection))
        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                usersList.Add(new AdminUserSummary
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                    Roles = reader.IsDBNull(reader.GetOrdinal("Roles")) ? "User" : reader.GetString(reader.GetOrdinal("Roles"))
                });
            }
        }
        model.RecentUsers = usersList;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CleanupHolds(CancellationToken cancellationToken)
    {
        var cleaned = await _bookingService.CleanupExpiredHoldsAsync(cancellationToken);
        TempData["AdminMessage"] = $"Cleaned up {cleaned} expired seat holds successfully.";
        _logger.LogInformation("Admin triggered expired seat holds cleanup: {Count} holds removed.", cleaned);
        return RedirectToAction(nameof(Index));
    }
}
