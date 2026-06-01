using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CarRental.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    public JwtMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"]
                        .FirstOrDefault()?.Split(" ").Last()
                    ?? context.Session.GetString("Token");

        if (token != null)
        {
            AttachUserToContext(context, token);
        }
        else
        {
            // Fallback: If no token but session has UserId, populate Items for compatibility
            var userId = context.Session.GetInt32("UserId");
            if (userId != null)
            {
                context.Items["UserId"] = userId;
                context.Items["Roles"] = context.Session.GetString("Roles")?.Split(',').ToList() ?? new List<string>();
            }
        }

        await _next(context);
    }

    private void AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;

            // Use the standard ClaimTypes or the specific strings used in GenerateJwtToken
            var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid");
            if (userIdClaim != null)
            {
                context.Items["UserId"] = int.Parse(userIdClaim.Value);
            }

            context.Items["Roles"] = jwt.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .ToList();
        }
        catch
        {
            // If token validation fails, we still have the session fallback in Invoke if needed,
            // or we let the filters handle the missing Items.
        }
    }
}