
using Microsoft.EntityFrameworkCore;
using UniversityCateringSystem.Data;
using UniversityCateringSystem.Models;

namespace UniversityCateringSystem.Controllers
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            try
            {
               
                if (user == null)
                {
                    var newUser = new User
                    {
                        Email = email,
                        Role=Role.Customer,
                        Id = Guid.NewGuid(),
                        NewUser=true
                    };
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                    return newUser;
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(email, ex);
            }
        }
    }
}
