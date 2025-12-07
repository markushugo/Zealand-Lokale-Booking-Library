using System;
using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Repos;

namespace Zealand_Lokale_Booking_Library.Services
{
    public class UserService
    {
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Attempts to authenticate the user and returns a SessionID GUID on success.
        /// </summary>
        public async Task<(bool Success, Guid? SessionId, string Message)> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, null, "Email is required.");

            if (string.IsNullOrWhiteSpace(password))
                return (false, null, "Password is required.");

            var user = await _userRepo.AuthenticateUserAsync(email, password);

            if (user == null)
            {
                return (false, null, "Invalid email or password.");
            }

            // Generate a new SessionId for the authenticated user
            var sessionId = Guid.NewGuid();

            return (true, sessionId, "Login successful.");
        }
    }
}
