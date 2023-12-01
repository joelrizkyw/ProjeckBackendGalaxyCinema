using Microsoft.EntityFrameworkCore;

namespace GalaxyCinemaBackEnd.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> MsUser { get; set; }
        public DbSet<Category> MsCategory { get; set; }
    }
}
