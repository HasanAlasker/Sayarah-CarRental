using CarRental.Models;
using Microsoft.EntityFrameworkCore;
using Type = CarRental.Models.Type;

namespace CarRental.Data;

public class RentalDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Transmission> Transmissions { get; set; }
    public DbSet<Type> Types { get; set; }
    public DbSet<Fuel> Fuels { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRoles> UserRoles { get; set; }

    public RentalDbContext(DbContextOptions<RentalDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── UserRoles (composite key) ──────────────────────────
        modelBuilder.Entity<UserRoles>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRoles>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRoles>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        // ── Car → Owner (User) ─────────────────────────────────
        modelBuilder.Entity<Car>()
            .HasOne(c => c.Owner)
            .WithMany(u => u.Cars)
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // don't delete cars if user deleted

        // ── Car → LOVs ─────────────────────────────────────────
        modelBuilder.Entity<Car>()
            .HasOne(c => c.Fuel)
            .WithMany(f => f.Cars)
            .HasForeignKey(c => c.FuelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Car>()
            .HasOne(c => c.Type)
            .WithMany(t => t.Cars)
            .HasForeignKey(c => c.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Car>()
            .HasOne(c => c.Transmission)
            .WithMany(t => t.Cars)
            .HasForeignKey(c => c.TransmissionId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Rental → Renter (User) ─────────────────────────────
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Renter)
            .WithMany(u => u.Rentals)
            .HasForeignKey(r => r.RenterId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Rental → CarOwner (User) ───────────────────────────
        // Two FK to same table (User) — must name them explicitly
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.CarOwner)
            .WithMany()
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Rental → Car ───────────────────────────────────────
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Car)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}