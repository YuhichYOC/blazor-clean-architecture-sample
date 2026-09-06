// OracleContext.cs
// 
// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
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
using Sample.Persistence.Records;

namespace Sample.Persistence.Context;

public sealed class OracleContext : DbContext
{
    public OracleContext(
        DbContextOptions<OracleContext> options)
        : base(options)
    {
    }

    public DbSet<ItemRecord> Items => Set<ItemRecord>();

    public DbSet<MaterialRecord> Materials => Set<MaterialRecord>();

    public DbSet<BomRecord> Boms => Set<BomRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureItem(modelBuilder);

        ConfigureMaterial(modelBuilder);

        ConfigureBom(modelBuilder);
    }

    private static void ConfigureItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemRecord>(entity =>
        {
            entity.ToTable("ITEM");

            entity.HasKey(x => x.ItemCode);

            entity.Property(x => x.ItemCode)
                  .HasColumnName("ITEM_CODE");

            entity.Property(x => x.ItemName)
                  .HasColumnName("ITEM_NAME");
        });
    }

    private static void ConfigureMaterial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaterialRecord>(entity =>
        {
            entity.ToTable("MATERIAL");

            entity.HasKey(x => x.MaterialCode);

            entity.Property(x => x.MaterialCode)
                  .HasColumnName("MATERIAL_CODE");

            entity.Property(x => x.MaterialName)
                  .HasColumnName("MATERIAL_NAME");
        });
    }

    private static void ConfigureBom(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BomRecord>(entity =>
        {
            entity.ToTable("BOM");

            entity.HasKey(x =>
                new
                {
                    x.ItemCode,
                    x.MaterialCode
                });

            entity.Property(x => x.ItemCode)
                  .HasColumnName("ITEM_CODE");

            entity.Property(x => x.MaterialCode)
                  .HasColumnName("MATERIAL_CODE");

            entity.Property(x => x.Requirement)
                  .HasColumnName("REQUIREMENT");

            entity.HasOne(x => x.Item)
                .WithMany(x => x.Boms)
                .HasForeignKey(x => x.ItemCode)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Material)
                .WithMany(x => x.Boms)
                .HasForeignKey(x => x.MaterialCode)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}