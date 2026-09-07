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

public sealed class ItemConfiguration
    : IEntityTypeConfiguration<ItemRecord>
{
    public void Configure(EntityTypeBuilder<ItemRecord> builder)
    {
        builder.ToTable("ITEM");

        builder.HasKey(x => x.ItemCode);

        builder.Property(x => x.ItemCode)
            .HasColumnName("ITEM_CODE")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ItemName)
            .HasColumnName("ITEM_NAME")
            .HasMaxLength(100)
            .IsRequired();
    }
}