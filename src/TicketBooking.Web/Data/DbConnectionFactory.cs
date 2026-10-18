namespace TicketBooking.Web.Data;

using System.Data;
using Microsoft.Data.SqlClient;

public interface IDbConnectionFactory
{
    Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    SqlConnection CreateConnection();
}

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=ShowSphereDb;Trusted_Connection=True;TrustServerCertificate=True;";
    }

    public async Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    public SqlConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
