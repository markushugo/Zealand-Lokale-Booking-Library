using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Repos;

namespace Zealand_Lokale_Booking_Library.Repos
{
    public class CreateBookingRepo: ICreateBookingRepo
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
        /// </summary>
        public async Task CreateBookingAsync(int userId, int roomId, DateTime date, TimeSpan startTime, int? smartBoardId)
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
        }
    }
}

//How to use this repo:
//using Zealand_Lokale_Booking_Library.Repos;

//internal class Program
//{
//    static async Task Main(string[] args)
//    {
//        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ZealandBooking;Integrated Security=True;Encrypt=False;TrustServerCertificate=False;";

//        var repo = new CreateBookingRepo(connectionString);

//        await repo.CreateBookingAsync
//        (
//            1,
//            1,
//            DateTime.Parse("2025-11-29"),
//            TimeSpan.Parse("12:00"),
//            null
//        );

//        Console.WriteLine("Booking created with ID: ");
//    }
//}