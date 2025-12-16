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
    /// Provides access to filter-related data and available booking slots,
    /// including fetching filter options for a specific user and retrieving
    /// free room booking times based on user-defined criteria..
    /// </summary>
    public class FilterRepo: IFilterRepo
    {
        private readonly string _connectionString;
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRepository"/> class,
        /// using the provided database connection string.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string used to connect to the SQL Server database.
        /// </param>
        public FilterRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retrieves all filter options accessible to the specified user.
        /// This includes:
        /// <list type="bullet">
        /// <item><description>Departments</description></item>
        /// <item><description>Buildings</description></item>
        /// <item><description>Room types</description></item>
        /// <item><description>Hardcoded time slot options</description></item>
        /// <item><description>Hardcoded level options</description></item>
        /// </list>
        /// The data is sourced from the stored procedure <c>dbo.GetFilterOptionsForUser</c>.
        /// </summary>
        /// <param name="userId">The ID of the user requesting filter options.</param>
        /// <returns>A <see cref="FilterOptions"/> object containing all available filter values.</returns>
        public async Task<FilterOptions> GetFilterOptionsForUserAsync(int userId)
        {
            var filters = new FilterOptions();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.GetFilterOptionsForUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserID", userId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var key = reader["Value"]?.ToString() ?? string.Empty;
                var value = reader["Text"]?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(key))
                    filters.Departments[key] = value;
            }
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var key = reader["Value"]?.ToString() ?? string.Empty;
                    var value = reader["Text"]?.ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(key))
                        filters.Buildings[key] = value;
                }
            }
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var key = reader["Value"]?.ToString() ?? string.Empty;
                    var value = reader["Text"]?.ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(key))
                        filters.RoomTypes[key] = value;
                }
            }
            // Hardcoded TimeSlots
            filters.TimeSlots = new Dictionary<string, string>
            {
                { "8",  "8-10" },
                { "10", "10-12" },
                { "12", "12-14" },
                { "14", "14-16" }
            };
            // Hardcoded LevelOptions
            filters.LevelOptions = new Dictionary<string, string>
            {
                { "1", "1" },
                { "2", "2" },
                { "3", "3" }
            };
            return filters;
        }

        /// <summary>
        /// Retrieves all available (unbooked) booking slots based on the specified filter criteria.
        /// This method calls the stored procedure <c>dbo.usp_GetAvailableBookingSlots</c>,
        /// which returns room/time combinations where no booking exists.
        /// </summary>
        /// <param name="filter">A <see cref="BookingFilter"/> object containing date, user ID, and filter lists.</param>
        /// <returns>An enumerable collection of <see cref="Booking"/> objects representing free booking slots.</returns>
        public IEnumerable<Booking> GetAvailableBookingSlots(BookingFilter filter)
        {
            var list = new List<Booking>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.usp_GetAvailableBookingSlots", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserID", filter.UserID);
            cmd.Parameters.AddWithValue("@Date", filter.Date.Date);

            _addIntListParameter(cmd, "@DepartmentIds", "dbo.IntList", filter.DepartmentIds);
            _addIntListParameter(cmd, "@BuildingIds", "dbo.IntList", filter.BuildingIds);
            _addIntListParameter(cmd, "@RoomIds", "dbo.IntList", filter.RoomIds);
            _addIntListParameter(cmd, "@RoomTypeIds", "dbo.IntList", filter.RoomTypeIds);
            _addLevelListParameter(cmd, "@Levels", "dbo.LevelList", filter.Levels);
            _addTimeListParameter(cmd, "@Times", "dbo.TimeList", filter.Times);

            conn.Open();
            using var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var booking = new Booking
                {
                    BookingID = rdr["BookingID"] as int?,
                    UserID = rdr["UserID"] as int?,
                    UserName = rdr["UserName"] as string,
                    SmartBoardID = rdr["SmartBoardID"] as int?,

                    Date = rdr.GetDateTime(rdr.GetOrdinal("Date")),
                    StartTime = rdr.GetTimeSpan(rdr.GetOrdinal("StartTime")),
                    RoomID = rdr.GetInt32(rdr.GetOrdinal("RoomID")),
                    RoomName = rdr.GetString(rdr.GetOrdinal("RoomName")),
                    Level = rdr.GetString(rdr.GetOrdinal("Level")),
                    RoomTypeID = rdr.GetInt32(rdr.GetOrdinal("RoomTypeID")),
                    RoomType = rdr.GetString(rdr.GetOrdinal("RoomType")),
                    Capacity = rdr.GetInt32(rdr.GetOrdinal("Capacity")),
                    BuildingID = rdr.GetInt32(rdr.GetOrdinal("BuildingID")),
                    BuildingName = rdr.GetString(rdr.GetOrdinal("BuildingName")),
                    DepartmentID = rdr.GetInt32(rdr.GetOrdinal("DepartmentID")),
                    DepartmentName = rdr.GetString(rdr.GetOrdinal("DepartmentName"))
                };

                list.Add(booking);
            }

            return list;
        }

        private static void _addIntListParameter(
            SqlCommand cmd,
            string paramName,
            string typeName,
            List<int>? values)
        {
            // Create DataTable matching dbo.IntList (Id INT)
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));

            if (values != null)
            {
                foreach (var id in values)
                    table.Rows.Add(id);
            }

            var parameter = cmd.Parameters.AddWithValue(paramName, table);
            parameter.SqlDbType = SqlDbType.Structured;
            parameter.TypeName = typeName;
        }

        private static void _addLevelListParameter(
            SqlCommand cmd,
            string paramName,
            string typeName,
            List<string>? levels)
        {
            // Create DataTable matching dbo.LevelList (Level VARCHAR(3))
            var table = new DataTable();
            table.Columns.Add("Level", typeof(string));

            if (levels != null)
            {
                foreach (var level in levels)
                    table.Rows.Add(level);
            }

            var parameter = cmd.Parameters.AddWithValue(paramName, table);
            parameter.SqlDbType = SqlDbType.Structured;
            parameter.TypeName = typeName;
        }

        private static void _addTimeListParameter(
            SqlCommand cmd,
            string paramName,
            string typeName,
            List<TimeOnly>? times)
        {
            // Create DataTable matching dbo.TimelList (StartTime TIME)
            var table = new DataTable();
            table.Columns.Add("StartTime", typeof(TimeSpan));

            if (times != null)
            {
                foreach (var t in times)
                table.Rows.Add(t.ToTimeSpan());
            }

            var parameter = cmd.Parameters.AddWithValue(paramName, table);
            parameter.SqlDbType = SqlDbType.Structured;
            parameter.TypeName = typeName;
        }

    }
}
