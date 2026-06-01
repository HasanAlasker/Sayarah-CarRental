using CarRental.Data;
using CarRental.Filters;
using CarRental.Models;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers;

[Authenticated]
[RequireRole("Admin")]
public class AdminController : Controller
{
    private readonly RentalDbContext _db;

    public AdminController(RentalDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Dashboard()
    {
        ViewBag.TotalCars = await _db.Cars.CountAsync();
        ViewBag.TotalRentals = await _db.Rentals.CountAsync();
        ViewBag.ActiveRentals = await _db.Rentals.CountAsync(r => r.EndDate >= DateTime.UtcNow);
        ViewBag.TotalUsers = await _db.Users.CountAsync();

        var recentRentals = await _db.Rentals
            .Include(r => r.Car)
            .Include(r => r.Renter)
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .ToListAsync();

        return View(recentRentals);
    }

    public async Task<IActionResult> Cars()
    {
        var cars = await _db.Cars
            .Include(c => c.Fuel)
            .Include(c => c.Type)
            .Include(c => c.Transmission)
            .Include(c => c.Owner)
            .ToListAsync();
        return View(cars);
    }

    public async Task<IActionResult> AddCar()
    {
        ViewBag.Fuels = await _db.Fuels.ToListAsync();
        ViewBag.Types = await _db.Types.ToListAsync();
        ViewBag.Transmissions = await _db.Transmissions.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddCar(CarViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Fuels = await _db.Fuels.ToListAsync();
            ViewBag.Types = await _db.Types.ToListAsync();
            ViewBag.Transmissions = await _db.Transmissions.ToListAsync();
            return View(model);
        }

        var userId = HttpContext.Session.GetInt32("UserId");

        var car = new Car
        {
            Name = model.Name,
            Make = model.Make,
            Year = model.Year,
            Seats = model.Seats,
            CostPerDay = model.CostPerDay,
            FuelId = model.FuelId,
            TypeId = model.TypeId,
            TransmissionId = model.TransmissionId,
            OwnerId = userId.Value
        };

        _db.Cars.Add(car);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Cars));
    }

    public async Task<IActionResult> Rentals()
    {
        var rentals = await _db.Rentals
            .Include(r => r.Car)
            .Include(r => r.Renter)
            .Include(r => r.CarOwner)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
        return View(rentals);
    }
}
