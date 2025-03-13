using Dal.Configurations;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public class MainContext : DbContext
{
    public DbSet<BotUser> Users { get; set; }
    public DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;User ID=sa;Password=1;Initial Catalog=UserRegistration;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BotUserConfiguration());
        modelBuilder.ApplyConfiguration(new UserInfoConfiguration());
    }
}
