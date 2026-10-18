namespace TicketBooking.Web.Data;

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using TicketBooking.Web.Models;

public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly IWebHostEnvironment _env;

    public DatabaseInitializer(
        IDbConnectionFactory connectionFactory,
        IConfiguration configuration,
        ILogger<DatabaseInitializer> logger,
        IWebHostEnvironment env)
    {
        _connectionFactory = connectionFactory;
        _configuration = configuration;
        _logger = logger;
        _env = env;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting database initialization check...");

            var rootPath = _env.ContentRootPath;
            var databaseDir = Path.Combine(rootPath, "..", "..", "database");
            if (!Directory.Exists(databaseDir))
            {
                databaseDir = Path.Combine(rootPath, "database");
            }

            using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

            // 1. Verify if tables already exist
            bool tablesExist;
            using (var checkCmd = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo';", connection))
            {
                var count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync(cancellationToken));
                tablesExist = count > 10;
            }

            if (!tablesExist && Directory.Exists(databaseDir))
            {
                _logger.LogInformation("Database tables missing. Applying SQL scripts from {Dir}...", databaseDir);
                await ExecuteSqlFileAsync(connection, Path.Combine(databaseDir, "01_schema.sql"), cancellationToken);
                await ExecuteSqlFileAsync(connection, Path.Combine(databaseDir, "02_indexes.sql"), cancellationToken);
                await ExecuteSqlFileAsync(connection, Path.Combine(databaseDir, "03_seed_reference.sql"), cancellationToken);
                await ExecuteSqlFileAsync(connection, Path.Combine(databaseDir, "04_seed_demo.sql"), cancellationToken);
                await ExecuteSqlFileAsync(connection, Path.Combine(databaseDir, "05_seed_events.sql"), cancellationToken);
            }

            // 2. Ensure Admin, Customer, and Abhay demo users exist
            await EnsureAdminUserAsync(connection, cancellationToken);
            await EnsureDemoCustomerUserAsync(connection, cancellationToken);
            await EnsureAbhayUserAsync(connection, cancellationToken);

            _logger.LogInformation("Database initialization check completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed: {Message}", ex.Message);
            throw;
        }
    }

    private async Task EnsureAdminUserAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        var adminEmail = _configuration["AdminSeed:Email"] ?? "admin@showsphere.local";
        var adminPassword = _configuration["AdminSeed:Password"] ?? "Admin@ShowSphere2026!";
        var adminFullName = _configuration["AdminSeed:FullName"] ?? "System Administrator";

        const string checkSql = "SELECT UserId FROM dbo.Users WHERE Email = @Email;";
        int? adminUserId = null;

        using (var cmd = new SqlCommand(checkSql, connection))
        {
            cmd.Parameters.AddWithValue("@Email", adminEmail);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            if (result != null && result != DBNull.Value)
            {
                adminUserId = Convert.ToInt32(result);
            }
        }

        if (!adminUserId.HasValue)
        {
            _logger.LogInformation("Seeding Admin account for {Email} using PasswordHasher...", adminEmail);

            var hasher = new PasswordHasher<User>();
            var dummyUser = new User { Email = adminEmail, FullName = adminFullName };
            var hashedPassword = hasher.HashPassword(dummyUser, adminPassword);

            const string insertUserSql = @"
                INSERT INTO dbo.Users (FullName, Email, PasswordHash, PhoneNumber, PreferredCityId, IsActive, CreatedAtUtc)
                VALUES (@FullName, @Email, @PasswordHash, @Phone, 1, 1, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";

            using (var cmd = new SqlCommand(insertUserSql, connection))
            {
                cmd.Parameters.AddWithValue("@FullName", adminFullName);
                cmd.Parameters.AddWithValue("@Email", adminEmail);
                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                cmd.Parameters.AddWithValue("@Phone", "+919876543210");

                adminUserId = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken));
            }
        }

        // Ensure Admin role is mapped
        const string checkRoleSql = @"
            SELECT COUNT(*) FROM dbo.UserRoles ur
            JOIN dbo.Roles r ON ur.RoleId = r.RoleId
            WHERE ur.UserId = @UserId AND r.RoleName = 'Admin';";

        using (var cmd = new SqlCommand(checkRoleSql, connection))
        {
            cmd.Parameters.AddWithValue("@UserId", adminUserId.Value);
            var roleCount = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken));

            if (roleCount == 0)
            {
                const string assignRoleSql = @"
                    INSERT INTO dbo.UserRoles (UserId, RoleId)
                    SELECT @UserId, RoleId FROM dbo.Roles WHERE RoleName = 'Admin';";

                using var assignCmd = new SqlCommand(assignRoleSql, connection);
                assignCmd.Parameters.AddWithValue("@UserId", adminUserId.Value);
                await assignCmd.ExecuteNonQueryAsync(cancellationToken);
                _logger.LogInformation("Assigned Admin role to user ID {UserId}.", adminUserId.Value);
            }
        }
    }

    private async Task EnsureDemoCustomerUserAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        const string customerEmail = "customer@showsphere.local";
        const string customerPassword = "Customer@ShowSphere2026!";
        const string customerFullName = "Demo Customer";

        const string checkSql = "SELECT UserId FROM dbo.Users WHERE Email = @Email;";
        int? customerUserId = null;

        using (var cmd = new SqlCommand(checkSql, connection))
        {
            cmd.Parameters.AddWithValue("@Email", customerEmail);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            if (result != null && result != DBNull.Value)
            {
                customerUserId = Convert.ToInt32(result);
            }
        }

        if (!customerUserId.HasValue)
        {
            _logger.LogInformation("Seeding Demo Customer account for {Email} using PasswordHasher...", customerEmail);

            var hasher = new PasswordHasher<User>();
            var dummyUser = new User { Email = customerEmail, FullName = customerFullName };
            var hashedPassword = hasher.HashPassword(dummyUser, customerPassword);

            const string insertUserSql = @"
                INSERT INTO dbo.Users (FullName, Email, PasswordHash, PhoneNumber, PreferredCityId, IsActive, CreatedAtUtc)
                VALUES (@FullName, @Email, @PasswordHash, @Phone, 1, 1, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";

            using (var cmd = new SqlCommand(insertUserSql, connection))
            {
                cmd.Parameters.AddWithValue("@FullName", customerFullName);
                cmd.Parameters.AddWithValue("@Email", customerEmail);
                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                cmd.Parameters.AddWithValue("@Phone", "+919876500001");

                customerUserId = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken));
            }
        }

        // Ensure Customer role is mapped
        const string checkRoleSql = @"
            SELECT COUNT(*) FROM dbo.UserRoles ur
            JOIN dbo.Roles r ON ur.RoleId = r.RoleId
            WHERE ur.UserId = @UserId AND r.RoleName = 'Customer';";

        using (var cmd = new SqlCommand(checkRoleSql, connection))
        {
            cmd.Parameters.AddWithValue("@UserId", customerUserId.Value);
            var roleCount = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken));

            if (roleCount == 0)
            {
                const string assignRoleSql = @"
                    INSERT INTO dbo.UserRoles (UserId, RoleId)
                    SELECT @UserId, RoleId FROM dbo.Roles WHERE RoleName = 'Customer';";

                using var assignCmd = new SqlCommand(assignRoleSql, connection);
                assignCmd.Parameters.AddWithValue("@UserId", customerUserId.Value);
                await assignCmd.ExecuteNonQueryAsync(cancellationToken);
                _logger.LogInformation("Assigned Customer role to user ID {UserId}.", customerUserId.Value);
            }
        }
    }

    private async Task EnsureAbhayUserAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        var targetEmails = new[] { "abhay@gmail.com", "abhay@gmai.com" };
        const string abhayPassword = "Abhay@2026";
        const string abhayFullName = "Abhay";

        var hasher = new PasswordHasher<User>();

        foreach (var email in targetEmails)
        {
            var dummyUser = new User { Email = email, FullName = abhayFullName };
            var hashedPassword = hasher.HashPassword(dummyUser, abhayPassword);

            const string checkSql = "SELECT UserId FROM dbo.Users WHERE Email = @Email;";
            int? abhayUserId = null;

            using (var cmd = new SqlCommand(checkSql, connection))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                var result = await cmd.ExecuteScalarAsync(cancellationToken);
                if (result != null && result != DBNull.Value)
                {
                    abhayUserId = Convert.ToInt32(result);
                }
            }

            if (!abhayUserId.HasValue)
            {
                _logger.LogInformation("Seeding Abhay account for {Email}...", email);

                const string insertUserSql = @"
                    INSERT INTO dbo.Users (FullName, Email, PasswordHash, PhoneNumber, PreferredCityId, IsActive, CreatedAtUtc)
                    VALUES (@FullName, @Email, @PasswordHash, @Phone, 1, 1, SYSUTCDATETIME());
                    SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(insertUserSql, connection))
                {
                    cmd.Parameters.AddWithValue("@FullName", abhayFullName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    cmd.Parameters.AddWithValue("@Phone", "+919876543210");

                    abhayUserId = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken));
                }
            }
            else
            {
                // Update password hash to guarantee abhay@123 works
                const string updateSql = "UPDATE dbo.Users SET PasswordHash = @PasswordHash, IsActive = 1 WHERE UserId = @UserId;";
                using var updateCmd = new SqlCommand(updateSql, connection);
                updateCmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                updateCmd.Parameters.AddWithValue("@UserId", abhayUserId.Value);
                await updateCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            // Ensure both Customer and Admin roles are mapped for full access
            const string assignRolesSql = @"
                IF NOT EXISTS (SELECT 1 FROM dbo.UserRoles ur JOIN dbo.Roles r ON ur.RoleId = r.RoleId WHERE ur.UserId = @UserId AND r.RoleName = 'Customer')
                BEGIN
                    INSERT INTO dbo.UserRoles (UserId, RoleId) SELECT @UserId, RoleId FROM dbo.Roles WHERE RoleName = 'Customer';
                END
                IF NOT EXISTS (SELECT 1 FROM dbo.UserRoles ur JOIN dbo.Roles r ON ur.RoleId = r.RoleId WHERE ur.UserId = @UserId AND r.RoleName = 'Admin')
                BEGIN
                    INSERT INTO dbo.UserRoles (UserId, RoleId) SELECT @UserId, RoleId FROM dbo.Roles WHERE RoleName = 'Admin';
                END";

            using var roleCmd = new SqlCommand(assignRolesSql, connection);
            roleCmd.Parameters.AddWithValue("@UserId", abhayUserId.Value);
            await roleCmd.ExecuteNonQueryAsync(cancellationToken);
            _logger.LogInformation("Ensured roles for user {Email}.", email);
        }
    }

    private static async Task ExecuteSqlFileAsync(SqlConnection connection, string filePath, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath)) return;

        var sql = await File.ReadAllTextAsync(filePath, cancellationToken);
        // Split by GO on its own line
        var batches = Regex.Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        foreach (var batch in batches)
        {
            var trimmed = batch.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;

            using var cmd = new SqlCommand(trimmed, connection);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
