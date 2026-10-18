namespace TicketBooking.Web.ViewModels.Admin;

public class AdminDashboardViewModel
{
    public int TotalMovies { get; set; }
    public int TotalEvents { get; set; }
    public int TotalBookings { get; set; }
    public int TotalUsers { get; set; }
    public decimal TotalRevenue { get; set; }
    public int ConfirmedBookingsCount { get; set; }
    public IReadOnlyList<AdminBookingSummary> RecentBookings { get; set; } = Array.Empty<AdminBookingSummary>();
    public IReadOnlyList<AdminUserSummary> RecentUsers { get; set; } = Array.Empty<AdminUserSummary>();
}

public class AdminBookingSummary
{
    public string BookingReference { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string BookingType { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}

public class AdminUserSummary
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string Roles { get; set; } = string.Empty;
}
