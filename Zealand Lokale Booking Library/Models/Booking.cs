using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Lokale_Booking_Library.Models
{
    /// <summary>
    /// Represents a booking or available room time slot returned from 
    /// the <c>usp_GetFilteredBookings</c> stored procedure.
    /// A time slot is considered available when <see cref="IsBooked"/> is <c>false</c>.
    /// </summary>
    public class Booking
    {
        /// <summary>
        /// Gets or sets the unique identifier of the booking.
        /// This value is <c>null</c> when the time slot is available.
        /// </summary>
        public int? BookingID { get; set; }

        /// <summary>
        /// Gets a value indicating whether the time slot is booked.
        /// Returns <c>true</c> when <see cref="BookingID"/> has a value;
        /// otherwise <c>false</c>.
        /// </summary>
        public bool IsBooked => BookingID.HasValue;

        /// <summary>
        /// Gets or sets the date of the booking or available time slot.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the starting time of the booking or available time slot.
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Gets or sets the userID associated with the booking.
        /// This value is <c>null</c> when the time slot is available.
        /// </summary>
        public int? UserID { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who made the booking.
        /// This value is <c>null</c> when the time slot is available.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the roomID related to the booking or available slot.
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// Gets or sets the name of the room.
        /// </summary>
        public string RoomName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the level/floor where the room is located.
        /// </summary>
        public string Level { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the room type.
        /// </summary>
        public int RoomTypeID { get; set; }

        /// <summary>
        /// Gets or sets the name of the room type.
        /// </summary>
        public string RoomType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the capacity of the room.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Gets or sets the ID of the building the room is located in.
        /// </summary>
        public int BuildingID { get; set; }

        /// <summary>
        /// Gets or sets the building name.
        /// </summary>
        public string BuildingName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the departmentID that the building belongs to.
        /// </summary>
        public int DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the name of the department.
        /// </summary>
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the SmartBoard ID associated with the booking, if any.
        /// This will be <c>null</c> if no SmartBoard is assigned.
        /// </summary>
        public int? SmartBoardID { get; set; }

    }
}
