using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Services
{
    public interface IBookingService
    {
        IEnumerable<string> GetDepartments();
        IEnumerable<string> GetBuildings();
        IEnumerable<int> GetFloors();
        IEnumerable<string> GetTypes();
        IEnumerable<string> GetRooms();
        IEnumerable<string> GetTimes();

        IEnumerable<RoomBooking> GetBookings(BookingFilter filter);
    }
}
