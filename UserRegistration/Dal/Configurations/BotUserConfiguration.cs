using Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dal.Configurations;

public class BotUserConfiguration : IEntityTypeConfiguration<BotUser>
{
    public void Configure(EntityTypeBuilder<BotUser> builder)
    {
        builder.ToTable("User");
        builder.HasKey(u => u.BotUserId);

        builder.Property(u => u.FirstName).IsRequired(false);
        builder.Property(u => u.LastName).IsRequired(false);
        builder.Property(u => u.Username).IsRequired(false);
        builder.Property(u => u.PhoneNumber).IsRequired(false);
        builder.Property(u => u.UpdatedAt).IsRequired(false);

        builder.HasIndex(u => u.TelegramUserId).IsUnique(true);

        builder.HasOne(bu => bu.UserInfo)
            .WithOne(ui => ui.BotUser)
            .HasForeignKey<UserInfo>(ui => ui.BotUserId);
    }
}
