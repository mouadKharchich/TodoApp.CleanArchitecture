using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common.Models;
using TodoApp.Application.Dtos.TaskItem;
using TodoApp.Application.Dtos.User;
using TodoApp.Application.Interfaces.IServices;
using TodoApp.Domain.Enums;
using TaskStatus = TodoApp.Domain.Enums.TaskStatus;

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
        var response = await _taskItemService.GetAllTasksAsync();
        return Ok(response);
    }

    [HttpGet("parameters")]
    public async Task<ActionResult<IEnumerable<TaskItemResponseDto>>> GetTaskItemsParameters([FromQuery]TaskItemQueryParameters queryParameters)
    {
        var response = await _taskItemService.GetTasksByQueryAsync(queryParameters);
        return Ok(response);
    }
    
    [HttpGet("{taskItemId}")]
    public async Task<ActionResult<TaskItemResponseDto>> GetById(Guid taskItemId)
    {
         var item = await _taskItemService.GetTaskByIdAsync(taskItemId);
         return Ok(item);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TaskItemResponseDto>> Create(TaskItemRequestDto item)
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

    [HttpPut("{taskItemId}")]
    [Authorize]
    public async Task<ActionResult> Update(Guid? taskItemId, TaskItemUpdateDto item)
    {
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

    [HttpDelete("{taskItemId}")]
    [Authorize]
    public async Task<ActionResult> Delete(Guid taskItemId)
    {
        await _taskItemService.DeleteTaskAsync(taskItemId);
        return NoContent();
    }
    
    /* Assign user to task */
    [HttpPut("{taskId}/assign-user")]
    [Authorize]
    public async Task<IActionResult> AssignUser(Guid taskId, [FromBody] AssignUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            throw new ValidationException(string.Join("; ", errors));
        }
        
        var updatedTask = await _taskItemService.AssignUserToTaskAsync(taskId, dto.UserId);
        return Ok(updatedTask);
    }
    
    /* Update Status */
    [HttpPut("{taskId}/update-status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(Guid taskId, [FromBody] UpdateStatusDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            throw new ValidationException(string.Join("; ", errors));
        }
        
        var updatedTask = await _taskItemService.UpdateStatusAsync(taskId, (TaskStatus)dto.Status);
        return Ok(updatedTask);
    }
    
    /* Update Priority */
    [HttpPut("{taskId}/update-priority")]
    [Authorize]
    public async Task<IActionResult> UpdatePriority(Guid taskId, [FromBody] UpdatePriorityDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            throw new ValidationException(string.Join("; ", errors));
        }
        
        var updatedTask = await _taskItemService.UpdatePriorityAsync(taskId, (PriorityLevel)dto.Priority);
        return Ok(updatedTask);
    }
    
}
