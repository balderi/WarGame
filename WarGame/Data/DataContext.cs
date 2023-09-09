using Microsoft.EntityFrameworkCore;

namespace WarGame.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        { 

        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserData> UserData { get; set; }
    }
}
