using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Lokale_Booking_Library.Models
{
    public class RoomBooking
    {
        public string RoomNumber { get; set; }   
        public string Building { get; set; }     
        public int Floor { get; set; }           
        public string Type { get; set; }         
        public string Department { get; set; }   
        public string Time { get; set; }
    }
}
