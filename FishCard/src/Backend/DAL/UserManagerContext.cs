using CodeWithSaar.FishCard.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeWithSaar.FishCard.DataAccess;

public class UserManagerContext : DbContext
{
    public UserManagerContext(DbContextOptions<UserManagerContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
}