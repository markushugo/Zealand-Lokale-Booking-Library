using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Lokale_Booking_Library.Models
{
    /// <summary>
    /// Represents the input parameters for the stored procedure 
    /// <c>dbo.usp_GetFilteredBookings</c>. 
    /// This class is used to supply filter criteria when retrieving
    /// bookings based on user, date, and optional selection filters.
    /// </summary>
    public class BookingFilter
    {
        /// <summary>
        /// Gets or sets the ID of the user requesting the bookings.
        /// Corresponds to <c>@UserID</c>.
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// Gets or sets the date for which bookings are retrieved.
        /// Corresponds to <c>@Date</c>.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets an optional list of department IDs to filter by.
        /// Corresponds to the table-valued parameter <c>@DepartmentIds</c>
        /// using SQL type <c>dbo.IntList</c>.
        /// </summary>
        public List<int>? DepartmentIds { get; set; }
        /// <summary>
        /// Gets or sets an optional list of building IDs to filter by.
        /// Corresponds to the table-valued parameter <c>@BuildingIds</c>
        /// using SQL type <c>dbo.IntList</c>.
        /// </summary>
        public List<int>? BuildingIds { get; set; }
        /// <summary>
        /// Gets or sets an optional list of building IDs to filter by.
        /// Corresponds to the table-valued parameter <c>@BuildingIds</c>
        /// using SQL type <c>dbo.IntList</c>.
        /// </summary>
        public List<int>? RoomIds { get; set; }
        /// <summary>
        /// Gets or sets an optional list of room type IDs to filter by.
        /// Corresponds to the table-valued parameter <c>@RoomTypeIds</c>
        /// using SQL type <c>dbo.IntList</c>.
        /// </summary>
        public List<int>? RoomTypeIds { get; set; }
        /// <summary>
        /// Gets or sets an optional list of level identifiers.
        /// Corresponds to the table-valued parameter <c>@Levels</c>
        /// using SQL type <c>dbo.LevelList</c>.
        /// </summary>
        public List<string>? Levels { get; set; }
        /// <summary>
        /// Gets or sets an optional list of start times to filter by.
        /// Corresponds to the table-valued parameter <c>@Times</c>
        /// using SQL type <c>dbo.TimeList</c>.
        /// </summary>
        public List<TimeOnly>? Times { get; set; }
    }
}
