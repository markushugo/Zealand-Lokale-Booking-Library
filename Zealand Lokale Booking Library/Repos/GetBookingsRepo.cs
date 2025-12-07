using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Zealand_Lokale_Booking_Library.Models;
using Dapper;

namespace Zealand_Lokale_Booking_Library.Repos
{
    /// <summary>
    /// Repository for retrieving booking information for a specific user.
    /// This repository uses Dapper to execute the stored procedure 
    /// <c>usp_GetBookingsByUserID</c> and maps the result to <see cref="Booking"/> objects.
    /// </summary>
    public class GetBookingsRepo
    {
        private readonly string _connectionString;
        /// <summary>
        /// Initializes a new instance of the <see cref="GetBookingsRepo"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public GetBookingsRepo(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// Retrieves all bookings belonging to the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose bookings should be retrieved.</param>
        /// <returns>
        /// A list of <see cref="Booking"/> objects representing the user's bookings.
        /// </returns>
        /// <remarks>
        /// This method calls the stored procedure <c>usp_GetBookingsByUserID</c>.
        /// </remarks>
        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserID", userId);

                var results = await connection.QueryAsync<Booking>(
                    "usp_GetBookingsByUserID",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return results.AsList();
            }
        }


    }
}
