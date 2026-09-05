// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// BomDbContext.cs
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.EntityFrameworkCore;
using Sample.Api.Entities;

namespace Sample.Api;

public class BomDbContext(DbContextOptions<BomDbContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<Bom> Boms => Set<Bom>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Oracle は非クォート識別子を大文字で格納するため、大文字名へ明示的にマッピングして
        // DDL(非クォート)と一致させる。これを怠ると EF が "Items" 等をクォート出力して別オブジェクト扱いになる。

        modelBuilder.Entity<Item>(e =>
        {
            e.ToTable("ITEM");
            e.HasKey(x => x.ItemCode);
            e.Property(x => x.ItemCode).HasColumnName("ITEM_CODE").HasColumnType("VARCHAR2(20)");
            e.Property(x => x.ItemName).HasColumnName("ITEM_NAME").HasColumnType("NVARCHAR2(40)");
        });

        modelBuilder.Entity<Material>(e =>
        {
            e.ToTable("MATERIAL");
            e.HasKey(x => x.ItemCode);
            e.Property(x => x.ItemCode).HasColumnName("ITEM_CODE").HasColumnType("VARCHAR2(20)");
            e.Property(x => x.ItemName).HasColumnName("ITEM_NAME").HasColumnType("NVARCHAR2(40)");
        });

        modelBuilder.Entity<Bom>(e =>
        {
            e.ToTable("BOM");

            // 複合主キー (item_code, m_item_code)
            e.HasKey(x => new { x.ItemCode, x.MItemCode });

            e.Property(x => x.ItemCode).HasColumnName("ITEM_CODE").HasColumnType("VARCHAR2(20)");
            e.Property(x => x.MItemCode).HasColumnName("M_ITEM_CODE").HasColumnType("VARCHAR2(20)");
            e.Property(x => x.Requirement).HasColumnName("REQUIREMENT").HasColumnType("NUMBER(9,2)");

            // FK: Bom.item_code -> Item.item_code
            e.HasOne(x => x.Item)
             .WithMany(i => i.Boms)
             .HasForeignKey(x => x.ItemCode)
             .OnDelete(DeleteBehavior.Restrict);   // 削除順はリポジトリ側で明示制御するため自動カスケードはしない

            // FK: Bom.m_item_code -> Material.item_code
            e.HasOne(x => x.Material)
             .WithMany(m => m.Boms)
             .HasForeignKey(x => x.MItemCode)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
