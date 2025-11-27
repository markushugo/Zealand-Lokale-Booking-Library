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
    public class FilterRepository
    {
        private readonly string _connectionString;
        public FilterRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
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

            // -----------------------------
            // 1) Departments
            // -----------------------------
            while (await reader.ReadAsync())
            {
                var key = reader["Value"]?.ToString() ?? string.Empty;
                var value = reader["Text"]?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(key))
                    filters.Departments[key] = value;
            }

            // -----------------------------
            // 2) Buildings
            // -----------------------------
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

            // -----------------------------
            // 3) RoomTypes (all from RoomType table)
            // -----------------------------
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

            // -----------------------------
            // 4) Hardcoded TimeSlots
            // -----------------------------
            filters.TimeSlots = new Dictionary<string, string>
            {
                { "8",  "8-10" },
                { "10", "10-12" },
                { "12", "12-14" },
                { "14", "14-16" }
            };

            // -----------------------------
            // 5) Hardcoded LevelOptions
            // -----------------------------
            filters.LevelOptions = new Dictionary<string, string>
            {
                { "1", "1" },
                { "2", "2" },
                { "3", "3" }
            };

            return filters;
        }

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

            AddIntListParameter(cmd, "@DepartmentIds", "dbo.IntList", filter.DepartmentIds);
            AddIntListParameter(cmd, "@BuildingIds", "dbo.IntList", filter.BuildingIds);
            AddIntListParameter(cmd, "@RoomIds", "dbo.IntList", filter.RoomIds);
            AddIntListParameter(cmd, "@RoomTypeIds", "dbo.IntList", filter.RoomTypeIds);
            AddLevelListParameter(cmd, "@Levels", "dbo.LevelList", filter.Levels);
            AddTimeListParameter(cmd, "@Times", "dbo.TimeList", filter.Times);

            conn.Open();
            using var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var booking = new Booking
                {
                    // disse er altid null for ledige slots
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

        // -----------------------
        // AddIntListParameter
        // -----------------------
        private static void AddIntListParameter(
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


        // -----------------------
        // AddLevelListParameter
        // -----------------------
        private static void AddLevelListParameter(
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


        // -----------------------
        // AddTimeListParameter
        // -----------------------
        private static void AddTimeListParameter(
            SqlCommand cmd,
            string paramName,
            string typeName,
            List<TimeOnly>? times)
        {
            // SQL TIME = .NET TimeSpan
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
