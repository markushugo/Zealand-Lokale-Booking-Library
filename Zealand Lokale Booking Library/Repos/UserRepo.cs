using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Zealand_Lokale_Booking_Library.Repos
{
    /// <summary>
    /// Provides methods for authenticating a user using dbo.usp_LoginUser.
    /// </summary>
    public class UserRepo
    {
        private readonly string _connectionString;

        public UserRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Calls dbo.usp_LoginUser and returns success + SessionID GUID.
        /// Returns (false, null) if login fails.
        /// </summary>
        public async Task<(bool Success, Guid? SessionId)> LoginUserAsync(string email, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.usp_LoginUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 255) { Value = email });
            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 255) { Value = password });

            await connection.OpenAsync().ConfigureAwait(false);

            try
            {
                using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                if (await reader.ReadAsync().ConfigureAwait(false))
                {
                    // SUCCESS CASE – SP returns a single GUID column
                    if (reader.FieldCount == 1)
                    {
                        string guidString = reader.GetString(0);

                        if (Guid.TryParse(guidString, out Guid sessionId))
                        {
                            return (true, sessionId);
                        }
                    }

                    // FAILURE CASE – SP returns (IsAuthenticated=0, UserID=null, etc.)
                    if (reader.FieldCount > 1)
                    {
                        return (false, null);
                    }
                }

                return (false, null);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Login error: " + ex.Message);
                return (false, null);
            }
        }
    }
}
