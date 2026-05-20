using CarRental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(RentalDbContext db)
    {
        // Apply any pending migrations automatically
        await db.Database.MigrateAsync();

        await SeedRolesAsync(db);
        await SeedLovAsync(db);
        await SeedUsersAsync(db);
        await SeedCarsAsync(db);
        await SeedRentalsAsync(db);
    }

    // ── ROLES ──────────────────────────────────────────────────
    private static async Task SeedRolesAsync(RentalDbContext db)
    {
        if (await db.Roles.AnyAsync()) return;

        db.Roles.AddRange(
            new Role { Name = "Admin" },
            new Role { Name = "User" }
        );

        await db.SaveChangesAsync();
    }

    // ── LOVs (Fuels, Types, Transmissions) ─────────────────────
    private static async Task SeedLovAsync(RentalDbContext db)
    {
        if (!await db.Fuels.AnyAsync())
        {
            db.Fuels.AddRange(
                new Fuel { Name = "Petrol" },
                new Fuel { Name = "Diesel" },
                new Fuel { Name = "Electric" },
                new Fuel { Name = "Hybrid" }
            );
        }

        if (!await db.Types.AnyAsync())
        {
            db.Types.AddRange(
                new CarRental.Models.Type { Name = "Sedan" },
                new CarRental.Models.Type { Name = "SUV" },
                new CarRental.Models.Type { Name = "Hatchback" },
                new CarRental.Models.Type { Name = "Convertible" },
                new CarRental.Models.Type { Name = "Truck" }
            );
        }

        if (!await db.Transmissions.AnyAsync())
        {
            db.Transmissions.AddRange(
                new Transmission { Name = "Automatic" },
                new Transmission { Name = "Manual" }
            );
        }

        await db.SaveChangesAsync();
    }

    // ── USERS ──────────────────────────────────────────────────
    private static async Task SeedUsersAsync(RentalDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        var hasher = new PasswordHasher<User>();

        var admin = new User { Name = "Admin", Email = "admin@carrental.com", Password = "" };
        var alice = new User { Name = "Alice Smith", Email = "alice@example.com", Password = "" };
        var bob   = new User { Name = "Bob Jones",  Email = "bob@example.com",   Password = "" };

        admin.Password = hasher.HashPassword(admin, "Admin@123");
        alice.Password = hasher.HashPassword(alice, "Alice@123");
        bob.Password   = hasher.HashPassword(bob,   "Bob@123");

        db.Users.AddRange(admin, alice, bob);
        await db.SaveChangesAsync();

        // Assign roles
        var adminRole = await db.Roles.FirstAsync(r => r.Name == "Admin");
        var userRole  = await db.Roles.FirstAsync(r => r.Name == "User");

        db.UserRoles.AddRange(
            new UserRoles { UserId = admin.Id, RoleId = adminRole.Id },
            new UserRoles { UserId = alice.Id, RoleId = userRole.Id },
            new UserRoles { UserId = bob.Id,   RoleId = userRole.Id }
        );

        await db.SaveChangesAsync();
    }

    // ── CARS ───────────────────────────────────────────────────
    private static async Task SeedCarsAsync(RentalDbContext db)
    {
        if (await db.Cars.AnyAsync()) return;

        var alice = await db.Users.FirstAsync(u => u.Email == "alice@example.com");
        var bob   = await db.Users.FirstAsync(u => u.Email == "bob@example.com");

        var petrol    = await db.Fuels.FirstAsync(f => f.Name == "Petrol");
        var electric  = await db.Fuels.FirstAsync(f => f.Name == "Electric");
        var diesel    = await db.Fuels.FirstAsync(f => f.Name == "Diesel");

        var sedan     = await db.Types.FirstAsync(t => t.Name == "Sedan");
        var suv       = await db.Types.FirstAsync(t => t.Name == "SUV");
        var hatchback = await db.Types.FirstAsync(t => t.Name == "Hatchback");

        var automatic = await db.Transmissions.FirstAsync(t => t.Name == "Automatic");
        var manual    = await db.Transmissions.FirstAsync(t => t.Name == "Manual");

        db.Cars.AddRange(
            new Car
            {
                Name           = "Camry",
                Make           = "Toyota",
                Year           = 2022,
                Seats          = 5,
                CostPerDay     = 45.00m,
                OwnerId        = alice.Id,
                FuelId         = petrol.Id,
                TypeId         = sedan.Id,
                TransmissionId = automatic.Id
            },
            new Car
            {
                Name           = "Model 3",
                Make           = "Tesla",
                Year           = 2023,
                Seats          = 5,
                CostPerDay     = 85.00m,
                OwnerId        = alice.Id,
                FuelId         = electric.Id,
                TypeId         = sedan.Id,
                TransmissionId = automatic.Id
            },
            new Car
            {
                Name           = "RAV4",
                Make           = "Toyota",
                Year           = 2021,
                Seats          = 7,
                CostPerDay     = 65.00m,
                OwnerId        = bob.Id,
                FuelId         = petrol.Id,
                TypeId         = suv.Id,
                TransmissionId = automatic.Id
            },
            new Car
            {
                Name           = "Golf",
                Make           = "Volkswagen",
                Year           = 2020,
                Seats          = 5,
                CostPerDay     = 38.00m,
                OwnerId        = bob.Id,
                FuelId         = diesel.Id,
                TypeId         = hatchback.Id,
                TransmissionId = manual.Id
            }
        );

        await db.SaveChangesAsync();
    }

    // ── RENTALS ────────────────────────────────────────────────
    private static async Task SeedRentalsAsync(RentalDbContext db)
    {
        if (await db.Rentals.AnyAsync()) return;

        var alice  = await db.Users.FirstAsync(u => u.Email == "alice@example.com");
        var bob    = await db.Users.FirstAsync(u => u.Email == "bob@example.com");

        var camry  = await db.Cars.FirstAsync(c => c.Name == "Camry");
        var rav4   = await db.Cars.FirstAsync(c => c.Name == "RAV4");

        // Bob rents Alice's Camry (past rental)
        var days1 = 3;
        db.Rentals.Add(new Rental
        {
            RenterId   = bob.Id,
            CarId      = camry.Id,
            OwnerId    = alice.Id,
            StartDate  = DateTime.UtcNow.AddDays(-10),
            EndDate    = DateTime.UtcNow.AddDays(-10 + days1),
            TotalPrice  = days1 * camry.CostPerDay
        });

        // Alice rents Bob's RAV4 (upcoming rental)
        var days2 = 5;
        db.Rentals.Add(new Rental
        {
            RenterId   = alice.Id,
            CarId      = rav4.Id,
            OwnerId    = bob.Id,
            StartDate  = DateTime.UtcNow.AddDays(5),
            EndDate    = DateTime.UtcNow.AddDays(5 + days2),
            TotalPrice  = days2 * rav4.CostPerDay
        });

        await db.SaveChangesAsync();
    }
}