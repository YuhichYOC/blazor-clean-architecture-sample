using Microsoft.EntityFrameworkCore;
using Sample.Persistence.Entities;

namespace Sample.Persistence;

/*
 * 機能追加時にパーシステント層でやること
 * 2. DbContext の追加
 *     コンテキスト ... ユースケースと対応する「『ひと塊の処理の流れ』で必要なデータ」
 *     コンテキストという言葉が意味するもの ... 大雑把に「現在アプリで行っているオペレーションの "context ( 文脈 )" である」と把握していい
 * 以下はログイン処理でデータベースとやり取りするデータの内容についての定義
 *     ITEM, MATERIAL, BOM はこの処理と関わりがないのでこの DbContext には出てこない
 *
 * クラスの宣言で指定する型パラメータが正しいか注意
 *     この例はログイン処理のリポジトリなので UserDbContext が正解
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
