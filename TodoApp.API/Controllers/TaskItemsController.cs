using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common.Enums;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Models;
using TodoApp.Application.Dtos.TaskItem;
using TodoApp.Application.Dtos.User;
using TodoApp.Application.Interfaces.IServices;
using TodoApp.Domain.Enums;
using TaskStatus = TodoApp.Domain.Enums.TaskStatus;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace TodoApp.API.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[ApiController]
[Route("api/v{version:apiVersion}/taskItems")]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;
    private readonly ILogger<ErrorResponse> _logger;

    public TaskItemsController(ITaskItemService taskItemService, ILogger<ErrorResponse> logger)
    {
        _taskItemService = taskItemService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItemResponseDto>>> GetAll()
    {
        try
        {
            var response = await _taskItemService.GetAllTasksAsync();
            return Ok(response);
        }
        catch (RepositoryException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Database error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Internal server error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
    }

    [HttpGet("parameters")]
    public async Task<ActionResult<IEnumerable<TaskItemResponseDto>>> GetTaskItemsParameters([FromQuery]TaskItemQueryParameters queryParameters)
    {
        try
        {
            var response = await _taskItemService.GetTasksByQueryAsync(queryParameters);
            return Ok(response);
        }
        catch (RepositoryException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Database error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Internal server error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
    }
    
    [HttpGet("{taskItemId}")]
    public async Task<ActionResult<TaskItemResponseDto>> GetById(Guid taskItemId)
    {
        try
        {
            var item = await _taskItemService.GetTaskByIdAsync(taskItemId);
            return Ok(item);
        }
        catch (NotFoundException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.NotFound, "Task Item not found", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.NotFound, errorResponse);
        }
        catch (RepositoryException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Database error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Internal server error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TaskItemResponseDto>> Create(TaskItemRequestDto item)
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
            var createdTask = await _taskItemService.AddTaskAsync(item);
            return Ok(createdTask);
        }
        catch (ValidationException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.BadRequest, "Validation error while creating task Item", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.BadRequest, errorResponse);
        }
        catch (RepositoryException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Database error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Internal server error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
    }

    [HttpPut("{taskItemId}")]
    [Authorize]
    public async Task<ActionResult> Update(Guid? taskItemId, TaskItemUpdateDto item)
    {
        try
        {
            if (taskItemId == null)
            {
                var errorResponse = new ErrorResponse((int)ErrorStatus.BadRequest, "Bad Request", "Task ID is required",_logger);
                return BadRequest(errorResponse);
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                throw new ValidationException(string.Join("; ", errors));
            }
            var updatedTask = await _taskItemService.UpdateTaskAsync(taskItemId, item);
            return Ok(updatedTask);
        }
        catch (ValidationException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.BadRequest, "Validation error while updating task Item", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.BadRequest, errorResponse);
        }
        catch (NotFoundException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.NotFound, "Task Item not found", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.NotFound, errorResponse);
        }
        catch (RepositoryException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Database error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Internal server error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
    }

    [HttpDelete("{taskItemId}")]
    [Authorize]
    public async Task<ActionResult> Delete(Guid taskItemId)
    {
        try
        {
            await _taskItemService.DeleteTaskAsync(taskItemId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.NotFound, "Task Item not found", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.NotFound, errorResponse);
        }
        catch (RepositoryException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Database error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.InternalServerError, "Internal server error", ex.Message,_logger);
            return StatusCode((int)ErrorStatus.InternalServerError, errorResponse);
        }
    }
    
    /* Assign user to task */
    [HttpPut("{taskId}/assign-user")]
    [Authorize]
    public async Task<IActionResult> AssignUser(Guid taskId, [FromBody] AssignUserDto dto)
    {
        try
        {
            var updatedTask = await _taskItemService.AssignUserToTaskAsync(taskId, dto.UserId);
            return Ok(updatedTask);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse(404, ex.Message));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ErrorResponse(400, ex.Message));
        }
    }
    
    /* Update Status */
    [HttpPut("{taskId}/update-status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(Guid taskId, [FromBody] UpdateStatusDto dto)
    {
        try
        {
            var updatedTask = await _taskItemService.UpdateStatusAsync(taskId, (TaskStatus)dto.Status);
            return Ok(updatedTask);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse(404, ex.Message));
        }
    }
    
    /* Update Priority */
    [HttpPut("{taskId}/update-priority")]
    [Authorize]
    public async Task<IActionResult> UpdatePriority(Guid taskId, [FromBody] UpdatePriorityDto dto)
    {
        try
        {
            var updatedTask = await _taskItemService.UpdatePriorityAsync(taskId, (PriorityLevel)dto.Priority);
            return Ok(updatedTask);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse(404, ex.Message));
        }
    }
    
}
