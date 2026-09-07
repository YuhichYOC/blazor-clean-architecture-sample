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
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Persistence.Records;

namespace Sample.Persistence.Configurations;

public sealed class BomConfiguration
    : IEntityTypeConfiguration<BomRecord>
{
    public void Configure(EntityTypeBuilder<BomRecord> builder)
    {
        builder.ToTable("BOM");

        builder.HasKey(x => new
        {
            x.ItemCode,
            x.MaterialCode
        });

        builder.Property(x => x.ItemCode)
            .HasColumnName("ITEM_CODE")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.MaterialCode)
            .HasColumnName("MATERIAL_CODE")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Requirement)
            .HasColumnName("REQUIREMENT")
            .IsRequired();

        builder.HasOne(x => x.Item)
            .WithMany(x => x.Boms)
            .HasForeignKey(x => x.ItemCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Material)
            .WithMany(x => x.Boms)
            .HasForeignKey(x => x.MaterialCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}