using Microsoft.EntityFrameworkCore;
using Sample.Persistence.Records;

namespace Sample.Persistence;

/// <summary>
/// ORACLE Database へのアクセスを行う DbContext。
/// テーブル構造は docs/design/データベース.png に準拠する。
/// </summary>
public sealed class SampleDbContext : DbContext
{
    public SampleDbContext(DbContextOptions<SampleDbContext> options)
        : base(options)
    {
    }

    public DbSet<ItemRecord> Items => Set<ItemRecord>();

    public DbSet<MaterialRecord> Materials => Set<MaterialRecord>();

    public DbSet<BomRecord> Boms => Set<BomRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemRecord>(entity =>
        {
            entity.ToTable("ITEM");
            entity.HasKey(e => e.ItemCode);

            entity.Property(e => e.ItemCode)
                .HasColumnName("ITEM_CODE")
                .HasColumnType("VARCHAR2(20)")
                .IsRequired();

            entity.Property(e => e.ItemName)
                .HasColumnName("ITEM_NAME")
                .HasColumnType("NVARCHAR2(40)")
                .IsRequired();
        });

        modelBuilder.Entity<MaterialRecord>(entity =>
        {
            entity.ToTable("MATERIAL");
            entity.HasKey(e => e.ItemCode);

            entity.Property(e => e.ItemCode)
                .HasColumnName("ITEM_CODE")
                .HasColumnType("VARCHAR2(20)")
                .IsRequired();

            entity.Property(e => e.ItemName)
                .HasColumnName("ITEM_NAME")
                .HasColumnType("NVARCHAR2(40)")
                .IsRequired();
        });

        modelBuilder.Entity<BomRecord>(entity =>
        {
            entity.ToTable("BOM");
            entity.HasKey(e => new { e.ItemCode, e.MItemCode });

            entity.Property(e => e.ItemCode)
                .HasColumnName("ITEM_CODE")
                .HasColumnType("VARCHAR2(20)")
                .IsRequired();

            entity.Property(e => e.MItemCode)
                .HasColumnName("M_ITEM_CODE")
                .HasColumnType("VARCHAR2(20)")
                .IsRequired();

            entity.Property(e => e.Requirement)
                .HasColumnName("REQUIREMENT")
                .HasColumnType("NUMBER(9,2)")
                .IsRequired();

            entity.HasOne<ItemRecord>()
                .WithMany()
                .HasForeignKey(e => e.ItemCode)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<MaterialRecord>()
                .WithMany()
                .HasForeignKey(e => e.MItemCode)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
