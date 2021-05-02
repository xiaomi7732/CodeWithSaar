using Microsoft.EntityFrameworkCore;

namespace JWT.Example.WithSQLDB
{
    public class UserDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=UserDB.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}