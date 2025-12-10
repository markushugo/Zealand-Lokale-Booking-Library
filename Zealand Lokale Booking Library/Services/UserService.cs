using System;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Models;
using Zealand_Lokale_Booking_Library.Repos;

namespace Zealand_Lokale_Booking_Library.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _repo;

        public UserService(IUserRepo repo)
        {
            _repo = repo;
        }

        public User? ValidateLogin(string email, string password)
        {
            return _repo.GetUserByCredentials(email, password);
        }
    }
}
