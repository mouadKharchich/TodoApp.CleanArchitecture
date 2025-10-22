using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common.Models;
using TodoApp.Application.Dtos.User;
using TodoApp.Application.Interfaces.IServices;

namespace TodoApp.API.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<ErrorResponse> _logger;


    public UsersController(IUserService userService, ILogger<ErrorResponse> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserResponseDto>> GetUser(Guid userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        return Ok(user);
    }

    [HttpGet("{userId}/with-tasks")]
    public async Task<ActionResult<UserResponseDto>> GetUserWithTasks(Guid userId)
    {
        var user = await _userService.GetUserByIdWithTasksAsync(userId);
        return Ok(user);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            throw new ValidationException(string.Join("; ", errors));
        }
        var createdUser = await _userService.RegisterUserAsync(registerUserDto);
        return Ok(createdUser);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            throw new ValidationException(string.Join("; ", errors));
        }
        
        var loginUserResponse = await _userService.LoginAsync(loginUserDto);
        return Ok(loginUserResponse);
    }
}
