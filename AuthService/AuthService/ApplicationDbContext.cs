using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> ApplicationUsers { get; set; }
    }
}
