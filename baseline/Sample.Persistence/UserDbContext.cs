using Microsoft.EntityFrameworkCore;
using Sample.Persistence.Entities;

namespace Sample.Persistence;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("USERS");
            e.HasKey(x => x.UserId);
            e.Property(x => x.UserId).HasColumnName("USER_ID").HasColumnType("VARCHAR2(20)");
            e.Property(x => x.Password).HasColumnName("PASSWORD").HasColumnType("VARCHAR2(20)");
            e.Property(x => x.UserName).HasColumnName("USER_NAME").HasColumnType("NVARCHAR2(40)");
        });
    }
}
