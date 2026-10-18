namespace TicketBooking.Tests;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using TicketBooking.Web.Data;
using TicketBooking.Web.Data.Repositories;
using TicketBooking.Web.Services;

public class EventBookingTests
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly EventRepository _eventRepository;
    private readonly BookingRepository _bookingRepository;
    private readonly BookingService _bookingService;

    public EventBookingTests()
    {
        var settings = new Dictionary<string, string?>
        {
            {"ConnectionStrings:DefaultConnection", "Server=(localdb)\\MSSQLLocalDB;Database=ShowSphereDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;"},
            {"BookingRules:ConvenienceFeePercent", "10.0"},
            {"BookingRules:GstPercent", "18.0"},
            {"BookingRules:HoldDurationMinutes", "8"},
            {"BookingRules:CancellationCutoffHours", "2"}
        };

        IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        _connectionFactory = new DbConnectionFactory(config);
        _eventRepository = new EventRepository(_connectionFactory, NullLogger<EventRepository>.Instance);
        _bookingRepository = new BookingRepository(_connectionFactory, NullLogger<BookingRepository>.Instance);

        var showtimeRepo = new ShowtimeRepository(_connectionFactory);
        var paymentService = new SimulatedPaymentService(NullLogger<SimulatedPaymentService>.Instance);
        _bookingService = new BookingService(_bookingRepository, showtimeRepo, _eventRepository, paymentService, config, NullLogger<BookingService>.Instance);
    }

    [Fact]
    public async Task EventRepository_GetEventsAsync_FilterByCategoryAndCity_ReturnsMatchingEvents()
    {
        // Act: Filter by category 'Concert' and city 'Rajkot'
        var events = await _eventRepository.GetEventsAsync("Concert", "Rajkot", null);

        // Assert
        Assert.NotEmpty(events);
        Assert.All(events, ev =>
        {
            Assert.Equal("Concert", ev.Category);
            Assert.Equal("Rajkot", ev.CityName);
            Assert.True(ev.StartingPrice > 0, "Starting price should be positive.");
            Assert.True(ev.TotalRemainingTickets > 0, "Remaining tickets should be positive.");
        });
    }

    [Fact]
    public async Task BookingService_HoldEventTicketsAsync_EnforcesServerAuthorityPricing()
    {
        var testUserId = await EnsureTestUserAsync("pricing.user@showsphere.test", "Pricing Test User");

        // Target Event 1, Category 1 (Price: 999.00)
        const int eventId = 1;
        const int categoryId = 1;
        const int quantity = 2;

        // Act: Hold 2 tickets
        var (success, error, bookingRef, expiresAtUtc) = await _bookingService.HoldEventTicketsAsync(
            testUserId, eventId, categoryId, quantity);

        // Assert
        Assert.True(success, $"Hold failed: {error}");
        Assert.NotNull(bookingRef);
        Assert.StartsWith("SSP-2026-", bookingRef);

        // Inspect checkout summary computed on the server
        var summary = await _bookingService.GetCheckoutSummaryAsync(bookingRef, testUserId);
        Assert.NotNull(summary);
        Assert.Equal(2, summary.Quantity);
        Assert.Equal(1998.00m, summary.BaseAmount); // 999 * 2
        Assert.Equal(199.80m, summary.ConvenienceFee); // 10%
        Assert.Equal(35.96m, summary.TaxAmount); // 18% of 199.80 = 35.964 -> 35.96
        Assert.Equal(2233.76m, summary.TotalAmount); // 1998 + 199.80 + 35.96

        // Cleanup: release hold
        await _bookingRepository.ReleaseBookingHoldAsync(bookingRef, "Test cleanup");
    }

    [Fact]
    public async Task TwoUsers_CompetingForLastRemainingTickets_AtomicDecrementPreventsOverselling()
    {
        // Setup: Create a temporary test tier with capacity of exactly 2 tickets
        var testCategoryId = await CreateTestTicketCategoryAsync(eventId: 1, name: "Limited VIP Test", price: 500.00m, capacity: 2);

        var user1Id = await EnsureTestUserAsync("compete.event1@showsphere.test", "Event Competitor 1");
        var user2Id = await EnsureTestUserAsync("compete.event2@showsphere.test", "Event Competitor 2");

        // Both users concurrently request 2 tickets for this category
        var task1 = Task.Run(() => _bookingRepository.CreateEventTicketHoldAsync(
            user1Id, eventId: 1, ticketCategoryId: testCategoryId, quantity: 2,
            basePrice: 1000.00m, convenienceFee: 100.00m, taxAmount: 18.00m,
            holdDuration: TimeSpan.FromMinutes(5)));

        var task2 = Task.Run(() => _bookingRepository.CreateEventTicketHoldAsync(
            user2Id, eventId: 1, ticketCategoryId: testCategoryId, quantity: 2,
            basePrice: 1000.00m, convenienceFee: 100.00m, taxAmount: 18.00m,
            holdDuration: TimeSpan.FromMinutes(5)));

        var results = await Task.WhenAll(task1, task2);

        var successCount = results.Count(r => r.Success);
        var failureCount = results.Count(r => !r.Success);

        // Assert: Exactly one winner, exactly one loser
        Assert.Equal(1, successCount);
        Assert.Equal(1, failureCount);

        // Verify loser received proper conflict / sold out message
        var failedResult = results.First(r => !r.Success);
        Assert.NotNull(failedResult.Error);

        // Verify remaining capacity in database is exactly 0 (no overselling!)
        var remaining = await GetRemainingCapacityAsync(testCategoryId);
        Assert.Equal(0, remaining);

        // Cleanup winning hold
        var winnerRef = results.First(r => r.Success).BookingReference!;
        await _bookingRepository.ReleaseBookingHoldAsync(winnerRef, "Concurrency test completion");
    }

    [Fact]
    public async Task PaymentFailure_ReleasesEventCapacity_RestoresRemainingTickets()
    {
        // Setup: Create test tier with capacity 5
        var testCategoryId = await CreateTestTicketCategoryAsync(eventId: 1, name: "Payment Fail Restor Tier", price: 400.00m, capacity: 5);
        var userId = await EnsureTestUserAsync("fail.event@showsphere.test", "Fail Event User");

        // Act 1: Reserve 3 tickets
        var (holdSuccess, error, bookingRef, _) = await _bookingRepository.CreateEventTicketHoldAsync(
            userId, eventId: 1, ticketCategoryId: testCategoryId, quantity: 3,
            basePrice: 1200.00m, convenienceFee: 120.00m, taxAmount: 21.60m,
            holdDuration: TimeSpan.FromMinutes(5));

        Assert.True(holdSuccess, $"Hold failed: {error}");
        Assert.NotNull(bookingRef);

        // Capacity should now be 2
        var capacityAfterHold = await GetRemainingCapacityAsync(testCategoryId);
        Assert.Equal(2, capacityAfterHold);

        // Act 2: Simulate payment failure
        var released = await _bookingRepository.ReleaseBookingHoldAsync(bookingRef, "Simulated payment failure (Insufficient funds)");
        Assert.True(released);

        // Capacity should be fully restored back to 5
        var capacityAfterRelease = await GetRemainingCapacityAsync(testCategoryId);
        Assert.Equal(5, capacityAfterRelease);
    }

    [Fact]
    public async Task ConfirmedEventBooking_WhenCancelledBeforeCutoff_RestoresCapacity()
    {
        // Setup: Create test tier with capacity 4
        var testCategoryId = await CreateTestTicketCategoryAsync(eventId: 1, name: "Cancel Restor Tier", price: 300.00m, capacity: 4);
        var userId = await EnsureTestUserAsync("cancel.event@showsphere.test", "Cancel Event User");

        // Act 1: Hold 2 tickets
        var (holdSuccess, error, bookingRef, _) = await _bookingRepository.CreateEventTicketHoldAsync(
            userId, eventId: 1, ticketCategoryId: testCategoryId, quantity: 2,
            basePrice: 600.00m, convenienceFee: 60.00m, taxAmount: 10.80m,
            holdDuration: TimeSpan.FromMinutes(5));

        Assert.True(holdSuccess, $"Hold failed: {error}");

        // Act 2: Confirm booking
        var (confirmSuccess, _, txnRef) = await _bookingRepository.ConfirmMovieBookingAsync(bookingRef!, userId, "UPI");
        Assert.True(confirmSuccess);
        Assert.NotNull(txnRef);

        // Capacity should be 2
        var capacityAfterConfirm = await GetRemainingCapacityAsync(testCategoryId);
        Assert.Equal(2, capacityAfterConfirm);

        // Act 3: Cancel booking before cutoff
        var (cancelSuccess, cancelError) = await _bookingRepository.CancelBookingAndRestoreSeatsAsync(bookingRef!, userId, cutoffHours: 2);
        Assert.True(cancelSuccess, $"Cancellation failed: {cancelError}");

        // Capacity should be restored back to 4
        var capacityAfterCancel = await GetRemainingCapacityAsync(testCategoryId);
        Assert.Equal(4, capacityAfterCancel);
    }

    private async Task<int> EnsureTestUserAsync(string email, string fullName)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            IF EXISTS (SELECT 1 FROM dbo.Users WHERE Email = @Email)
                SELECT UserId FROM dbo.Users WHERE Email = @Email;
            ELSE
            BEGIN
                INSERT INTO dbo.Users (FullName, Email, PasswordHash, PhoneNumber, PreferredCityId, IsActive, CreatedAtUtc)
                VALUES (@FullName, @Email, 'AQAAAAIAAYagAAAAEPlaceholderHashForIntegrationTesting123==', '+919999988888', 1, 1, SYSUTCDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            END";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@FullName", fullName);

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    private async Task<int> CreateTestTicketCategoryAsync(int eventId, string name, decimal price, int capacity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            INSERT INTO dbo.TicketCategories (EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
            VALUES (@EventId, 1, @Name, @Price, @Cap, @Cap, 10, 'Test Tier Perks');
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@EventId", eventId);
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.Parameters.AddWithValue("@Price", price);
        cmd.Parameters.AddWithValue("@Cap", capacity);

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    private async Task<int> GetRemainingCapacityAsync(int categoryId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT RemainingCapacity FROM dbo.TicketCategories WHERE CategoryId = @CategoryId;";
        using var cmd = new SqlCommand(sql, (SqlConnection)connection);
        cmd.Parameters.AddWithValue("@CategoryId", categoryId);

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
}
