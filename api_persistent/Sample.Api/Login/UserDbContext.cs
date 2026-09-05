using Microsoft.EntityFrameworkCore;
using Sample.Api.Entities;

namespace Sample.Api;

/*
 * baseline との差
 * DbContext も API 側に定義する
 */
public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    // 機能追加時にパーシステント層でやること
    // 2-1. このコンテキストで利用する表のレコードセットを用意
    public DbSet<User> Users => Set<User>();

    // 機能追加時にパーシステント層でやること
    // 2-2. このコンテキストで利用する表の形を指定
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
