using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Models;
using Zealand_Lokale_Booking_Library.Repos;

namespace Zealand_Lokale_Booking_Library.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public Task<User?> AuthenticateAsync(string email, string password)
        {
            return _userRepo.AuthenticateUserAsync(email, password);
        }
    }
}

