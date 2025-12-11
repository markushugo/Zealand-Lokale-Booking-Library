using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly string _connectionString;

        public UserRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User? GetUserByCredentials(string email, string password)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
        SELECT UserID, Name, Email, Password, Phone, UserTypeID
        FROM dbo.[User]
        WHERE Email = @Email AND Password = @Password";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new User
            {
                UserID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3),
                Phone = reader.IsDBNull(4) ? null : reader.GetString(4), // <- FIX HER!
                UserTypeID = reader.GetInt32(5)
            };
        }


    }
}
