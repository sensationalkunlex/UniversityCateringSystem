
namespace UniversityCateringSystem.Controllers
{
    internal interface IUserService
    {
        Task GetUserByEmailAsync(string email);
    }
}
