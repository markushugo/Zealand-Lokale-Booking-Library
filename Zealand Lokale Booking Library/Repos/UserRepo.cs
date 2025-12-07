using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Repos
{
    /// <summary>
    /// Provides methods for authenticating a user using
    /// the <c>dbo.usp_AuthenticateUser</c> stored procedure.
    /// </summary>
    public class UserRepo : IUserRepo
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of <see cref="UserRepo"/>.
        /// </summary>
        public UserRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Authenticates a user by executing the <c>dbo.usp_AuthenticateUser</c> stored procedure.
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="password">User password (plain text as stored)</param>
        /// <returns>
        /// A <see cref="User"/> instance if credentials match, otherwise <c>null</c>.
        /// </returns>
        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.usp_AuthenticateUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Parameters
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar, 100)
            {
                Value = email
            });

            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar, 255)
            {
                Value = password
            });

            await connection.OpenAsync().ConfigureAwait(false);

            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            if (await reader.ReadAsync().ConfigureAwait(false))
            {
                return MapUser(reader);
            }

            return null;
        }

        /// <summary>
        /// Maps the current SqlDataReader row to a <see cref="User"/>.
        /// </summary>
        private static User MapUser(SqlDataReader reader)
        {
            int idOrdinal = reader.GetOrdinal("UserID");
            int nameOrdinal = reader.GetOrdinal("Name");
            int emailOrdinal = reader.GetOrdinal("Email");
            int typeOrdinal = reader.GetOrdinal("UserTypeID");

            return new User
            {
                UserID = reader.GetInt32(idOrdinal),
                Name = reader.GetString(nameOrdinal),
                Email = reader.GetString(emailOrdinal),
                UserTypeID = reader.GetInt32(typeOrdinal)
            };
        }
    }
}
