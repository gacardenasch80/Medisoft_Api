using MedisoftAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedisoftAPI.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<AppUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Username).IsUnique();
            e.Property(x => x.Username).IsRequired().HasMaxLength(100);
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.Role).IsRequired().HasMaxLength(50);
        });
    }
}
