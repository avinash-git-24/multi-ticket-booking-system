namespace TicketBooking.Web.Models;

public class User
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int? PreferredCityId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public List<string> Roles { get; set; } = new();
}

public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class City
{
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string StateName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
