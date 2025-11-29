using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Lokale_Booking_Library.Models
{
    /// <summary>
    /// Represents the available filter options for the booking system,
    /// including departments, buildings, room types, time slots, and levels.
    /// </summary>
    public class FilterOptions
    {
        /// <summary>
        /// Gets or sets the collection of department filter options.
        /// The key represents the department ID, and the value represents the department name.
        /// </summary>
        public Dictionary<string, string> Departments { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of building filter options.
        /// The key represents the building ID, and the value represents the building name.
        /// </summary>
        public Dictionary<string, string> Buildings { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of room type filter options.
        /// The key represents the room type ID, and the value represents the room type name.
        /// </summary>
        public Dictionary<string, string> RoomTypes { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of time slot filter options.
        /// The key represents the time value (e.g., "08:00"), and the value represents a display label.
        /// </summary>
        public Dictionary<string, string> TimeSlots { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of level filter options.
        /// The key represents the level value, and the value represents a display label for the level.
        /// </summary>
        public Dictionary<string, string> LevelOptions { get; set; } = new();
    }
}
