namespace TicketBooking.Web.Data.Repositories;

using System.Data;
using Microsoft.Data.SqlClient;
using TicketBooking.Web.Data;
using TicketBooking.Web.Models;

public interface IShowtimeRepository
{
    Task<IReadOnlyList<Showtime>> GetShowtimesByMovieAndDateAsync(int movieId, DateTime date, string? city, CancellationToken cancellationToken = default);
    Task<Showtime?> GetByIdAsync(int showtimeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShowtimeSeat>> GetSeatMatrixWithStatusAsync(int showtimeId, CancellationToken cancellationToken = default);
    Task EnsureShowtimeSeatsPopulatedAsync(int showtimeId, int screenId, CancellationToken cancellationToken = default);
    Task<int> CreateShowtimeAsync(Showtime showtime, CancellationToken cancellationToken = default);
}

public class ShowtimeRepository : IShowtimeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ShowtimeRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Showtime>> GetShowtimesByMovieAndDateAsync(
        int movieId, DateTime date, string? city, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        var sql = @"
            SELECT s.ShowtimeId, s.MovieId, m.Title AS MovieTitle, s.ScreenId, sc.ScreenName, sc.ScreenType,
                   t.TheatreId, t.TheatreName, t.Address AS TheatreAddress, s.StartTime, s.EndTime,
                   s.SilverPrice, s.GoldPrice, s.PlatinumPrice, s.IsActive
            FROM dbo.Showtimes s
            INNER JOIN dbo.Movies m ON s.MovieId = m.MovieId
            INNER JOIN dbo.Screens sc ON s.ScreenId = sc.ScreenId
            INNER JOIN dbo.Theatres t ON sc.TheatreId = t.TheatreId
            LEFT JOIN dbo.Cities c ON t.CityId = c.CityId
            WHERE s.MovieId = @MovieId 
              AND s.IsActive = 1
              AND CAST(s.StartTime AS DATE) = CAST(@Date AS DATE) ";

        if (!string.IsNullOrWhiteSpace(city))
        {
            sql += " AND c.CityName = @City ";
        }

        sql += " ORDER BY t.TheatreName, s.StartTime ASC;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@MovieId", movieId);
        command.Parameters.AddWithValue("@Date", date.Date);

        if (!string.IsNullOrWhiteSpace(city))
            command.Parameters.AddWithValue("@City", city);

        var list = new List<Showtime>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(MapShowtime(reader));
        }

        return list;
    }

    public async Task<Showtime?> GetByIdAsync(int showtimeId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT s.ShowtimeId, s.MovieId, m.Title AS MovieTitle, s.ScreenId, sc.ScreenName, sc.ScreenType,
                   t.TheatreId, t.TheatreName, t.Address AS TheatreAddress, s.StartTime, s.EndTime,
                   s.SilverPrice, s.GoldPrice, s.PlatinumPrice, s.IsActive
            FROM dbo.Showtimes s
            INNER JOIN dbo.Movies m ON s.MovieId = m.MovieId
            INNER JOIN dbo.Screens sc ON s.ScreenId = sc.ScreenId
            INNER JOIN dbo.Theatres t ON sc.TheatreId = t.TheatreId
            WHERE s.ShowtimeId = @ShowtimeId;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@ShowtimeId", showtimeId);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return MapShowtime(reader);
        }

        return null;
    }

    public async Task EnsureShowtimeSeatsPopulatedAsync(int showtimeId, int screenId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            IF NOT EXISTS (SELECT 1 FROM dbo.ShowtimeSeats WHERE ShowtimeId = @ShowtimeId)
            BEGIN
                INSERT INTO dbo.ShowtimeSeats (ShowtimeId, SeatId, Status)
                SELECT @ShowtimeId, s.SeatId, 'Available'
                FROM dbo.Seats s
                WHERE s.ScreenId = @ScreenId AND s.IsActive = 1;
            END";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@ShowtimeId", showtimeId);
        command.Parameters.AddWithValue("@ScreenId", screenId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShowtimeSeat>> GetSeatMatrixWithStatusAsync(
        int showtimeId, CancellationToken cancellationToken = default)
    {
        // First ensure showtime has seat inventory populated
        var showtime = await GetByIdAsync(showtimeId, cancellationToken);
        if (showtime != null)
        {
            await EnsureShowtimeSeatsPopulatedAsync(showtimeId, showtime.ScreenId, cancellationToken);
        }

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT ss.ShowtimeSeatId, ss.ShowtimeId, s.SeatId, s.RowLabel, s.SeatNumber, s.SeatTier,
                   CASE 
                       WHEN ss.Status = 'Booked' THEN 'Booked'
                       WHEN ss.Status = 'Held' AND ss.HoldExpiresAtUtc > SYSUTCDATETIME() THEN 'Held'
                       ELSE 'Available'
                   END AS EffectiveStatus,
                   ss.HoldExpiresAtUtc, ss.BookingId,
                   CASE s.SeatTier
                       WHEN 'Silver' THEN sh.SilverPrice
                       WHEN 'Gold' THEN sh.GoldPrice
                       WHEN 'Platinum' THEN sh.PlatinumPrice
                       ELSE sh.GoldPrice
                   END AS Price
            FROM dbo.Seats s
            INNER JOIN dbo.Showtimes sh ON sh.ShowtimeId = @ShowtimeId
            LEFT JOIN dbo.ShowtimeSeats ss ON s.SeatId = ss.SeatId AND ss.ShowtimeId = @ShowtimeId
            WHERE s.ScreenId = sh.ScreenId AND s.IsActive = 1
            ORDER BY s.RowLabel ASC, s.SeatNumber ASC;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@ShowtimeId", showtimeId);

        var list = new List<ShowtimeSeat>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(new ShowtimeSeat
            {
                ShowtimeSeatId = reader.IsDBNull(reader.GetOrdinal("ShowtimeSeatId")) ? 0 : reader.GetInt64(reader.GetOrdinal("ShowtimeSeatId")),
                ShowtimeId = showtimeId,
                SeatId = reader.GetInt32(reader.GetOrdinal("SeatId")),
                RowLabel = reader.GetString(reader.GetOrdinal("RowLabel")),
                SeatNumber = reader.GetInt32(reader.GetOrdinal("SeatNumber")),
                SeatTier = reader.GetString(reader.GetOrdinal("SeatTier")),
                Status = reader.GetString(reader.GetOrdinal("EffectiveStatus")),
                HoldExpiresAtUtc = reader.IsDBNull(reader.GetOrdinal("HoldExpiresAtUtc")) ? null : reader.GetDateTime(reader.GetOrdinal("HoldExpiresAtUtc")),
                BookingId = reader.IsDBNull(reader.GetOrdinal("BookingId")) ? null : reader.GetInt32(reader.GetOrdinal("BookingId")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price"))
            });
        }

        return list;
    }

    public async Task<int> CreateShowtimeAsync(Showtime showtime, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
            VALUES (@MovieId, @ScreenId, @StartTime, @EndTime, @SilverPrice, @GoldPrice, @PlatinumPrice, @IsActive, SYSUTCDATETIME());
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@MovieId", showtime.MovieId);
        command.Parameters.AddWithValue("@ScreenId", showtime.ScreenId);
        command.Parameters.AddWithValue("@StartTime", showtime.StartTime);
        command.Parameters.AddWithValue("@EndTime", showtime.EndTime);
        command.Parameters.AddWithValue("@SilverPrice", showtime.SilverPrice);
        command.Parameters.AddWithValue("@GoldPrice", showtime.GoldPrice);
        command.Parameters.AddWithValue("@PlatinumPrice", showtime.PlatinumPrice);
        command.Parameters.AddWithValue("@IsActive", showtime.IsActive);

        var id = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));

        // Automatically populate seats for this showtime
        await EnsureShowtimeSeatsPopulatedAsync(id, showtime.ScreenId, cancellationToken);

        return id;
    }

    private static Showtime MapShowtime(SqlDataReader reader)
    {
        return new Showtime
        {
            ShowtimeId = reader.GetInt32(reader.GetOrdinal("ShowtimeId")),
            MovieId = reader.GetInt32(reader.GetOrdinal("MovieId")),
            MovieTitle = reader.GetString(reader.GetOrdinal("MovieTitle")),
            ScreenId = reader.GetInt32(reader.GetOrdinal("ScreenId")),
            ScreenName = reader.GetString(reader.GetOrdinal("ScreenName")),
            ScreenType = reader.GetString(reader.GetOrdinal("ScreenType")),
            TheatreId = reader.GetInt32(reader.GetOrdinal("TheatreId")),
            TheatreName = reader.GetString(reader.GetOrdinal("TheatreName")),
            TheatreAddress = reader.GetString(reader.GetOrdinal("TheatreAddress")),
            StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
            EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
            SilverPrice = reader.GetDecimal(reader.GetOrdinal("SilverPrice")),
            GoldPrice = reader.GetDecimal(reader.GetOrdinal("GoldPrice")),
            PlatinumPrice = reader.GetDecimal(reader.GetOrdinal("PlatinumPrice")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };
    }
}
