namespace TicketBooking.Web.Data.Repositories;

using System.Data;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using TicketBooking.Web.Data;
using TicketBooking.Web.ViewModels.Booking;

public interface IBookingRepository
{
    Task<(bool Success, string? Error, string? BookingReference, DateTime? ExpiresAtUtc)> CreateMovieSeatHoldAsync(
        int userId, int showtimeId, IReadOnlyList<int> seatIds, decimal basePrice, decimal convenienceFee, decimal taxAmount, TimeSpan holdDuration, CancellationToken cancellationToken = default);

    Task<(bool Success, string? Error, string? BookingReference, DateTime? ExpiresAtUtc)> CreateEventTicketHoldAsync(
        int userId, int eventId, int ticketCategoryId, int quantity, decimal basePrice, decimal convenienceFee, decimal taxAmount, TimeSpan holdDuration, CancellationToken cancellationToken = default);

    Task<(bool Success, string? Error, string? TransactionRef)> ConfirmMovieBookingAsync(
        string bookingReference, int userId, string paymentMode, CancellationToken cancellationToken = default);

    Task<bool> ReleaseBookingHoldAsync(
        string bookingReference, string failureReason, CancellationToken cancellationToken = default);

    Task<(bool Success, string? Error)> CancelBookingAndRestoreSeatsAsync(
        string bookingReference, int userId, int cutoffHours, CancellationToken cancellationToken = default);

    Task<CheckoutViewModel?> GetCheckoutSummaryAsync(
        string bookingReference, int userId, CancellationToken cancellationToken = default);

    Task<ConfirmationViewModel?> GetConfirmationDetailsAsync(
        string bookingReference, int userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserBookingItemSummary>> GetUserBookingsAsync(
        int userId, CancellationToken cancellationToken = default);

    Task<int> CleanupExpiredHoldsAsync(CancellationToken cancellationToken = default);
}

public class BookingRepository : IBookingRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<BookingRepository> _logger;

    public BookingRepository(IDbConnectionFactory connectionFactory, ILogger<BookingRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<(bool Success, string? Error, string? BookingReference, DateTime? ExpiresAtUtc)> CreateMovieSeatHoldAsync(
        int userId, int showtimeId, IReadOnlyList<int> seatIds, decimal basePrice, decimal convenienceFee, decimal taxAmount, TimeSpan holdDuration, CancellationToken cancellationToken = default)
    {
        if (seatIds == null || seatIds.Count == 0)
            return (false, "Please select at least one seat.", null, null);

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            // 1. Lock and inspect each requested seat
            var seatIdParams = string.Join(",", seatIds.Select((id, idx) => $"@SeatId{idx}"));
            var checkSql = $@"
                SELECT ss.ShowtimeSeatId, ss.SeatId, s.RowLabel, s.SeatNumber, s.SeatTier, ss.Status, ss.HoldExpiresAtUtc
                FROM dbo.ShowtimeSeats ss WITH (UPDLOCK, HOLDLOCK)
                INNER JOIN dbo.Seats s ON ss.SeatId = s.SeatId
                WHERE ss.ShowtimeId = @ShowtimeId AND ss.SeatId IN ({seatIdParams});";

            using var checkCmd = new SqlCommand(checkSql, connection, transaction);
            checkCmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
            for (int i = 0; i < seatIds.Count; i++)
            {
                checkCmd.Parameters.AddWithValue($"@SeatId{i}", seatIds[i]);
            }

            var lockedSeats = new List<(long ShowtimeSeatId, int SeatId, string Label, string Tier)>();
            using (var reader = await checkCmd.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    var status = reader.GetString(reader.GetOrdinal("Status"));
                    var holdExpires = reader.IsDBNull(reader.GetOrdinal("HoldExpiresAtUtc")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("HoldExpiresAtUtc"));

                    // A seat is available only if Status == 'Available' OR ('Held' AND expired)
                    bool isAvailable = status == "Available" || (status == "Held" && holdExpires.HasValue && holdExpires.Value <= DateTime.UtcNow);

                    if (!isAvailable)
                    {
                        reader.Close();
                        transaction.Rollback();
                        return (false, "One or more selected seats are no longer available. Please select different seats.", null, null);
                    }

                    lockedSeats.Add((
                        reader.GetInt64(reader.GetOrdinal("ShowtimeSeatId")),
                        reader.GetInt32(reader.GetOrdinal("SeatId")),
                        $"{reader.GetString(reader.GetOrdinal("RowLabel"))}{reader.GetInt32(reader.GetOrdinal("SeatNumber"))}",
                        reader.GetString(reader.GetOrdinal("SeatTier"))
                    ));
                }
            }

            if (lockedSeats.Count != seatIds.Count)
            {
                transaction.Rollback();
                return (false, "Some selected seats could not be found for this showtime.", null, null);
            }

            // 2. Generate unique booking reference
            var bookingRef = GenerateBookingReference();
            var expiresAtUtc = DateTime.UtcNow.Add(holdDuration);
            var totalAmount = basePrice + convenienceFee + taxAmount;

            // 3. Create Booking record in Pending state
            const string insertBookingSql = @"
                INSERT INTO dbo.Bookings (BookingReference, UserId, BookingType, ShowtimeId, TotalQuantity, 
                                          BaseAmount, ConvenienceFee, TaxAmount, TotalAmount, Status, CreatedAtUtc, ExpiresAtUtc)
                VALUES (@Ref, @UserId, 'Movie', @ShowtimeId, @Qty, @BaseAmount, @Fee, @Tax, @Total, 'Pending', SYSUTCDATETIME(), @ExpiresAtUtc);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int bookingId;
            using (var insertCmd = new SqlCommand(insertBookingSql, connection, transaction))
            {
                insertCmd.Parameters.AddWithValue("@Ref", bookingRef);
                insertCmd.Parameters.AddWithValue("@UserId", userId);
                insertCmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
                insertCmd.Parameters.AddWithValue("@Qty", seatIds.Count);
                insertCmd.Parameters.AddWithValue("@BaseAmount", basePrice);
                insertCmd.Parameters.AddWithValue("@Fee", convenienceFee);
                insertCmd.Parameters.AddWithValue("@Tax", taxAmount);
                insertCmd.Parameters.AddWithValue("@Total", totalAmount);
                insertCmd.Parameters.AddWithValue("@ExpiresAtUtc", expiresAtUtc);

                bookingId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync(cancellationToken));
            }

            // 4. Update ShowtimeSeats to Held with Expiry
            var updateSeatsSql = $@"
                UPDATE dbo.ShowtimeSeats
                SET Status = 'Held',
                    HoldExpiresAtUtc = @ExpiresAtUtc,
                    BookingId = @BookingId,
                    UpdatedAtUtc = SYSUTCDATETIME()
                WHERE ShowtimeId = @ShowtimeId AND SeatId IN ({seatIdParams});";

            using (var updateCmd = new SqlCommand(updateSeatsSql, connection, transaction))
            {
                updateCmd.Parameters.AddWithValue("@ExpiresAtUtc", expiresAtUtc);
                updateCmd.Parameters.AddWithValue("@BookingId", bookingId);
                updateCmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
                for (int i = 0; i < seatIds.Count; i++)
                {
                    updateCmd.Parameters.AddWithValue($"@SeatId{i}", seatIds[i]);
                }

                await updateCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            // 5. Insert BookingItems
            const string insertItemSql = @"
                INSERT INTO dbo.BookingItems (BookingId, SeatId, SeatLabel, CategoryName, UnitPrice)
                VALUES (@BookingId, @SeatId, @SeatLabel, @CategoryName, @UnitPrice);";

            var unitPrice = basePrice / seatIds.Count;
            foreach (var seat in lockedSeats)
            {
                using var itemCmd = new SqlCommand(insertItemSql, connection, transaction);
                itemCmd.Parameters.AddWithValue("@BookingId", bookingId);
                itemCmd.Parameters.AddWithValue("@SeatId", seat.SeatId);
                itemCmd.Parameters.AddWithValue("@SeatLabel", seat.Label);
                itemCmd.Parameters.AddWithValue("@CategoryName", seat.Tier);
                itemCmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                await itemCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            transaction.Commit();
            _logger.LogInformation("Held {Count} seats for Showtime {ShowtimeId} under BookingRef {Ref} for User {UserId}.",
                seatIds.Count, showtimeId, bookingRef, userId);

            return (true, null, bookingRef, expiresAtUtc);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Transaction failed creating seat hold for Showtime {ShowtimeId}: {Message}", showtimeId, ex.Message);
            return (false, "A temporary error occurred while reserving your seats. Please try again.", null, null);
        }
    }

    public async Task<(bool Success, string? Error, string? BookingReference, DateTime? ExpiresAtUtc)> CreateEventTicketHoldAsync(
        int userId, int eventId, int ticketCategoryId, int quantity, decimal basePrice, decimal convenienceFee, decimal taxAmount, TimeSpan holdDuration, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            return (false, "Please select at least 1 ticket.", null, null);

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            // 1. Lock and inspect the ticket category
            const string checkSql = @"
                SELECT CategoryName, Price, RemainingCapacity, MaxPerBooking
                FROM dbo.TicketCategories WITH (UPDLOCK, HOLDLOCK)
                WHERE CategoryId = @CategoryId AND EventId = @EventId;";

            string categoryName;
            decimal price;
            int remainingCapacity;
            int maxPerBooking;

            using (var checkCmd = new SqlCommand(checkSql, connection, transaction))
            {
                checkCmd.Parameters.AddWithValue("@CategoryId", ticketCategoryId);
                checkCmd.Parameters.AddWithValue("@EventId", eventId);

                using var reader = await checkCmd.ExecuteReaderAsync(cancellationToken);
                if (!await reader.ReadAsync(cancellationToken))
                {
                    reader.Close();
                    transaction.Rollback();
                    return (false, "Selected ticket tier was not found for this event.", null, null);
                }

                categoryName = reader.GetString(0);
                price = reader.GetDecimal(1);
                remainingCapacity = reader.GetInt32(2);
                maxPerBooking = reader.GetInt32(3);
            }

            if (quantity > maxPerBooking)
            {
                transaction.Rollback();
                return (false, $"You can book a maximum of {maxPerBooking} tickets for this tier.", null, null);
            }

            if (remainingCapacity < quantity)
            {
                transaction.Rollback();
                return (false, $"Only {remainingCapacity} tickets remain for {categoryName}. Please choose a smaller quantity.", null, null);
            }

            // 2. Atomic decrement of remaining capacity
            const string updateCapacitySql = @"
                UPDATE dbo.TicketCategories
                SET RemainingCapacity = RemainingCapacity - @Quantity
                WHERE CategoryId = @CategoryId AND RemainingCapacity >= @Quantity;";

            using (var updateCmd = new SqlCommand(updateCapacitySql, connection, transaction))
            {
                updateCmd.Parameters.AddWithValue("@Quantity", quantity);
                updateCmd.Parameters.AddWithValue("@CategoryId", ticketCategoryId);

                var rows = await updateCmd.ExecuteNonQueryAsync(cancellationToken);
                if (rows == 0)
                {
                    transaction.Rollback();
                    return (false, "Selected tickets are no longer available. Please try a different tier.", null, null);
                }
            }

            // 3. Generate unique booking reference and compute totals
            var bookingRef = GenerateBookingReference();
            var expiresAtUtc = DateTime.UtcNow.Add(holdDuration);
            var totalAmount = basePrice + convenienceFee + taxAmount;

            // 4. Create Booking record in Pending state
            const string insertBookingSql = @"
                INSERT INTO dbo.Bookings (BookingReference, UserId, BookingType, EventId, TicketCategoryId, TotalQuantity, 
                                          BaseAmount, ConvenienceFee, TaxAmount, TotalAmount, Status, CreatedAtUtc, ExpiresAtUtc)
                VALUES (@Ref, @UserId, 'Event', @EventId, @CategoryId, @Qty, @BaseAmount, @Fee, @Tax, @Total, 'Pending', SYSUTCDATETIME(), @ExpiresAtUtc);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int bookingId;
            using (var insertCmd = new SqlCommand(insertBookingSql, connection, transaction))
            {
                insertCmd.Parameters.AddWithValue("@Ref", bookingRef);
                insertCmd.Parameters.AddWithValue("@UserId", userId);
                insertCmd.Parameters.AddWithValue("@EventId", eventId);
                insertCmd.Parameters.AddWithValue("@CategoryId", ticketCategoryId);
                insertCmd.Parameters.AddWithValue("@Qty", quantity);
                insertCmd.Parameters.AddWithValue("@BaseAmount", basePrice);
                insertCmd.Parameters.AddWithValue("@Fee", convenienceFee);
                insertCmd.Parameters.AddWithValue("@Tax", taxAmount);
                insertCmd.Parameters.AddWithValue("@Total", totalAmount);
                insertCmd.Parameters.AddWithValue("@ExpiresAtUtc", expiresAtUtc);

                bookingId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync(cancellationToken));
            }

            // 5. Insert BookingItems
            const string insertItemSql = @"
                INSERT INTO dbo.BookingItems (BookingId, SeatId, SeatLabel, TicketCategoryId, CategoryName, UnitPrice)
                VALUES (@BookingId, NULL, @SeatLabel, @CategoryId, @CategoryName, @UnitPrice);";

            var unitPrice = basePrice / quantity;
            var safeCategoryLabel = categoryName.Length > 80 ? categoryName[..80] : categoryName;
            for (int i = 1; i <= quantity; i++)
            {
                using var itemCmd = new SqlCommand(insertItemSql, connection, transaction);
                itemCmd.Parameters.AddWithValue("@BookingId", bookingId);
                itemCmd.Parameters.AddWithValue("@SeatLabel", $"{safeCategoryLabel} #{i}");
                itemCmd.Parameters.AddWithValue("@CategoryId", ticketCategoryId);
                itemCmd.Parameters.AddWithValue("@CategoryName", categoryName);
                itemCmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                await itemCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            transaction.Commit();
            _logger.LogInformation("Held {Quantity} tickets for Event {EventId} Category {CategoryId} under BookingRef {Ref} for User {UserId}.",
                quantity, eventId, ticketCategoryId, bookingRef, userId);

            return (true, null, bookingRef, expiresAtUtc);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Transaction failed creating ticket hold for Event {EventId}: {Message}", eventId, ex.Message);
            return (false, "A temporary error occurred while reserving your tickets. Please try again.", null, null);
        }
    }

    public async Task<(bool Success, string? Error, string? TransactionRef)> ConfirmMovieBookingAsync(
        string bookingReference, int userId, string paymentMode, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);

        try
        {
            const string lockBookingSql = @"
                SELECT BookingId, Status, TotalAmount, ExpiresAtUtc
                FROM dbo.Bookings WITH (UPDLOCK)
                WHERE BookingReference = @Ref AND UserId = @UserId;";

            int bookingId;
            string status;
            decimal totalAmount;
            DateTime expiresAtUtc;

            using (var cmd = new SqlCommand(lockBookingSql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Ref", bookingReference);
                cmd.Parameters.AddWithValue("@UserId", userId);

                using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                if (!await reader.ReadAsync(cancellationToken))
                {
                    reader.Close();
                    transaction.Rollback();
                    return (false, "Booking reservation not found or access denied.", null);
                }

                bookingId = reader.GetInt32(0);
                status = reader.GetString(1);
                totalAmount = reader.GetDecimal(2);
                expiresAtUtc = reader.GetDateTime(3);
            }

            if (status == "Confirmed")
            {
                transaction.Rollback();
                return (true, null, "TXN-EXISTING");
            }

            if (status != "Pending")
            {
                transaction.Rollback();
                return (false, $"Booking cannot be confirmed because status is {status}.", null);
            }

            if (expiresAtUtc <= DateTime.UtcNow)
            {
                // Expired hold: release inventory and mark expired
                const string markExpiredSql = @"
                    UPDATE dbo.Bookings SET Status = 'Expired' WHERE BookingId = @BookingId;
                    UPDATE dbo.ShowtimeSeats SET Status = 'Available', HoldExpiresAtUtc = NULL, BookingId = NULL WHERE BookingId = @BookingId;";

                using var expireCmd = new SqlCommand(markExpiredSql, connection, transaction);
                expireCmd.Parameters.AddWithValue("@BookingId", bookingId);
                await expireCmd.ExecuteNonQueryAsync(cancellationToken);

                transaction.Commit();
                return (false, "The 8-minute reservation time for these seats has expired. Please select your seats again.", null);
            }

            // Confirm Booking
            const string confirmSql = @"
                UPDATE dbo.Bookings
                SET Status = 'Confirmed', ConfirmedAtUtc = SYSUTCDATETIME()
                WHERE BookingId = @BookingId;

                UPDATE dbo.ShowtimeSeats
                SET Status = 'Booked', HoldExpiresAtUtc = NULL
                WHERE BookingId = @BookingId;";

            using (var confirmCmd = new SqlCommand(confirmSql, connection, transaction))
            {
                confirmCmd.Parameters.AddWithValue("@BookingId", bookingId);
                await confirmCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            // Create Payment record
            var txnRef = "TXN-" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
            const string insertPaymentSql = @"
                INSERT INTO dbo.Payments (BookingId, TransactionReference, PaymentGateway, PaymentMode, Amount, Status, ProcessedAtUtc)
                VALUES (@BookingId, @TxnRef, 'SimulatedGateway', @Mode, @Amount, 'Success', SYSUTCDATETIME());";

            using (var payCmd = new SqlCommand(insertPaymentSql, connection, transaction))
            {
                payCmd.Parameters.AddWithValue("@BookingId", bookingId);
                payCmd.Parameters.AddWithValue("@TxnRef", txnRef);
                payCmd.Parameters.AddWithValue("@Mode", paymentMode);
                payCmd.Parameters.AddWithValue("@Amount", totalAmount);
                await payCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            transaction.Commit();
            _logger.LogInformation("Confirmed Booking {Ref} (ID: {Id}) with Txn {TxnRef}.", bookingReference, bookingId, txnRef);
            return (true, null, txnRef);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Failed to confirm booking {Ref}: {Message}", bookingReference, ex.Message);
            return (false, "Payment confirmation failed due to a database error.", null);
        }
    }

    public async Task<bool> ReleaseBookingHoldAsync(
        string bookingReference, string failureReason, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            const string sql = @"
                DECLARE @BookingId INT, @Amount DECIMAL(10,2);
                SELECT @BookingId = BookingId, @Amount = TotalAmount 
                FROM dbo.Bookings WITH (UPDLOCK) 
                WHERE BookingReference = @Ref AND Status = 'Pending';

                IF @BookingId IS NOT NULL
                BEGIN
                    UPDATE dbo.Bookings 
                    SET Status = 'Failed', CancellationReason = @Reason
                    WHERE BookingId = @BookingId;

                    -- Release movie seats
                    UPDATE dbo.ShowtimeSeats
                    SET Status = 'Available', HoldExpiresAtUtc = NULL, BookingId = NULL
                    WHERE BookingId = @BookingId;

                    -- Release event capacity if applicable
                    UPDATE tc
                    SET tc.RemainingCapacity = tc.RemainingCapacity + b.TotalQuantity
                    FROM dbo.TicketCategories tc
                    INNER JOIN dbo.Bookings b ON tc.CategoryId = b.TicketCategoryId
                    WHERE b.BookingId = @BookingId;

                    -- Record failed payment
                    INSERT INTO dbo.Payments (BookingId, TransactionReference, PaymentGateway, PaymentMode, Amount, Status, FailureReason, ProcessedAtUtc)
                    VALUES (@BookingId, 'FAIL-' + SUBSTRING(CONVERT(VARCHAR(40), NEWID()), 1, 10), 'SimulatedGateway', 'Simulated', @Amount, 'Failed', @Reason, SYSUTCDATETIME());
                END";

            using var cmd = new SqlCommand(sql, connection, transaction);
            cmd.Parameters.AddWithValue("@Ref", bookingReference);
            cmd.Parameters.AddWithValue("@Reason", failureReason);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            transaction.Commit();

            _logger.LogInformation("Released inventory for failed Booking {Ref}. Reason: {Reason}", bookingReference, failureReason);
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Failed releasing hold for Booking {Ref}: {Message}", bookingReference, ex.Message);
            return false;
        }
    }

    public async Task<(bool Success, string? Error)> CancelBookingAndRestoreSeatsAsync(
        string bookingReference, int userId, int cutoffHours, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            const string lookupSql = @"
                SELECT b.BookingId, b.BookingType, b.Status, s.StartTime AS MovieStart, e.StartDate AS EventStart
                FROM dbo.Bookings b WITH (UPDLOCK)
                LEFT JOIN dbo.Showtimes s ON b.ShowtimeId = s.ShowtimeId
                LEFT JOIN dbo.Events e ON b.EventId = e.EventId
                WHERE b.BookingReference = @Ref AND b.UserId = @UserId;";

            int bookingId;
            string bookingType;
            string status;
            DateTime? eventStart;

            using (var cmd = new SqlCommand(lookupSql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Ref", bookingReference);
                cmd.Parameters.AddWithValue("@UserId", userId);

                using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                if (!await reader.ReadAsync(cancellationToken))
                {
                    reader.Close();
                    transaction.Rollback();
                    return (false, "Booking record not found or access unauthorized.");
                }

                bookingId = reader.GetInt32(0);
                bookingType = reader.GetString(1);
                status = reader.GetString(2);
                eventStart = bookingType == "Movie" 
                    ? (reader.IsDBNull(3) ? null : reader.GetDateTime(3))
                    : (reader.IsDBNull(4) ? null : reader.GetDateTime(4));
            }

            if (status != "Confirmed")
            {
                transaction.Rollback();
                return (false, $"Only confirmed bookings can be cancelled. Current status is {status}.");
            }

            if (eventStart.HasValue)
            {
                var cutoffTime = eventStart.Value.AddHours(-cutoffHours);
                if (DateTime.UtcNow > cutoffTime)
                {
                    transaction.Rollback();
                    return (false, $"Cancellations are only permitted up to {cutoffHours} hours prior to the show start time.");
                }
            }

            // Cancel booking and restore inventory
            const string cancelSql = @"
                UPDATE dbo.Bookings
                SET Status = 'Cancelled', CancelledAtUtc = SYSUTCDATETIME(), CancellationReason = 'Cancelled by user'
                WHERE BookingId = @BookingId;

                -- Restore movie seats
                UPDATE dbo.ShowtimeSeats
                SET Status = 'Available', HoldExpiresAtUtc = NULL, BookingId = NULL
                WHERE BookingId = @BookingId;

                -- Restore event tickets
                UPDATE tc
                SET tc.RemainingCapacity = tc.RemainingCapacity + b.TotalQuantity
                FROM dbo.TicketCategories tc
                INNER JOIN dbo.Bookings b ON tc.CategoryId = b.TicketCategoryId
                WHERE b.BookingId = @BookingId;";

            using (var cancelCmd = new SqlCommand(cancelSql, connection, transaction))
            {
                cancelCmd.Parameters.AddWithValue("@BookingId", bookingId);
                await cancelCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            transaction.Commit();
            _logger.LogInformation("Cancelled Booking {Ref} and restored inventory.", bookingReference);
            return (true, null);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Failed cancelling booking {Ref}: {Message}", bookingReference, ex.Message);
            return (false, "An error occurred while cancelling your booking.");
        }
    }

    public async Task<CheckoutViewModel?> GetCheckoutSummaryAsync(
        string bookingReference, int userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT b.BookingReference, b.BookingType, b.TotalQuantity, b.BaseAmount, b.ConvenienceFee, b.TaxAmount, b.TotalAmount, b.ExpiresAtUtc,
                   COALESCE(m.Title, e.Title) AS ItemTitle,
                   COALESCE(t.TheatreName, v.VenueName) AS VenueName,
                   COALESCE(sc.ScreenName, tc.CategoryName) AS SubVenue,
                   COALESCE(s.StartTime, e.StartDate) AS EventDateTime
            FROM dbo.Bookings b
            LEFT JOIN dbo.Showtimes s ON b.ShowtimeId = s.ShowtimeId
            LEFT JOIN dbo.Movies m ON s.MovieId = m.MovieId
            LEFT JOIN dbo.Screens sc ON s.ScreenId = sc.ScreenId
            LEFT JOIN dbo.Theatres t ON sc.TheatreId = t.TheatreId
            LEFT JOIN dbo.Events e ON b.EventId = e.EventId
            LEFT JOIN dbo.Venues v ON e.VenueId = v.VenueId
            LEFT JOIN dbo.TicketCategories tc ON b.TicketCategoryId = tc.CategoryId
            WHERE b.BookingReference = @Ref AND b.UserId = @UserId;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Ref", bookingReference);
        command.Parameters.AddWithValue("@UserId", userId);

        CheckoutViewModel? summary = null;
        using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            if (await reader.ReadAsync(cancellationToken))
            {
                summary = new CheckoutViewModel
                {
                    BookingReference = reader.GetString(reader.GetOrdinal("BookingReference")),
                    BookingType = reader.GetString(reader.GetOrdinal("BookingType")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("TotalQuantity")),
                    BaseAmount = reader.GetDecimal(reader.GetOrdinal("BaseAmount")),
                    ConvenienceFee = reader.GetDecimal(reader.GetOrdinal("ConvenienceFee")),
                    TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    ExpiresAtUtc = reader.GetDateTime(reader.GetOrdinal("ExpiresAtUtc")),
                    ItemTitle = reader.GetString(reader.GetOrdinal("ItemTitle")),
                    VenueName = reader.GetString(reader.GetOrdinal("VenueName")),
                    SubVenue = reader.IsDBNull(reader.GetOrdinal("SubVenue")) ? null : reader.GetString(reader.GetOrdinal("SubVenue")),
                    EventDateTime = reader.GetDateTime(reader.GetOrdinal("EventDateTime"))
                };
            }
        }

        if (summary != null)
        {
            // Fetch seat or ticket labels
            const string itemsSql = @"
                SELECT bi.SeatLabel, bi.CategoryName
                FROM dbo.BookingItems bi
                INNER JOIN dbo.Bookings b ON bi.BookingId = b.BookingId
                WHERE b.BookingReference = @Ref;";

            using var itemsCmd = new SqlCommand(itemsSql, connection);
            itemsCmd.Parameters.AddWithValue("@Ref", bookingReference);

            using var itemReader = await itemsCmd.ExecuteReaderAsync(cancellationToken);
            while (await itemReader.ReadAsync(cancellationToken))
            {
                summary.SeatOrTicketLabels.Add(itemReader.GetString(0));
            }
        }

        return summary;
    }

    public async Task<ConfirmationViewModel?> GetConfirmationDetailsAsync(
        string bookingReference, int userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT b.BookingReference, b.BookingType, b.TotalQuantity, b.TotalAmount, b.Status, b.ConfirmedAtUtc,
                   COALESCE(m.Title, e.Title) AS ItemTitle,
                   COALESCE(t.TheatreName, v.VenueName) AS VenueName,
                   COALESCE(sc.ScreenName, tc.CategoryName) AS SubVenue,
                   COALESCE(s.StartTime, e.StartDate) AS EventDateTime,
                   p.PaymentMode, p.TransactionReference
            FROM dbo.Bookings b
            LEFT JOIN dbo.Showtimes s ON b.ShowtimeId = s.ShowtimeId
            LEFT JOIN dbo.Movies m ON s.MovieId = m.MovieId
            LEFT JOIN dbo.Screens sc ON s.ScreenId = sc.ScreenId
            LEFT JOIN dbo.Theatres t ON sc.TheatreId = t.TheatreId
            LEFT JOIN dbo.Events e ON b.EventId = e.EventId
            LEFT JOIN dbo.Venues v ON e.VenueId = v.VenueId
            LEFT JOIN dbo.TicketCategories tc ON b.TicketCategoryId = tc.CategoryId
            LEFT JOIN dbo.Payments p ON b.BookingId = p.BookingId AND p.Status = 'Success'
            WHERE b.BookingReference = @Ref AND b.UserId = @UserId;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Ref", bookingReference);
        command.Parameters.AddWithValue("@UserId", userId);

        ConfirmationViewModel? confirmation = null;
        using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            if (await reader.ReadAsync(cancellationToken))
            {
                confirmation = new ConfirmationViewModel
                {
                    BookingReference = reader.GetString(reader.GetOrdinal("BookingReference")),
                    BookingType = reader.GetString(reader.GetOrdinal("BookingType")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("TotalQuantity")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    ConfirmedAtUtc = reader.IsDBNull(reader.GetOrdinal("ConfirmedAtUtc")) ? DateTime.UtcNow : reader.GetDateTime(reader.GetOrdinal("ConfirmedAtUtc")),
                    ItemTitle = reader.GetString(reader.GetOrdinal("ItemTitle")),
                    VenueName = reader.GetString(reader.GetOrdinal("VenueName")),
                    SubVenue = reader.IsDBNull(reader.GetOrdinal("SubVenue")) ? null : reader.GetString(reader.GetOrdinal("SubVenue")),
                    EventDateTime = reader.GetDateTime(reader.GetOrdinal("EventDateTime")),
                    PaymentMode = reader.IsDBNull(reader.GetOrdinal("PaymentMode")) ? "Simulated" : reader.GetString(reader.GetOrdinal("PaymentMode")),
                    TransactionReference = reader.IsDBNull(reader.GetOrdinal("TransactionReference")) ? "TXN-DEMO" : reader.GetString(reader.GetOrdinal("TransactionReference"))
                };
            }
        }

        if (confirmation != null)
        {
            const string itemsSql = @"
                SELECT bi.SeatLabel
                FROM dbo.BookingItems bi
                INNER JOIN dbo.Bookings b ON bi.BookingId = b.BookingId
                WHERE b.BookingReference = @Ref;";

            using var itemsCmd = new SqlCommand(itemsSql, connection);
            itemsCmd.Parameters.AddWithValue("@Ref", bookingReference);

            using var itemReader = await itemsCmd.ExecuteReaderAsync(cancellationToken);
            while (await itemReader.ReadAsync(cancellationToken))
            {
                confirmation.SeatOrTicketLabels.Add(itemReader.GetString(0));
            }
        }

        return confirmation;
    }

    public async Task<IReadOnlyList<UserBookingItemSummary>> GetUserBookingsAsync(
        int userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT b.BookingId, b.BookingReference, b.BookingType, b.TotalQuantity, b.TotalAmount, b.Status, b.CreatedAtUtc,
                   COALESCE(m.Title, e.Title) AS ItemTitle,
                   COALESCE(t.TheatreName, v.VenueName) AS VenueName,
                   COALESCE(s.StartTime, e.StartDate) AS EventDateTime
            FROM dbo.Bookings b
            LEFT JOIN dbo.Showtimes s ON b.ShowtimeId = s.ShowtimeId
            LEFT JOIN dbo.Movies m ON s.MovieId = m.MovieId
            LEFT JOIN dbo.Screens sc ON s.ScreenId = sc.ScreenId
            LEFT JOIN dbo.Theatres t ON sc.TheatreId = t.TheatreId
            LEFT JOIN dbo.Events e ON b.EventId = e.EventId
            LEFT JOIN dbo.Venues v ON e.VenueId = v.VenueId
            WHERE b.UserId = @UserId
            ORDER BY b.CreatedAtUtc DESC;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UserId", userId);

        var list = new List<UserBookingItemSummary>();
        using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                var eventDate = reader.GetDateTime(reader.GetOrdinal("EventDateTime"));
                var status = reader.GetString(reader.GetOrdinal("Status"));

                list.Add(new UserBookingItemSummary
                {
                    BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                    BookingReference = reader.GetString(reader.GetOrdinal("BookingReference")),
                    BookingType = reader.GetString(reader.GetOrdinal("BookingType")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("TotalQuantity")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    Status = status,
                    CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                    ItemTitle = reader.GetString(reader.GetOrdinal("ItemTitle")),
                    VenueName = reader.GetString(reader.GetOrdinal("VenueName")),
                    EventDateTime = eventDate,
                    CanCancel = status == "Confirmed" && DateTime.UtcNow < eventDate.AddHours(-2)
                });
            }
        }

        // Fetch seat labels for each booking
        foreach (var item in list)
        {
            const string itemSql = "SELECT SeatLabel FROM dbo.BookingItems WHERE BookingId = @BookingId;";
            using var itemCmd = new SqlCommand(itemSql, connection);
            itemCmd.Parameters.AddWithValue("@BookingId", item.BookingId);

            using var itemReader = await itemCmd.ExecuteReaderAsync(cancellationToken);
            while (await itemReader.ReadAsync(cancellationToken))
            {
                item.SeatLabels.Add(itemReader.GetString(0));
            }
        }

        return list;
    }

    public async Task<int> CleanupExpiredHoldsAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            DECLARE @ExpiredBookings TABLE (BookingId INT);

            UPDATE dbo.Bookings
            SET Status = 'Expired'
            OUTPUT INSERTED.BookingId INTO @ExpiredBookings
            WHERE Status = 'Pending' AND ExpiresAtUtc <= SYSUTCDATETIME();

            UPDATE ss
            SET ss.Status = 'Available', ss.HoldExpiresAtUtc = NULL, ss.BookingId = NULL
            FROM dbo.ShowtimeSeats ss
            INNER JOIN @ExpiredBookings eb ON ss.BookingId = eb.BookingId;

            -- Restore event inventory if any
            UPDATE tc
            SET tc.RemainingCapacity = tc.RemainingCapacity + b.TotalQuantity
            FROM dbo.TicketCategories tc
            INNER JOIN dbo.Bookings b ON tc.CategoryId = b.TicketCategoryId
            INNER JOIN @ExpiredBookings eb ON b.BookingId = eb.BookingId;

            SELECT COUNT(*) FROM @ExpiredBookings;";

        using var command = new SqlCommand(sql, connection);
        var cleaned = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        return cleaned;
    }

    private static string GenerateBookingReference()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var bytes = RandomNumberGenerator.GetBytes(6);
        var result = new char[6];
        for (int i = 0; i < 6; i++)
        {
            result[i] = chars[bytes[i] % chars.Length];
        }
        return $"SSP-2026-{new string(result)}";
    }
}
