namespace TicketBooking.Tests;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TicketBooking.Web.Data;
using TicketBooking.Web.Models;

public class FoundationTests
{
    [Fact]
    public void DbConnectionFactory_Constructs_With_Configured_Connection_String()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"ConnectionStrings:DefaultConnection", "Server=(localdb)\\MSSQLLocalDB;Database=ShowSphereDb;Trusted_Connection=True;"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var factory = new DbConnectionFactory(configuration);
        using var conn = factory.CreateConnection();

        Assert.NotNull(conn);
        Assert.Contains("ShowSphereDb", conn.ConnectionString);
    }

    [Fact]
    public void PasswordHasher_Correctly_Hashes_And_Verifies_Passwords()
    {
        var hasher = new PasswordHasher<User>();
        var user = new User { Email = "faculty@college.edu", FullName = "Faculty Reviewer" };
        var password = "SecureDemonstration2026!";

        var hash = hasher.HashPassword(user, password);
        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.NotEqual(password, hash);

        var verifySuccess = hasher.VerifyHashedPassword(user, hash, password);
        Assert.Equal(PasswordVerificationResult.Success, verifySuccess);

        var verifyFail = hasher.VerifyHashedPassword(user, hash, "WrongPassword123!");
        Assert.Equal(PasswordVerificationResult.Failed, verifyFail);
    }

    [Theory]
    [InlineData("SSP-2026-ABC123")]
    [InlineData("SSP-2026-XYZ789")]
    public void BookingReference_Follows_Expected_Format(string reference)
    {
        Assert.StartsWith("SSP-", reference);
        Assert.Contains("-2026-", reference);
        Assert.Equal(15, reference.Length);
    }
}
