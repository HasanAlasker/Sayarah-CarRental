using CarRental.Abstraction;
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

        modelBuilder.Entity<Car>()
            .HasOne(c => c.Owner)
            .WithMany(u => u.Cars)
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // don't delete cars if user deleted

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

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Renter)
            .WithMany(u => u.Rentals)
            .HasForeignKey(r => r.RenterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.CarOwner)
            .WithMany()
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Car)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
    private readonly IHttpContextAccessor _httpContextAccessor;

    // Add IHttpContextAccessor to constructor
    public RentalDbContext(DbContextOptions<RentalDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Get the logged in userId from session — null if not logged in (e.g. seeder)
        var userId = _httpContextAccessor.HttpContext?.Items["UserId"] as int?;

        var entries = ChangeTracker.Entries<TimeStamp>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.UpdatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}