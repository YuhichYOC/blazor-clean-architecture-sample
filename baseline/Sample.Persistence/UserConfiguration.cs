using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Domain;

namespace Sample.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("USERS");
        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasColumnName("USER_ID").HasMaxLength(20);

        builder.Property(u => u.UserName)
            .HasColumnName("USER_NAME").HasMaxLength(40);

        // Password は private プロパティ。規約では拾われないので明示的にマップする
        builder.Property("Password")
            .HasColumnName("PASSWORD").HasMaxLength(20);
    }
}
