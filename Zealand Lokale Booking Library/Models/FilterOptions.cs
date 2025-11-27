using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Lokale_Booking_Library.Models
{
    public class FilterOptions
    {
        public Dictionary<string, string> Departments { get; set; } = new();
        public Dictionary<string, string> Buildings { get; set; } = new();
        public Dictionary<string, string> RoomTypes { get; set; } = new();
        public Dictionary<string, string> TimeSlots { get; set; } = new();
        public Dictionary<string, string> LevelOptions { get; set; } = new();
    }
}
