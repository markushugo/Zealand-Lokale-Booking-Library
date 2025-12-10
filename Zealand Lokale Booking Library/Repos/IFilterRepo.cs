using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Repos
{
    /// <summary>
    /// Provides access to filter-related data and available booking slots,
    /// including fetching filter options for a specific user and retrieving
    /// free room booking times based on user-defined criteria.
    /// </summary>
    public interface IFilterRepo
    {
        /// <summary>
        /// Retrieves all filter options accessible to the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user requesting filter options.</param>
        /// <returns>
        /// A <see cref="FilterOptions"/> object containing all available filter values.
        /// </returns>
        Task<FilterOptions> GetFilterOptionsForUserAsync(int userId);

        /// <summary>
        /// Retrieves all available (unbooked) booking slots based on the specified filter criteria.
        /// </summary>
        /// <param name="filter">
        /// A <see cref="BookingFilter"/> object containing date, user ID, and filter lists.
        /// </param>
        /// <returns>
        /// An enumerable collection of <see cref="Booking"/> objects representing free booking slots.
        /// </returns>
        IEnumerable<Booking> GetAvailableBookingSlots(BookingFilter filter);
    }
}
