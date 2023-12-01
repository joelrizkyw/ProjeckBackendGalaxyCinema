using GalaxyCinemaBackEnd.Models.GalaxyCinemaDB;
using Microsoft.EntityFrameworkCore;

namespace GalaxyCinemaBackEnd.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<BookingDetail> BookingDetail { get; set; }
        public DbSet<BookingHeader> BookingHeader { get; set; }
        public DbSet<Movie> Movie { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<Studio> Studio { get; set; }
        public DbSet<StudioType> StudioType { get; set; }
        public DbSet<StudioPrice> StudioPrice { get; set; }
        public DbSet<StudioSeat> StudioSeat { get; set; }
        public DbSet<Theater> Theater { get; set; }
        public DbSet<User> User { get; set; }
    }
}
