namespace TicketBooking.Web.Data.Repositories;

using System.Data;
using Microsoft.Data.SqlClient;
using TicketBooking.Web.Data;
using TicketBooking.Web.Models;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetRolesByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> AssignRoleAsync(int userId, string roleName, CancellationToken cancellationToken = default);
}

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT UserId, FullName, Email, PasswordHash, PhoneNumber, PreferredCityId, IsActive, CreatedAtUtc, UpdatedAtUtc
            FROM dbo.Users
            WHERE UserId = @UserId;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UserId", userId);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var user = MapUser(reader);
            reader.Close();
            user.Roles = (await GetRolesByUserIdInternalAsync(connection, user.UserId, cancellationToken)).ToList();
            return user;
        }

        return null;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT UserId, FullName, Email, PasswordHash, PhoneNumber, PreferredCityId, IsActive, CreatedAtUtc, UpdatedAtUtc
            FROM dbo.Users
            WHERE Email = @Email;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", email.Trim().ToLowerInvariant());

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var user = MapUser(reader);
            reader.Close();
            user.Roles = (await GetRolesByUserIdInternalAsync(connection, user.UserId, cancellationToken)).ToList();
            return user;
        }

        return null;
    }

    public async Task<int> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            INSERT INTO dbo.Users (FullName, Email, PasswordHash, PhoneNumber, PreferredCityId, IsActive, CreatedAtUtc)
            VALUES (@FullName, @Email, @PasswordHash, @PhoneNumber, @PreferredCityId, @IsActive, SYSUTCDATETIME());
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FullName", user.FullName);
        command.Parameters.AddWithValue("@Email", user.Email.Trim().ToLowerInvariant());
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@PhoneNumber", (object?)user.PhoneNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@PreferredCityId", (object?)user.PreferredCityId ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsActive", user.IsActive);

        var newId = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        return newId;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            UPDATE dbo.Users
            SET FullName = @FullName,
                PhoneNumber = @PhoneNumber,
                PreferredCityId = @PreferredCityId,
                UpdatedAtUtc = SYSUTCDATETIME()
            WHERE UserId = @UserId;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UserId", user.UserId);
        command.Parameters.AddWithValue("@FullName", user.FullName);
        command.Parameters.AddWithValue("@PhoneNumber", (object?)user.PhoneNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@PreferredCityId", (object?)user.PreferredCityId ?? DBNull.Value);

        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected > 0;
    }

    public async Task<IReadOnlyList<string>> GetRolesByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        return await GetRolesByUserIdInternalAsync(connection, userId, cancellationToken);
    }

    public async Task<bool> AssignRoleAsync(int userId, string roleName, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = @"
            IF NOT EXISTS (
                SELECT 1 FROM dbo.UserRoles ur
                JOIN dbo.Roles r ON ur.RoleId = r.RoleId
                WHERE ur.UserId = @UserId AND r.RoleName = @RoleName
            )
            BEGIN
                INSERT INTO dbo.UserRoles (UserId, RoleId)
                SELECT @UserId, RoleId FROM dbo.Roles WHERE RoleName = @RoleName;
            END";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@RoleName", roleName);

        var rows = await command.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    private static async Task<IReadOnlyList<string>> GetRolesByUserIdInternalAsync(
        SqlConnection connection, int userId, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT r.RoleName
            FROM dbo.Roles r
            INNER JOIN dbo.UserRoles ur ON r.RoleId = ur.RoleId
            WHERE ur.UserId = @UserId;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UserId", userId);

        var roles = new List<string>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            roles.Add(reader.GetString(0));
        }

        return roles;
    }

    private static User MapUser(SqlDataReader reader)
    {
        return new User
        {
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            FullName = reader.GetString(reader.GetOrdinal("FullName")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
            PreferredCityId = reader.IsDBNull(reader.GetOrdinal("PreferredCityId")) ? null : reader.GetInt32(reader.GetOrdinal("PreferredCityId")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
            UpdatedAtUtc = reader.IsDBNull(reader.GetOrdinal("UpdatedAtUtc")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAtUtc"))
        };
    }
}
