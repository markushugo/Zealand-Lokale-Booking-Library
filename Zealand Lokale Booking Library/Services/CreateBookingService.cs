using Zealand_Lokale_Booking_Library.Repos;
using System;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Repos;

namespace Zealand_Lokale_Booking_Library.Services
{
    public class CreateBookingService : ICreateBookingService
    {
        private readonly ICreateBookingRepo _repo;

        public CreateBookingService(ICreateBookingRepo repo)
        {
            _repo = repo;
        }

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

            return (int)await _repo.CreateBookingAsync(userId, roomId, date, startTime, smartBoardId);
        }
    }
}