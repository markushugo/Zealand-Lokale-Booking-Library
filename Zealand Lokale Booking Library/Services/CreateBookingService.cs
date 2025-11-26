using System;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Repos;

namespace Zealand_Lokale_Booking_Library.Services
{
    public class CreateBookingService:ICreateBookingService
    {
        private readonly CreateBookingRepo _repo;

        public CreateBookingService(CreateBookingRepo repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// High-level wrapper used by UI (Razor Pages).
        /// Throws error if SP validation fails.
        /// </summary>
        public async Task<int> CreateBookingAsync(
            int userId,
            int roomId,
            DateTime date,
            TimeSpan startTime,
            int? smartBoardId)
        {
            if (date.Date < DateTime.Today)
                throw new ArgumentException("You cannot book a room for a past date.");

            if (startTime < TimeSpan.FromHours(7) || startTime > TimeSpan.FromHours(18))
                throw new ArgumentException("Start time must be between 07:00 and 18:00.");

            return await _repo.CreateBookingAsync(userId, roomId, date, startTime, smartBoardId);
        }
    }
}
