using Common.Model;
using Microsoft.EntityFrameworkCore;

namespace Server
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<User> Users { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }
    }
}
