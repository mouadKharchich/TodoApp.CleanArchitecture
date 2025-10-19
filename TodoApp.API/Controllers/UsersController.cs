using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common.Enums;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Models;
using TodoApp.Application.Dtos.User;
using TodoApp.Application.Interfaces.IServices;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

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
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Internal server error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserResponseDto>> GetUser(Guid userId)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.NotFound, "User not found", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.NotFound, errorResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Internal server error", ex.Message);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
    }

    [HttpGet("{userId}/with-tasks")]
    public async Task<ActionResult<UserResponseDto>> GetUserWithTasks(Guid userId)
    {
        try
        {
            var user = await _userService.GetUserByIdWithTasksAsync(userId);
            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.NotFound, "User not found", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.NotFound, errorResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Internal server error", ex.Message);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
    {
        try
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
        catch (ValidationException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.BadRequest, "Validation error while Register User", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.BadRequest, errorResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Internal server error", ex.Message);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        try
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
        catch (UnauthorizedAccessException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.Unauthorized, "Unauthorized", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.Unauthorized, errorResponse);
        }
        catch (ValidationException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.BadRequest, "Validation error while Login User", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.BadRequest, errorResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Internal server error", ex.Message);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
    }
}
