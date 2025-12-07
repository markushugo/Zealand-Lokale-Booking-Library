using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Repos
{
    public interface IUserRepo
    {
        Task<User?> AuthenticateUserAsync(string email, string password);
    }
}
