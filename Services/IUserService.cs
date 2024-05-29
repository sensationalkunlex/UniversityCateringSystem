
using UniversityCateringSystem.Models;

namespace UniversityCateringSystem.Controllers
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
    }
}
