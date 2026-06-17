using Levelbuild.CodingChallenge.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Levelbuild.CodingChallenge.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(255);
            entity.HasIndex(c => c.Name).IsUnique();
            entity.Property(c => c.WebSite).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.DisplayName).IsRequired().HasMaxLength(255);
            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(255);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(255);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.DateOfBirth).IsRequired();
            entity.HasIndex(u => u.DisplayName).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasOne(u => u.Customer)
                  .WithMany()
                  .HasForeignKey(u => u.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}