using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Repos
{
    /// <summary>
    /// Provides operations for managing bookings, including
    /// retrieving filtered bookings and deleting bookings.
    /// </summary>
    public interface IManageBookingRepo
    {
        /// <summary>
        /// Retrieves bookings that match the specified filter criteria
        /// by executing <c>dbo.usp_GetFilteredBookings</c>.
        /// </summary>
        /// <param name="filter">
        /// A <see cref="BookingFilter"/> object containing date, user ID,
        /// and filter lists (departments, buildings, rooms, etc.).
        /// </param>
        /// <returns>
        /// An enumerable collection of <see cref="Booking"/> objects
        /// representing the filtered bookings.
        /// </returns>
        IEnumerable<Booking> GetFilteredBookings(BookingFilter filter);

        /// <summary>
        /// Deletes a booking if the specified user is allowed to delete it,
        /// via the <c>dbo.usp_DeleteBooking</c> stored procedure.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to delete.</param>
        /// <param name="userId">The ID of the user performing the delete operation.</param>
        Task DeleteBookingAsync(int bookingId, int userId);
    }
}
