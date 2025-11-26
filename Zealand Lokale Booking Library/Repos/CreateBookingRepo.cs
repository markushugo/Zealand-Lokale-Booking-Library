using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Zealand_Lokale_Booking_Library.Repos
{
    public class CreateBookingRepo:ICreateBookingRepo
    {
        private readonly string _connectionString;

        public CreateBookingRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates a booking by calling dbo.usp_CreateBooking.
        /// The stored procedure performs:
        /// - Department validation
        /// - Double-booking validation
        /// - Room ownership validation
        /// - Returns the new BookingID
        /// </summary>
        public async Task<int> CreateBookingAsync(int userId, int roomId, DateTime date, TimeSpan startTime, int? smartBoardId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.usp_CreateBooking", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Required parameters
            command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userId });
            command.Parameters.Add(new SqlParameter("@RoomID", SqlDbType.Int) { Value = roomId });
            command.Parameters.Add(new SqlParameter("@Date", SqlDbType.Date) { Value = date.Date });
            command.Parameters.Add(new SqlParameter("@StartTime", SqlDbType.Time) { Value = startTime });

            // Optional SmartBoardID
            command.Parameters.Add(new SqlParameter("@SmartBoardID", SqlDbType.Int)
            {
                Value = smartBoardId.HasValue ? smartBoardId.Value : DBNull.Value
            });

            // Output parameter
            var outputParam = new SqlParameter("@NewBookingID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputParam);

            await connection.OpenAsync().ConfigureAwait(false);

            try
            {
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                // Stored procedure throws errors using RAISERROR or THROW
                throw new InvalidOperationException($"Booking could not be created: {ex.Message}", ex);
            }

            return (int)outputParam.Value;
        }
    }
}

