namespace TicketBooking.Tests;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using TicketBooking.Web.Data;
using TicketBooking.Web.Data.Repositories;

public class BookingRestorationTests
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly BookingRepository _bookingRepository;

    public BookingRestorationTests()
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
    public async Task PaymentFailure_ReleasesHeldSeats_BackToAvailable()
    {
        var showtimeId = await GetFutureShowtimeIdAsync();
        var testUserId = await EnsureTestUserAsync("failtest@showsphere.test", "Payment Failure Tester");
        var seats = new List<int> { 20, 21 };

        // 1. Reset target seats
        await ResetTestSeatsAsync(showtimeId, seats);

        // 2. Create hold
        var (holdSuccess, _, bookingRef, _) = await _bookingRepository.CreateMovieSeatHoldAsync(
            userId: testUserId,
            showtimeId: showtimeId,
            seatIds: seats,
            basePrice: 440.00m,
            convenienceFee: 44.00m,
            taxAmount: 7.92m,
            holdDuration: TimeSpan.FromMinutes(8));

        Assert.True(holdSuccess);
        Assert.NotNull(bookingRef);

        // 3. Verify status in database is 'Held'
        var statusWhileHeld = await GetSeatStatusesAsync(showtimeId, seats);
        Assert.All(statusWhileHeld, status => Assert.Equal("Held", status));

        // 4. Release hold due to simulated payment failure
        var releaseOk = await _bookingRepository.ReleaseBookingHoldAsync(bookingRef, "Simulated Card Declined");
        Assert.True(releaseOk);

        // 5. Verify status in database is restored to 'Available'
        var statusAfterRelease = await GetSeatStatusesAsync(showtimeId, seats);
        Assert.All(statusAfterRelease, status => Assert.Equal("Available", status));
    }

    [Fact]
    public async Task ConfirmedBooking_WhenCancelledBeforeCutoff_RestoresSeatsToAvailable()
    {
        var showtimeId = await GetFutureShowtimeIdAsync();
        var testUserId = await EnsureTestUserAsync("canceltest@showsphere.test", "Cancellation Tester");
        var seats = new List<int> { 25 };

        // 1. Reset target seat
        await ResetTestSeatsAsync(showtimeId, seats);

        // 2. Create hold
        var (holdSuccess, _, bookingRef, _) = await _bookingRepository.CreateMovieSeatHoldAsync(
            userId: testUserId,
            showtimeId: showtimeId,
            seatIds: seats,
            basePrice: 220.00m,
            convenienceFee: 22.00m,
            taxAmount: 3.96m,
            holdDuration: TimeSpan.FromMinutes(8));

        Assert.True(holdSuccess);
        Assert.NotNull(bookingRef);

        // 3. Confirm booking
        var (confirmSuccess, _, txnRef) = await _bookingRepository.ConfirmMovieBookingAsync(
            bookingRef, userId: testUserId, paymentMode: "UPI");

        Assert.True(confirmSuccess);
        Assert.NotNull(txnRef);

        // 4. Verify seat status in database is 'Booked'
        var statusWhileBooked = await GetSeatStatusesAsync(showtimeId, seats);
        Assert.All(statusWhileBooked, status => Assert.Equal("Booked", status));

        // 5. Cancel booking (cutoff hours set to 2 hours)
        var (cancelSuccess, cancelError) = await _bookingRepository.CancelBookingAndRestoreSeatsAsync(
            bookingRef, userId: testUserId, cutoffHours: 2);

        Assert.True(cancelSuccess, cancelError);

        // 6. Verify seat status in database is restored to 'Available'
        var statusAfterCancel = await GetSeatStatusesAsync(showtimeId, seats);
        Assert.All(statusAfterCancel, status => Assert.Equal("Available", status));
    }

    private async Task<int> GetFutureShowtimeIdAsync()
    {
        using var conn = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT TOP 1 ShowtimeId FROM dbo.Showtimes WHERE StartTime > DATEADD(HOUR, 12, SYSUTCDATETIME()) ORDER BY StartTime ASC;";
        using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
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

    private async Task<List<string>> GetSeatStatusesAsync(int showtimeId, IEnumerable<int> seatIds)
    {
        using var conn = await _connectionFactory.CreateConnectionAsync();
        var inClause = string.Join(",", seatIds);
        var sql = $@"
            SELECT Status FROM dbo.ShowtimeSeats
            WHERE ShowtimeId = {showtimeId} AND SeatId IN ({inClause});";

        using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
        var list = new List<string>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(reader.GetString(0));
        }
        return list;
    }
}
