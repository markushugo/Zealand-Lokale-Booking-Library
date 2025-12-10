using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Repos
{
    public interface IGetBookingsRepo
    {
        /// <summary>
        /// Retrieves all bookings belonging to the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose bookings should be retrieved.</param>
        /// <returns>
        /// A list of <see cref="Booking"/> objects representing the user's bookings.
        /// </returns>
        Task<List<Booking>> GetBookingsByUserIdAsync(int userId);
    }
}
