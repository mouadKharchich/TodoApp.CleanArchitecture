using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Application.Dtos.JWT;
using TodoApp.Application.Dtos.User;
using TodoApp.Application.Interfaces.IServices;

namespace TodoApp.Application.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    
    public JwtService(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }
    
    // <summary>
    /// Generates a JWT token for a user.
    /// </summary>
    public JwtSecurityToken GenerateJwtToken(UserResponseDto user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("username", user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );

        return token;
    }
}