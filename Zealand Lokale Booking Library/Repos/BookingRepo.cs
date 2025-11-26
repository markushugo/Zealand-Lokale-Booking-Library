using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Repos
{
    /// <summary>
    /// Provides methods for retrieving bookings from a SQL Server database
    /// using the <c>dbo.usp_GetFilteredBookings</c> stored procedure and
    /// table-valued parameters.
    /// </summary>
    public class BookingRepo
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingRepo"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string used to connect to the SQL Server instance.
        /// </param>
        public BookingRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retrieves bookings for a given date and optional filter values
        /// by executing the <c>dbo.usp_GetFilteredBookings</c> stored procedure.
        /// </summary>
        /// <param name="filter">
        /// An instance of <see cref="BookingFilter"/> that contains the user,
        /// date and optional filter lists (departments, buildings, rooms, etc.).
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a read-only list of <see cref="Booking"/> instances
        /// that match the supplied filter.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="filter"/> is <c>null</c>.
        /// </exception>
        public async Task<IReadOnlyList<Booking>> GetFilteredBookingsAsync(BookingFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            var bookings = new List<Booking>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.usp_GetFilteredBookings", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Scalar parameters
            command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int)
            {
                Value = filter.UserID
            });

            command.Parameters.Add(new SqlParameter("@Date", SqlDbType.Date)
            {
                Value = filter.Date.Date
            });

            // Table-valued parameters (TVPs)
            command.Parameters.Add(CreateIntListParameter("@DepartmentIds", "dbo.IntList", filter.DepartmentIds));
            command.Parameters.Add(CreateIntListParameter("@BuildingIds", "dbo.IntList", filter.BuildingIds));
            command.Parameters.Add(CreateIntListParameter("@RoomIds", "dbo.IntList", filter.RoomIds));
            command.Parameters.Add(CreateIntListParameter("@RoomTypeIds", "dbo.IntList", filter.RoomTypeIds));
            command.Parameters.Add(CreateLevelListParameter("@Levels", "dbo.LevelList", filter.Levels));
            command.Parameters.Add(CreateTimeListParameter("@Times", "dbo.TimeList", filter.Times));

            await connection.OpenAsync().ConfigureAwait(false);

            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var booking = MapBooking(reader);
                bookings.Add(booking);
            }

            return bookings;
        }

        /// <summary>
        /// Maps the current row of a <see cref="SqlDataReader"/> to a
        /// <see cref="Booking"/> instance.
        /// </summary>
        /// <param name="reader">
        /// The data reader positioned at the current record returned by the stored procedure.
        /// </param>
        /// <returns>
        /// A <see cref="Booking"/> object populated with the column values from the current row.
        /// </returns>
        private static Booking MapBooking(SqlDataReader reader)
        {
            int bookingIdOrdinal = reader.GetOrdinal("BookingID");
            int dateOrdinal = reader.GetOrdinal("Date");
            int startTimeOrdinal = reader.GetOrdinal("StartTime");
            int userIdOrdinal = reader.GetOrdinal("UserID");
            int userNameOrdinal = reader.GetOrdinal("UserName");
            int roomIdOrdinal = reader.GetOrdinal("RoomID");
            int roomNameOrdinal = reader.GetOrdinal("RoomName");
            int levelOrdinal = reader.GetOrdinal("Level");
            int roomTypeIdOrdinal = reader.GetOrdinal("RoomTypeID");
            int roomTypeOrdinal = reader.GetOrdinal("RoomType");
            int capacityOrdinal = reader.GetOrdinal("Capacity");
            int buildingIdOrdinal = reader.GetOrdinal("BuildingID");
            int buildingNameOrdinal = reader.GetOrdinal("BuildingName");
            int departmentIdOrdinal = reader.GetOrdinal("DepartmentID");
            int departmentNameOrdinal = reader.GetOrdinal("DepartmentName");
            int smartBoardIdOrdinal = reader.GetOrdinal("SmartBoardID");

            var booking = new Booking
            {
                BookingID = reader.GetInt32(bookingIdOrdinal),
                Date = reader.GetDateTime(dateOrdinal),
                StartTime = reader.GetTimeSpan(startTimeOrdinal),
                UserID = reader.GetInt32(userIdOrdinal),
                UserName = reader.IsDBNull(userNameOrdinal)
                                 ? null
                                 : reader.GetString(userNameOrdinal),
                RoomID = reader.GetInt32(roomIdOrdinal),
                RoomName = reader.GetString(roomNameOrdinal),
                Level = reader.GetString(levelOrdinal),
                RoomTypeID = reader.GetInt32(roomTypeIdOrdinal),
                RoomType = reader.GetString(roomTypeOrdinal),
                Capacity = reader.GetInt32(capacityOrdinal),
                BuildingID = reader.GetInt32(buildingIdOrdinal),
                BuildingName = reader.GetString(buildingNameOrdinal),
                DepartmentID = reader.GetInt32(departmentIdOrdinal),
                DepartmentName = reader.GetString(departmentNameOrdinal),
                SmartBoardID = reader.IsDBNull(smartBoardIdOrdinal)
                                 ? (int?)null
                                 : reader.GetInt32(smartBoardIdOrdinal)
            };

            return booking;
        }

        /// <summary>
        /// Creates a table-valued parameter of type <c>dbo.IntList</c> (or compatible)
        /// from a list of integer values.
        /// </summary>
        /// <param name="parameterName">The name of the SQL parameter (including the '@' prefix).</param>
        /// <param name="typeName">The SQL type name of the table-valued parameter (e.g. <c>dbo.IntList</c>).</param>
        /// <param name="values">The list of integer values to include in the table. May be <c>null</c>.</param>
        /// <returns>
        /// A <see cref="SqlParameter"/> configured as a structured parameter
        /// whose value is a <see cref="DataTable"/> with a single <c>Id</c> column.
        /// </returns>
        private static SqlParameter CreateIntListParameter(string parameterName, string typeName, List<int>? values)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));

            if (values != null)
            {
                foreach (var value in values)
                {
                    table.Rows.Add(value);
                }
            }

            return new SqlParameter(parameterName, SqlDbType.Structured)
            {
                TypeName = typeName,
                Value = table
            };
        }

        /// <summary>
        /// Creates a table-valued parameter of type <c>dbo.LevelList</c> (or compatible)
        /// from a list of level strings.
        /// </summary>
        /// <param name="parameterName">The name of the SQL parameter (including the '@' prefix).</param>
        /// <param name="typeName">The SQL type name of the table-valued parameter (e.g. <c>dbo.LevelList</c>).</param>
        /// <param name="values">The list of level values to include in the table. May be <c>null</c>.</param>
        /// <returns>
        /// A <see cref="SqlParameter"/> configured as a structured parameter
        /// whose value is a <see cref="DataTable"/> with a single <c>Level</c> column.
        /// </returns>
        private static SqlParameter CreateLevelListParameter(string parameterName, string typeName, List<string>? values)
        {
            var table = new DataTable();
            table.Columns.Add("Level", typeof(string));

            if (values != null)
            {
                foreach (var value in values)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        table.Rows.Add(value);
                    }
                }
            }

            return new SqlParameter(parameterName, SqlDbType.Structured)
            {
                TypeName = typeName,
                Value = table
            };
        }

        /// <summary>
        /// Creates a table-valued parameter of type <c>dbo.TimeList</c> (or compatible)
        /// from a list of <see cref="TimeOnly"/> values.
        /// </summary>
        /// <param name="parameterName">The name of the SQL parameter (including the '@' prefix).</param>
        /// <param name="typeName">The SQL type name of the table-valued parameter (e.g. <c>dbo.TimeList</c>).</param>
        /// <param name="values">The list of time values to include in the table. May be <c>null</c>.</param>
        /// <returns>
        /// A <see cref="SqlParameter"/> configured as a structured parameter
        /// whose value is a <see cref="DataTable"/> with a single <c>StartTime</c> column.
        /// </returns>
        private static SqlParameter CreateTimeListParameter(string parameterName, string typeName, List<TimeOnly>? values)
        {
            var table = new DataTable();
            table.Columns.Add("StartTime", typeof(TimeSpan));

            if (values != null)
            {
                foreach (var value in values)
                {
                    table.Rows.Add(value.ToTimeSpan());
                }
            }

            return new SqlParameter(parameterName, SqlDbType.Structured)
            {
                TypeName = typeName,
                Value = table
            };
        }

    }
}
