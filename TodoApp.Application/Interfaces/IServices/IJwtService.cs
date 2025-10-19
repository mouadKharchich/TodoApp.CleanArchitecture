using System.IdentityModel.Tokens.Jwt;
using TodoApp.Application.Dtos.User;

namespace TodoApp.Application.Interfaces.IServices;

public interface IJwtService
{
    JwtSecurityToken GenerateJwtToken(UserResponseDto user);
}