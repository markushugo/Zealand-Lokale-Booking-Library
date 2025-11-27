using System.Data;
using Microsoft.Data.SqlClient;

public interface ITestRepository
{
    Task InsertTestAsync(string value);
}

public class TestRepository : ITestRepository
{
    private readonly string _connectionString;

    public TestRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InsertTestAsync(string value)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand("dbo.InsertTest", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Value", value);

        await command.ExecuteNonQueryAsync();
    }
}

