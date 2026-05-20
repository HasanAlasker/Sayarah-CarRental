using System.IdentityModel.Tokens.Jwt;
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
            AttachUserToContext(context, token);

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
                ClockSkew = TimeSpan.Zero // token expires exactly on time
            }, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;

            // Attach claims to HttpContext.Items for use in filters/controllers
            context.Items["UserId"] = int.Parse(
                jwt.Claims.First(c => c.Type == "nameid").Value);

            context.Items["Roles"] = jwt.Claims
                .Where(c => c.Type == "role")
                .Select(c => c.Value)
                .ToList();
        }
        catch
        {
            // invalid
        }
    }
}