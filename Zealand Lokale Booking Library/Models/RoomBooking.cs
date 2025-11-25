using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Lokale_Booking_Library.Models
{
    public class RoomBooking
    {
        public string RoomId { get; set; }   
        public string BuildingId { get; set; }     
        public int FloorId { get; set; }           
        public string TypeId { get; set; }         
        public string DepartmentId { get; set; }   
        public string TimeId { get; set; }
    }
}
