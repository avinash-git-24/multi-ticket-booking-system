namespace TicketBooking.Web.Data.Repositories;

using System.Data;
using Microsoft.Data.SqlClient;
using TicketBooking.Web.Data;
using TicketBooking.Web.Models;

public interface IMovieRepository
{
    Task<IReadOnlyList<Movie>> GetMoviesAsync(string? city, string? status, string? genre, string? language, string? search, CancellationToken cancellationToken = default);
    Task<Movie?> GetByIdAsync(int movieId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Movie movie, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int movieId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetDistinctGenresAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetDistinctLanguagesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<City>> GetAllCitiesAsync(CancellationToken cancellationToken = default);
}

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MovieRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Movie>> GetMoviesAsync(
        string? city, string? status, string? genre, string? language, string? search, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var sql = @"
            SELECT DISTINCT m.MovieId, m.Title, m.Description, m.DurationMinutes, m.Language, m.Genre, 
                   m.ReleaseDate, m.Certificate, m.PosterUrl, m.BannerUrl, m.TrailerUrl, m.Status, m.CreatedAtUtc
            FROM dbo.Movies m ";

        if (!string.IsNullOrWhiteSpace(city))
        {
            sql += @"
                LEFT JOIN dbo.Showtimes s ON m.MovieId = s.MovieId AND s.IsActive = 1
                LEFT JOIN dbo.Screens sc ON s.ScreenId = sc.ScreenId
                LEFT JOIN dbo.Theatres t ON sc.TheatreId = t.TheatreId
                LEFT JOIN dbo.Cities c ON t.CityId = c.CityId ";
        }

        sql += " WHERE 1=1 ";

        if (!string.IsNullOrWhiteSpace(city))
        {
            sql += " AND (c.CityName = @City OR m.Status = 'Upcoming') ";
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            sql += " AND m.Status = @Status ";
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            sql += " AND m.Genre LIKE '%' + @Genre + '%' ";
        }

        if (!string.IsNullOrWhiteSpace(language))
        {
            sql += " AND m.Language LIKE '%' + @Language + '%' ";
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            sql += " AND (m.Title LIKE '%' + @Search + '%' OR m.Description LIKE '%' + @Search + '%') ";
        }

        sql += " ORDER BY m.ReleaseDate DESC;";

        using var command = new SqlCommand(sql, connection);

        if (!string.IsNullOrWhiteSpace(city))
            command.Parameters.AddWithValue("@City", city);

        if (!string.IsNullOrWhiteSpace(status))
            command.Parameters.AddWithValue("@Status", status);

        if (!string.IsNullOrWhiteSpace(genre))
            command.Parameters.AddWithValue("@Genre", genre);

        if (!string.IsNullOrWhiteSpace(language))
            command.Parameters.AddWithValue("@Language", language);

        if (!string.IsNullOrWhiteSpace(search))
            command.Parameters.AddWithValue("@Search", search);

        var list = new List<Movie>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(MapMovie(reader));
        }

        return list;
    }

    public async Task<Movie?> GetByIdAsync(int movieId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT MovieId, Title, Description, DurationMinutes, Language, Genre, 
                   ReleaseDate, Certificate, PosterUrl, BannerUrl, TrailerUrl, Status, CreatedAtUtc
            FROM dbo.Movies
            WHERE MovieId = @MovieId;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@MovieId", movieId);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return MapMovie(reader);
        }

        return null;
    }

    public async Task<int> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            INSERT INTO dbo.Movies (Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, TrailerUrl, Status, CreatedAtUtc)
            VALUES (@Title, @Description, @DurationMinutes, @Language, @Genre, @ReleaseDate, @Certificate, @PosterUrl, @BannerUrl, @TrailerUrl, @Status, SYSUTCDATETIME());
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Title", movie.Title);
        command.Parameters.AddWithValue("@Description", movie.Description);
        command.Parameters.AddWithValue("@DurationMinutes", movie.DurationMinutes);
        command.Parameters.AddWithValue("@Language", movie.Language);
        command.Parameters.AddWithValue("@Genre", movie.Genre);
        command.Parameters.AddWithValue("@ReleaseDate", movie.ReleaseDate);
        command.Parameters.AddWithValue("@Certificate", movie.Certificate);
        command.Parameters.AddWithValue("@PosterUrl", (object?)movie.PosterUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@BannerUrl", (object?)movie.BannerUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@TrailerUrl", (object?)movie.TrailerUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@Status", movie.Status);

        var newId = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        return newId;
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            UPDATE dbo.Movies
            SET Title = @Title,
                Description = @Description,
                DurationMinutes = @DurationMinutes,
                Language = @Language,
                Genre = @Genre,
                ReleaseDate = @ReleaseDate,
                Certificate = @Certificate,
                PosterUrl = @PosterUrl,
                BannerUrl = @BannerUrl,
                TrailerUrl = @TrailerUrl,
                Status = @Status,
                UpdatedAtUtc = SYSUTCDATETIME()
            WHERE MovieId = @MovieId;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@MovieId", movie.MovieId);
        command.Parameters.AddWithValue("@Title", movie.Title);
        command.Parameters.AddWithValue("@Description", movie.Description);
        command.Parameters.AddWithValue("@DurationMinutes", movie.DurationMinutes);
        command.Parameters.AddWithValue("@Language", movie.Language);
        command.Parameters.AddWithValue("@Genre", movie.Genre);
        command.Parameters.AddWithValue("@ReleaseDate", movie.ReleaseDate);
        command.Parameters.AddWithValue("@Certificate", movie.Certificate);
        command.Parameters.AddWithValue("@PosterUrl", (object?)movie.PosterUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@BannerUrl", (object?)movie.BannerUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@TrailerUrl", (object?)movie.TrailerUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@Status", movie.Status);

        var rows = await command.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int movieId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            UPDATE dbo.Movies
            SET Status = 'Archived', UpdatedAtUtc = SYSUTCDATETIME()
            WHERE MovieId = @MovieId;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@MovieId", movieId);

        var rows = await command.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<IReadOnlyList<string>> GetDistinctGenresAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = "SELECT DISTINCT Genre FROM dbo.Movies WHERE Status != 'Archived';";

        using var command = new SqlCommand(sql, connection);
        var genres = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var raw = reader.GetString(0);
            foreach (var item in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                genres.Add(item);
            }
        }

        return genres.OrderBy(g => g).ToList();
    }

    public async Task<IReadOnlyList<string>> GetDistinctLanguagesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = "SELECT DISTINCT Language FROM dbo.Movies WHERE Status != 'Archived';";

        using var command = new SqlCommand(sql, connection);
        var languages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var raw = reader.GetString(0);
            foreach (var item in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                languages.Add(item);
            }
        }

        return languages.OrderBy(l => l).ToList();
    }

    public async Task<IReadOnlyList<City>> GetAllCitiesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = "SELECT CityId, CityName, StateName, IsActive FROM dbo.Cities WHERE IsActive = 1 ORDER BY CityName;";

        using var command = new SqlCommand(sql, connection);
        var cities = new List<City>();

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            cities.Add(new City
            {
                CityId = reader.GetInt32(0),
                CityName = reader.GetString(1),
                StateName = reader.GetString(2),
                IsActive = reader.GetBoolean(3)
            });
        }

        return cities;
    }

    private static Movie MapMovie(SqlDataReader reader)
    {
        return new Movie
        {
            MovieId = reader.GetInt32(reader.GetOrdinal("MovieId")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Description = reader.GetString(reader.GetOrdinal("Description")),
            DurationMinutes = reader.GetInt32(reader.GetOrdinal("DurationMinutes")),
            Language = reader.GetString(reader.GetOrdinal("Language")),
            Genre = reader.GetString(reader.GetOrdinal("Genre")),
            ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
            Certificate = reader.GetString(reader.GetOrdinal("Certificate")),
            PosterUrl = reader.IsDBNull(reader.GetOrdinal("PosterUrl")) ? null : reader.GetString(reader.GetOrdinal("PosterUrl")),
            BannerUrl = reader.IsDBNull(reader.GetOrdinal("BannerUrl")) ? null : reader.GetString(reader.GetOrdinal("BannerUrl")),
            TrailerUrl = reader.IsDBNull(reader.GetOrdinal("TrailerUrl")) ? null : reader.GetString(reader.GetOrdinal("TrailerUrl")),
            Status = reader.GetString(reader.GetOrdinal("Status")),
            CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc"))
        };
    }
}
