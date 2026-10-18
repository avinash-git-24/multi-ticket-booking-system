namespace TicketBooking.Tests;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using TicketBooking.Web.Data;
using TicketBooking.Web.Data.Repositories;

public class BookingConcurrencyTests
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly BookingRepository _bookingRepository;

    public BookingConcurrencyTests()
    {
        var settings = new Dictionary<string, string?>
        {
            {"ConnectionStrings:DefaultConnection", "Server=(localdb)\\MSSQLLocalDB;Database=ShowSphereDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;"}
        };

        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        _connectionFactory = new DbConnectionFactory(config);
        _bookingRepository = new BookingRepository(_connectionFactory, NullLogger<BookingRepository>.Instance);
    }

    [Fact]
    public async Task TwoUsers_CompetingForSameSeat_ExactlyOneSucceeds_AndOneReceivesConflictMessage()
    {
        const int showtimeId = 1;

        // Ensure valid test users exist in database to satisfy foreign key constraints
        var user1Id = await EnsureTestUserAsync("competitor1@showsphere.test", "Competitor One");
        var user2Id = await EnsureTestUserAsync("competitor2@showsphere.test", "Competitor Two");

        // Target seats 15 and 16
        var user1Seats = new List<int> { 15, 16 };
        var user2Seats = new List<int> { 16, 17 }; // Seat 16 is in contention

        // Ensure target seats are clean and available before test
        await ResetTestSeatsAsync(showtimeId, new[] { 15, 16, 17 });

        // Launch concurrent hold requests simultaneously
        var task1 = Task.Run(() => _bookingRepository.CreateMovieSeatHoldAsync(
            userId: user1Id,
            showtimeId: showtimeId,
            seatIds: user1Seats,
            basePrice: 400.00m,
            convenienceFee: 40.00m,
            taxAmount: 7.20m,
            holdDuration: TimeSpan.FromMinutes(5)));

        var task2 = Task.Run(() => _bookingRepository.CreateMovieSeatHoldAsync(
            userId: user2Id,
            showtimeId: showtimeId,
            seatIds: user2Seats,
            basePrice: 400.00m,
            convenienceFee: 40.00m,
            taxAmount: 7.20m,
            holdDuration: TimeSpan.FromMinutes(5)));

        var results = await Task.WhenAll(task1, task2);
        var result1 = results[0];
        var result2 = results[1];

        // Exactly one must succeed and exactly one must fail
        var successCount = (result1.Success ? 1 : 0) + (result2.Success ? 1 : 0);
        var failureCount = (!result1.Success ? 1 : 0) + (!result2.Success ? 1 : 0);

        Assert.Equal(1, successCount);
        Assert.Equal(1, failureCount);

        var failedResult = !result1.Success ? result1 : result2;
        var successfulResult = result1.Success ? result1 : result2;

        Assert.False(string.IsNullOrWhiteSpace(successfulResult.BookingReference));
        Assert.Contains("no longer available", failedResult.Error, StringComparison.OrdinalIgnoreCase);

        // Cleanup: release hold so future tests stay clean
        if (successfulResult.BookingReference != null)
        {
            await _bookingRepository.ReleaseBookingHoldAsync(successfulResult.BookingReference, "Test teardown cleanup");
        }
    }

    private async Task<int> EnsureTestUserAsync(string email, string name)
    {
        using var conn = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = @Email)
            BEGIN
                INSERT INTO dbo.Users (FullName, Email, PasswordHash, PhoneNumber, PreferredCityId, IsActive, CreatedAtUtc)
                VALUES (@Name, @Email, 'TestHash123', '+919999999999', 1, 1, SYSUTCDATETIME());
            END
            SELECT UserId FROM dbo.Users WHERE Email = @Email;";

        using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@Name", name);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    private async Task ResetTestSeatsAsync(int showtimeId, IEnumerable<int> seatIds)
    {
        using var conn = await _connectionFactory.CreateConnectionAsync();
        var inClause = string.Join(",", seatIds);
        var sql = $@"
            UPDATE dbo.ShowtimeSeats
            SET Status = 'Available', HoldExpiresAtUtc = NULL, BookingId = NULL
            WHERE ShowtimeId = {showtimeId} AND SeatId IN ({inClause});";

        using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }
}
