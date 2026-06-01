using CarRental.Data;
using CarRental.Filters;
using CarRental.Models;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers;

[Authenticated]
public class RentalController : Controller
{
    private readonly RentalDbContext _db;

    public RentalController(RentalDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> MyRentals()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var rentals = await _db.Rentals
            .Include(r => r.Car)
            .Include(r => r.CarOwner)
            .Where(r => r.RenterId == userId)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();

        return View(rentals);
    }

    [HttpPost]
    public async Task<IActionResult> Book(RentalViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", "Car", new { id = model.CarId });
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        var car = await _db.Cars.FindAsync(model.CarId);
        if (car == null) return NotFound();

        // Check availability
        var isAvailable = !await _db.Rentals.AnyAsync(r => 
            r.CarId == model.CarId && 
            ((model.StartDate >= r.StartDate && model.StartDate < r.EndDate) || 
             (model.EndDate > r.StartDate && model.EndDate <= r.EndDate) ||
             (r.StartDate >= model.StartDate && r.StartDate < model.EndDate)));

        if (!isAvailable)
        {
            TempData["Error"] = "Car is not available for the selected dates.";
            return RedirectToAction("Details", "Car", new { id = model.CarId });
        }

        var days = (model.EndDate - model.StartDate).Days;
        if (days <= 0) days = 1;

        var rental = new Rental
        {
            CarId = model.CarId,
            RenterId = userId.Value,
            OwnerId = car.OwnerId,
            StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(model.EndDate, DateTimeKind.Utc),
            TotalPrice = days * car.CostPerDay
        };

        _db.Rentals.Add(rental);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Booking successful!";
        return RedirectToAction(nameof(MyRentals));
    }
}
