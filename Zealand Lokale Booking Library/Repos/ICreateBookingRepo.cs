using System;
using System.Threading.Tasks;

namespace Zealand_Lokale_Booking_Library.Repos
{
    public interface ICreateBookingRepo
    {
        /// <summary>
        /// Creates a booking by calling dbo.usp_CreateBooking.
        /// The stored procedure handles:
        /// - Department validation
        /// - Double-booking checks
        /// - Room ownership validation
        /// </summary>
        Task CreateBookingAsync(int userId, int roomId, DateTime date, TimeSpan startTime, int? smartBoardId);

    }
}
