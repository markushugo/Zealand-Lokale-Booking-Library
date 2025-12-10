using Zealand_Lokale_Booking_Library.Models;
using Zealand_Lokale_Booking_Library.Repos;

namespace Zealand_Lokale_Booking_Library.Services
{
    /// <summary>
    /// Provides a high-level service that coordinates all booking-related
    /// operations including creation, filtering, retrieving user bookings,
    /// filter options, available timeslots, and deletion.
    /// </summary>
    /// <remarks>
    /// This service acts as a facade and delegates operations to the underlying
    /// repository classes. Repositories are injected through dependency injection.
    /// </remarks>
    public class BookingService : IBookingService
    {
        private readonly ICreateBookingRepo _createBookingRepo;
        private readonly IGetBookingsRepo _getBookingsRepo;
        private readonly IBookingRepo _bookingRepo;
        private readonly IFilterRepo _filterRepo;
        private readonly IManageBookingRepo _manageBookingRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class.
        /// </summary>
        /// <param name="createBookingRepo">Repository for creating bookings.</param>
        /// <param name="getBookingsRepo">Repository for retrieving user bookings.</param>
        /// <param name="bookingRepo">Repository for filtered bookings.</param>
        /// <param name="filterRepo">Repository for filter options and available slots.</param>
        /// <param name="manageBookingRepo">Repository for deleting and managing bookings.</param>
        public BookingService(
            ICreateBookingRepo createBookingRepo,
            IGetBookingsRepo getBookingsRepo,
            IBookingRepo bookingRepo,
            IFilterRepo filterRepo,
            IManageBookingRepo manageBookingRepo)
        {
            _createBookingRepo = createBookingRepo;
            _getBookingsRepo = getBookingsRepo;
            _bookingRepo = bookingRepo;
            _filterRepo = filterRepo;
            _manageBookingRepo = manageBookingRepo;
        }

        /// <summary>
        /// Creates a new booking using the underlying repository.
        /// </summary>
        /// <param name="userId">The user creating the booking.</param>
        /// <param name="roomId">The room to be booked.</param>
        /// <param name="date">The date of the booking.</param>
        /// <param name="startTime">The starting time for the booking.</param>
        /// <param name="smartBoardId">Optional smartboard ID.</param>
        public async Task CreateBookingAsync(int userId, int roomId, DateTime date, TimeSpan startTime, int? smartBoardId)
        {
            await _createBookingRepo.CreateBookingAsync(userId, roomId, date, startTime, smartBoardId);
        }

        /// <summary>
        /// Retrieves all bookings associated with the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose bookings are requested.</param>
        /// <returns>A list of bookings belonging to the user.</returns>
        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _getBookingsRepo.GetBookingsByUserIdAsync(userId);
        }

        /// <summary>
        /// Retrieves bookings matching the provided filter criteria using an async repository call.
        /// </summary>
        /// <param name="filter">The filter containing departments, rooms, types, timestamps, and date.</param>
        /// <returns>A readonly list of bookings matching the filter.</returns>
        public async Task<IReadOnlyList<Booking>> GetFilteredBookingsAsync(BookingFilter filter)
        {
            return await _bookingRepo.GetFilteredBookingsAsync(filter);
        }

        /// <summary>
        /// Retrieves filtered bookings using a synchronous repository,
        /// typically used for admin/management pages.
        /// </summary>
        /// <param name="filter">The filter settings for the booking query.</param>
        /// <returns>A collection of filtered bookings.</returns>
        public IEnumerable<Booking> GetFilteredBookings(BookingFilter filter)
        {
            return _manageBookingRepo.GetFilteredBookings(filter);
        }

        /// <summary>
        /// Retrieves all filter options available to a given user, including buildings,
        /// departments, room types, time slots, and level options.
        /// </summary>
        /// <param name="userId">The ID of the user requesting filter options.</param>
        /// <returns>A populated <see cref="FilterOptions"/> instance.</returns>
        public async Task<FilterOptions> GetFilterOptionsForUserAsync(int userId)
        {
            return await _filterRepo.GetFilterOptionsForUserAsync(userId);
        }

        /// <summary>
        /// Retrieves all unbooked room-time combinations matching the specified filter criteria.
        /// </summary>
        /// <param name="filter">Booking filter parameters.</param>
        /// <returns>A collection of available booking slots.</returns>
        public IEnumerable<Booking> GetAvailableBookingSlots(BookingFilter filter)
        {
            return _filterRepo.GetAvailableBookingSlots(filter);
        }

        /// <summary>
        /// Deletes the booking with the specified ID, provided the user has permission.
        /// </summary>
        /// <param name="bookingId">The unique ID of the booking to delete.</param>
        /// <param name="userId">The user attempting the deletion.</param>
        public async Task DeleteBookingAsync(int bookingId, int userId)
        {
            await _manageBookingRepo.DeleteBookingAsync(bookingId, userId);
        }
    }
}
