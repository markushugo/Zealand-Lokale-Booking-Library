using System.Threading.Tasks;
using Zealand_Lokale_Booking_Library.Models;

namespace Zealand_Lokale_Booking_Library.Repos
{

    public interface IUserRepo
    {
        User? GetUserByCredentials(string email, string password);
    }
}
