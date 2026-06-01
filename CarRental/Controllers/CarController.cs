using CarRental.Data;
using CarRental.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers;

public class CarController : Controller
{
    private readonly RentalDbContext _db;

    public CarController(RentalDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? search, int? fuelId, int? typeId, int? transmissionId)
    {
        var query = _db.Cars
            .Include(c => c.Fuel)
            .Include(c => c.Type)
            .Include(c => c.Transmission)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Name.Contains(search) || c.Make.Contains(search));
        }

        if (fuelId.HasValue) query = query.Where(c => c.FuelId == fuelId);
        if (typeId.HasValue) query = query.Where(c => c.TypeId == typeId);
        if (transmissionId.HasValue) query = query.Where(c => c.TransmissionId == transmissionId);

        ViewBag.Fuels = await _db.Fuels.ToListAsync();
        ViewBag.Types = await _db.Types.ToListAsync();
        ViewBag.Transmissions = await _db.Transmissions.ToListAsync();

        var cars = await query.ToListAsync();
        return View(cars);
    }

    public async Task<IActionResult> Details(int id)
    {
        var car = await _db.Cars
            .Include(c => c.Fuel)
            .Include(c => c.Type)
            .Include(c => c.Transmission)
            .Include(c => c.Owner)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car == null) return NotFound();

        return View(car);
    }
}
