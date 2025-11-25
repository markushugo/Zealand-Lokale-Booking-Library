using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingDbContext _context;
        public BookingService(BookingDbContext context)
        {
            _context = context;
        }

        public IEnumerable<string> GetDepartments()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetBuildings()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetFloors()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetRooms()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetTimes()
        {
            throw new NotImplementedException();
        }
        public IEnumerable<RoomBooking> GetBookings(BookingFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
