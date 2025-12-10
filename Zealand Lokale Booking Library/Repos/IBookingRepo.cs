using System.Collections.Generic;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Repos
{
    public interface IBookingRepo
    {
        /// <summary>
        /// Defines methods for retrieving bookings from a SQL Server database
        /// using filters such as date, departments, buildings, rooms, etc.
        /// </summary>
        public interface IBookingRepo
        {
            /// <summary>
            /// Retrieves bookings for a given date and optional filter values
            /// by executing the <c>dbo.usp_GetFilteredBookings</c> stored procedure.
            /// </summary>
            /// <param name="filter">
            /// An instance of <see cref="BookingFilter"/> that contains the user,
            /// date and optional filter lists (departments, buildings, rooms, etc.).
            /// </param>
            /// <returns>
            /// A task that represents the asynchronous operation.
            /// The task result contains a read-only list of <see cref="Booking"/> instances
            /// that match the supplied filter.
            /// </returns>
            Task<IReadOnlyList<Booking>> GetFilteredBookingsAsync(BookingFilter filter);
        }

    }
}
