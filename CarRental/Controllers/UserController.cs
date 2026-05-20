using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarRental.Data;
using CarRental.Filters;
using CarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CarRental.Controllers;

public class UserController : Controller
{
    private readonly RentalDbContext _db;
    private readonly IConfiguration _config;

    public UserController(RentalDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [Authenticated]
    public async Task<IActionResult> Index()
    {
        var users = await _db.Users.ToListAsync();
        return View(users);
    }

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        if (!ModelState.IsValid) return View(user);

        // Check if email already exists
        var exists = await _db.Users.AnyAsync(u => u.Email == user.Email);
        if (exists)
        {
            ModelState.AddModelError("Email", "Email is already registered.");
            return View(user);
        }

        // Hash password before saving
        var hasher = new PasswordHasher<User>();
        user.Password = hasher.HashPassword(user, user.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Login));
    }
    
    public IActionResult Login() => View();
    
    [HttpPost]
    public async Task<IActionResult> Login(User user)
    {
        var match = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == user.Email);

        if (match is null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(user);
        }

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(match, match.Password, user.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(user);
        }

        var roles = await _db.UserRoles
            .Where(ur => ur.UserId == match.Id)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

        var token = GenerateJwtToken(match, roles);

        HttpContext.Session.SetInt32("UserId", match.Id);
        HttpContext.Session.SetString("Token", token);

        return View();
    }
    
    [Authorize]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
    
    private string GenerateJwtToken(User user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };
        
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}