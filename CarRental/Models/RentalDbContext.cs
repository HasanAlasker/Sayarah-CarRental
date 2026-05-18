using Microsoft.EntityFrameworkCore;

namespace CarRental.Models;

public class RentalDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Transmission> Transmissions { get; set; }  
    public DbSet<Type> Types { get; set; }
    public DbSet<Fuel> Fuels { get; set; }
    public DbSet<Roles> Roles { get; set; }
    public DbSet<UserRoles> UserRoles { get; set; }

    public RentalDbContext(DbContextOptions<RentalDbContext> options) : base(options)   
    {
            
    }
}