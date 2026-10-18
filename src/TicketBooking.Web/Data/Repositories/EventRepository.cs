namespace TicketBooking.Web.Data.Repositories;

using System.Data;
using Microsoft.Data.SqlClient;
using TicketBooking.Web.Models;

public class EventRepository : IEventRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<EventRepository> _logger;

    public EventRepository(IDbConnectionFactory connectionFactory, ILogger<EventRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<EventItem?> GetByIdAsync(int eventId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT 
                e.EventId, e.Title, e.Description, e.Category, e.CityId, e.VenueId, e.BannerUrl, 
                e.StartDate, e.EndDate, e.Status, e.CreatedAtUtc, e.UpdatedAtUtc,
                c.CityName, 
                ISNULL(v.VenueName, 'TBA') AS VenueName, 
                ISNULL(v.Address, '') AS VenueAddress,
                ISNULL(MIN(tc.Price), 0.00) AS StartingPrice,
                ISNULL(SUM(tc.RemainingCapacity), 0) AS TotalRemainingTickets
            FROM dbo.Events e
            INNER JOIN dbo.Cities c ON e.CityId = c.CityId
            LEFT JOIN dbo.Venues v ON e.VenueId = v.VenueId
            LEFT JOIN dbo.TicketCategories tc ON e.EventId = tc.EventId
            WHERE e.EventId = @EventId
            GROUP BY 
                e.EventId, e.Title, e.Description, e.Category, e.CityId, e.VenueId, e.BannerUrl, 
                e.StartDate, e.EndDate, e.Status, e.CreatedAtUtc, e.UpdatedAtUtc,
                c.CityName, v.VenueName, v.Address;";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@EventId", eventId);

        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return MapEventFromReader(reader);
        }

        return null;
    }

    public async Task<IReadOnlyList<EventItem>> GetEventsAsync(
        string? category, 
        string? city, 
        string? search, 
        CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var sql = @"
            SELECT 
                e.EventId, e.Title, e.Description, e.Category, e.CityId, e.VenueId, e.BannerUrl, 
                e.StartDate, e.EndDate, e.Status, e.CreatedAtUtc, e.UpdatedAtUtc,
                c.CityName, 
                ISNULL(v.VenueName, 'TBA') AS VenueName, 
                ISNULL(v.Address, '') AS VenueAddress,
                ISNULL(MIN(tc.Price), 0.00) AS StartingPrice,
                ISNULL(SUM(tc.RemainingCapacity), 0) AS TotalRemainingTickets
            FROM dbo.Events e
            INNER JOIN dbo.Cities c ON e.CityId = c.CityId
            LEFT JOIN dbo.Venues v ON e.VenueId = v.VenueId
            LEFT JOIN dbo.TicketCategories tc ON e.EventId = tc.EventId
            WHERE 1=1 ";

        var parameters = new List<SqlParameter>();

        if (!string.IsNullOrWhiteSpace(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            sql += " AND e.Category = @Category ";
            parameters.Add(new SqlParameter("@Category", SqlDbType.NVarChar, 50) { Value = category });
        }

        if (!string.IsNullOrWhiteSpace(city) && !city.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            sql += " AND c.CityName = @City ";
            parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar, 100) { Value = city });
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            sql += " AND (e.Title LIKE @Search OR e.Description LIKE @Search OR ISNULL(v.VenueName, '') LIKE @Search) ";
            parameters.Add(new SqlParameter("@Search", SqlDbType.NVarChar, 250) { Value = $"%{search.Trim()}%" });
        }

        sql += @"
            GROUP BY 
                e.EventId, e.Title, e.Description, e.Category, e.CityId, e.VenueId, e.BannerUrl, 
                e.StartDate, e.EndDate, e.Status, e.CreatedAtUtc, e.UpdatedAtUtc,
                c.CityName, v.VenueName, v.Address
            ORDER BY e.StartDate ASC;";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddRange(parameters.ToArray());

        var list = new List<EventItem>();
        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(MapEventFromReader(reader));
        }

        return list;
    }

    public async Task<IReadOnlyList<TicketCategory>> GetCategoriesByEventIdAsync(int eventId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT 
                CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks
            FROM dbo.TicketCategories
            WHERE EventId = @EventId
            ORDER BY Price ASC;";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@EventId", eventId);

        var list = new List<TicketCategory>();
        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(new TicketCategory
            {
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                SessionId = reader.IsDBNull(reader.GetOrdinal("SessionId")) ? null : reader.GetInt32(reader.GetOrdinal("SessionId")),
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                TotalCapacity = reader.GetInt32(reader.GetOrdinal("TotalCapacity")),
                RemainingCapacity = reader.GetInt32(reader.GetOrdinal("RemainingCapacity")),
                MaxPerBooking = reader.GetInt32(reader.GetOrdinal("MaxPerBooking")),
                Perks = reader.IsDBNull(reader.GetOrdinal("Perks")) ? null : reader.GetString(reader.GetOrdinal("Perks"))
            });
        }

        return list;
    }

    public async Task<IReadOnlyList<EventSession>> GetSessionsByEventIdAsync(int eventId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT 
                SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive
            FROM dbo.EventSessions
            WHERE EventId = @EventId AND IsActive = 1
            ORDER BY StartTime ASC;";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@EventId", eventId);

        var list = new List<EventSession>();
        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(new EventSession
            {
                SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                SessionTitle = reader.GetString(reader.GetOrdinal("SessionTitle")),
                StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            });
        }

        return list;
    }

    public async Task<TicketCategory?> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT 
                CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks
            FROM dbo.TicketCategories
            WHERE CategoryId = @CategoryId;";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@CategoryId", categoryId);

        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return new TicketCategory
            {
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                SessionId = reader.IsDBNull(reader.GetOrdinal("SessionId")) ? null : reader.GetInt32(reader.GetOrdinal("SessionId")),
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                TotalCapacity = reader.GetInt32(reader.GetOrdinal("TotalCapacity")),
                RemainingCapacity = reader.GetInt32(reader.GetOrdinal("RemainingCapacity")),
                MaxPerBooking = reader.GetInt32(reader.GetOrdinal("MaxPerBooking")),
                Perks = reader.IsDBNull(reader.GetOrdinal("Perks")) ? null : reader.GetString(reader.GetOrdinal("Perks"))
            };
        }

        return null;
    }

    public async Task<IReadOnlyList<string>> GetDistinctCategoriesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = "SELECT DISTINCT Category FROM dbo.Events WHERE Status = 'Upcoming' ORDER BY Category;";
        using var cmd = new SqlCommand(sql, connection);

        var list = new List<string>();
        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(reader.GetString(0));
        }

        return list;
    }

    public async Task<IReadOnlyList<string>> GetDistinctCitiesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT DISTINCT c.CityName 
            FROM dbo.Events e
            INNER JOIN dbo.Cities c ON e.CityId = c.CityId
            WHERE e.Status = 'Upcoming'
            ORDER BY c.CityName;";
        using var cmd = new SqlCommand(sql, connection);

        var list = new List<string>();
        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(reader.GetString(0));
        }

        return list;
    }

    public async Task<int> CreateAsync(EventItem item, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            INSERT INTO dbo.Events (Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
            VALUES (@Title, @Description, @Category, @CityId, @VenueId, @BannerUrl, @StartDate, @EndDate, @Status, SYSUTCDATETIME());
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Title", item.Title);
        cmd.Parameters.AddWithValue("@Description", item.Description);
        cmd.Parameters.AddWithValue("@Category", item.Category);
        cmd.Parameters.AddWithValue("@CityId", item.CityId);
        cmd.Parameters.AddWithValue("@VenueId", (object?)item.VenueId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BannerUrl", (object?)item.BannerUrl ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
        cmd.Parameters.AddWithValue("@EndDate", item.EndDate);
        cmd.Parameters.AddWithValue("@Status", item.Status);

        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(EventItem item, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            UPDATE dbo.Events
            SET Title = @Title,
                Description = @Description,
                Category = @Category,
                CityId = @CityId,
                VenueId = @VenueId,
                BannerUrl = @BannerUrl,
                StartDate = @StartDate,
                EndDate = @EndDate,
                Status = @Status,
                UpdatedAtUtc = SYSUTCDATETIME()
            WHERE EventId = @EventId;";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@EventId", item.EventId);
        cmd.Parameters.AddWithValue("@Title", item.Title);
        cmd.Parameters.AddWithValue("@Description", item.Description);
        cmd.Parameters.AddWithValue("@Category", item.Category);
        cmd.Parameters.AddWithValue("@CityId", item.CityId);
        cmd.Parameters.AddWithValue("@VenueId", (object?)item.VenueId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BannerUrl", (object?)item.BannerUrl ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
        cmd.Parameters.AddWithValue("@EndDate", item.EndDate);
        cmd.Parameters.AddWithValue("@Status", item.Status);

        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<int> AddTicketCategoryAsync(TicketCategory category, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            INSERT INTO dbo.TicketCategories (EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
            VALUES (@EventId, @SessionId, @CategoryName, @Price, @TotalCapacity, @RemainingCapacity, @MaxPerBooking, @Perks);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@EventId", category.EventId);
        cmd.Parameters.AddWithValue("@SessionId", (object?)category.SessionId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
        cmd.Parameters.AddWithValue("@Price", category.Price);
        cmd.Parameters.AddWithValue("@TotalCapacity", category.TotalCapacity);
        cmd.Parameters.AddWithValue("@RemainingCapacity", category.RemainingCapacity);
        cmd.Parameters.AddWithValue("@MaxPerBooking", category.MaxPerBooking);
        cmd.Parameters.AddWithValue("@Perks", (object?)category.Perks ?? DBNull.Value);

        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    private static EventItem MapEventFromReader(SqlDataReader reader)
    {
        return new EventItem
        {
            EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Description = reader.GetString(reader.GetOrdinal("Description")),
            Category = reader.GetString(reader.GetOrdinal("Category")),
            CityId = reader.GetInt32(reader.GetOrdinal("CityId")),
            VenueId = reader.IsDBNull(reader.GetOrdinal("VenueId")) ? null : reader.GetInt32(reader.GetOrdinal("VenueId")),
            BannerUrl = reader.IsDBNull(reader.GetOrdinal("BannerUrl")) ? null : reader.GetString(reader.GetOrdinal("BannerUrl")),
            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
            Status = reader.GetString(reader.GetOrdinal("Status")),
            CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
            UpdatedAtUtc = reader.IsDBNull(reader.GetOrdinal("UpdatedAtUtc")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc")),
            CityName = reader.GetString(reader.GetOrdinal("CityName")),
            VenueName = reader.GetString(reader.GetOrdinal("VenueName")),
            VenueAddress = reader.GetString(reader.GetOrdinal("VenueAddress")),
            StartingPrice = reader.GetDecimal(reader.GetOrdinal("StartingPrice")),
            TotalRemainingTickets = reader.GetInt32(reader.GetOrdinal("TotalRemainingTickets"))
        };
    }
}
