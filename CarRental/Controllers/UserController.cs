using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarRental.Data;
using CarRental.Filters;
using CarRental.Models;
using CarRental.ViewModels;
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
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var exists = await _db.Users.AnyAsync(u => u.Email == model.Email);
        if (exists)
        {
            ModelState.AddModelError("Email", "Email is already registered.");
            return View(model);
        }

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Password = "",
            Phone = model.Phone,
            DateOfBirth = DateTime.SpecifyKind(model.DateOfBirth, DateTimeKind.Utc),
            Gender = model.Gender,
            AgreedToTermsAndConditions = model.AgreedToTermsAndConditions
        };

        var hasher = new PasswordHasher<User>();
        user.Password = hasher.HashPassword(user, model.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Assign default "User" role
        var userRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        if (userRole != null)
        {
            _db.UserRoles.Add(new UserRoles { UserId = user.Id, RoleId = userRole.Id });
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Login));
    }
    
    public IActionResult Login() => View();
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var match = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (match is null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(match, match.Password, model.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        var roles = await _db.UserRoles
            .Where(ur => ur.UserId == match.Id)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

        var token = GenerateJwtToken(match, roles);

        HttpContext.Session.SetInt32("UserId", match.Id);
        HttpContext.Session.SetString("Token", token);
        HttpContext.Session.SetString("UserName", match.Name);
        HttpContext.Session.SetString("Roles", string.Join(",", roles));

        return RedirectToAction("Index", "Home");
    }
    
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