using UsermanagementApi.Models;
using UsermanagementApi.Data;
using Microsoft.EntityFrameworkCore;
namespace UsermanagementApi.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public List<User> GetUsers()
        {
            return new List<User>
{
    new User { Id = 1, Name = "admin", Password = "admin123" }
};
        }

        public User AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
    }
}