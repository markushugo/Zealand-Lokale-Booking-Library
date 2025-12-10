using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Services
{
    /// <summary>
    /// Defines high-level booking operations including creation, retrieval,
    /// filtering, availability checks, and deletion. This interface represents
    /// the public contract for the <see cref="BookingService"/>.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Creates a new booking using the provided booking details.
        /// </summary>
        /// <param name="userId">The ID of the user creating the booking.</param>
        /// <param name="roomId">The ID of the room being booked.</param>
        /// <param name="date">The date on which the booking takes place.</param>
        /// <param name="startTime">The starting time of the booking.</param>
        /// <param name="smartBoardId">Optional SmartBoard ID associated with the booking.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CreateBookingAsync(int userId, int roomId, DateTime date, TimeSpan startTime, int? smartBoardId);

        /// <summary>
        /// Retrieves all bookings belonging to the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose bookings are requested.</param>
        /// <returns>A task containing the list of bookings associated with the user.</returns>
        Task<List<Booking>> GetBookingsByUserIdAsync(int userId);

        /// <summary>
        /// Retrieves all bookings that match the provided filter criteria,
        /// using asynchronous execution suitable for UI loading scenarios.
        /// </summary>
        /// <param name="filter">
        /// The <see cref="BookingFilter"/> object containing filtering parameters such as
        /// departments, buildings, rooms, room types, levels, times, and date.
        /// </param>
        /// <returns>A task containing the filtered list of bookings.</returns>
        Task<IReadOnlyList<Booking>> GetFilteredBookingsAsync(BookingFilter filter);

        /// <summary>
        /// Retrieves all bookings matching the provided filter criteria using
        /// synchronous execution. Typically used for administrative or management scenarios.
        /// </summary>
        /// <param name="filter">The filtering parameters.</param>
        /// <returns>A collection of filtered bookings.</returns>
        IEnumerable<Booking> GetFilteredBookings(BookingFilter filter);

        /// <summary>
        /// Retrieves all filter options available to the specified user.
        /// This includes buildings, departments, room types, level options,
        /// and standard timeslot options.
        /// </summary>
        /// <param name="userId">The ID of the user requesting filter data.</param>
        /// <returns>A task containing the available filter options.</returns>
        Task<FilterOptions> GetFilterOptionsForUserAsync(int userId);

        /// <summary>
        /// Retrieves all available (unbooked) room and timeslot combinations
        /// that match the provided filtering criteria.
        /// </summary>
        /// <param name="filter">The specified filtering criteria.</param>
        /// <returns>
        /// A collection of <see cref="Booking"/> objects representing free booking slots.
        /// </returns>
        IEnumerable<Booking> GetAvailableBookingSlots(BookingFilter filter);

        /// <summary>
        /// Deletes a booking with the specified ID, provided the user has
        /// permission to perform the deletion.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to delete.</param>
        /// <param name="userId">The ID of the user performing the delete action.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        Task DeleteBookingAsync(int bookingId, int userId);
    }
}
