using System;
using System.Threading.Tasks;

namespace Zealand_Lokale_Booking_Library.Services
{
    public interface ICreateBookingService
    {
        Task<int> CreateBookingAsync(
            int userId,
            int roomId,
            DateTime date,
            TimeSpan startTime,
            int? smartBoardId);
    }
}

