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
    }
}
