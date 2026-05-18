using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CarRental.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly RentalDbContext _db;
    private readonly IConfiguration _config;

    public UserController(ILogger<UserController> logger, RentalDbContext db, IConfiguration config)
    {
        _logger = logger;
        _db = db;
        _config = config;
    }

    // GET /User/Index — only logged in users
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var users = await _db.Users.ToListAsync();
        return View(users);
    }

    // GET /User/Register
    public IActionResult Register() => View();

    // POST /User/Register
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

    // GET /User/Login
    public IActionResult Login() => View();

    // POST /User/Login — returns JWT token + roles
    [HttpPost]
    public async Task<IActionResult> Login(User user)
    {
        // Step 1: Find by email only
        var match = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == user.Email);

        if (match is null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(user);
        }

        // Step 2: Verify hashed password
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(match, match.Password, user.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(user);
        }

        // Step 3: Fetch this user's roles
        var roles = await _db.UserRoles
            .Where(ur => ur.UserId == match.Id)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

        // Step 4: Build JWT
        var token = GenerateJwtToken(match, roles);

        // Step 5: Store userId in session (optional, for MVC views)
        HttpContext.Session.SetInt32("UserId", match.Id);
        HttpContext.Session.SetString("Token", token);

        // Return token — use Ok() if this is an API, or store and redirect for MVC
        return Ok(new
        {
            token,
            email = match.Email,
            roles
        });
    }
    
    [Authorize]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    // -------------------------------------------------------
    // Private helper — builds the JWT token
    // -------------------------------------------------------
    private string GenerateJwtToken(User user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        // One claim per role
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