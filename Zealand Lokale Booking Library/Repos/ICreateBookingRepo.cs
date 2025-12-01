using System;
using System.Threading.Tasks;

namespace Zealand_Lokale_Booking_Library.Repos
{
    public interface ICreateBookingRepo
    {
        Task<int?> CreateBookingAsync(
            int userId,
            int roomId,
            DateTime date,
            TimeSpan startTime,
            int? smartBoardId);
    }
}
