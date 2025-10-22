using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common.Enums;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Models;
using TodoApp.Application.Dtos.Assignment;
using TodoApp.Application.Interfaces.IServices;

namespace TodoApp.API.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[ApiController]
[Route("api/v{version:apiVersion}/assignments")]
public class AssignmentController: ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly ILogger<ErrorResponse> _logger;

    public AssignmentController(IAssignmentService assignmentService, ILogger<ErrorResponse> logger)
    {
        _assignmentService = assignmentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<AssignmentResponseDto>> GetAssignmentsAsync()
    {
        try
        {
            var response = await _assignmentService.GetAllAssignmentsAsync();
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
    
    // Get All asignments of Task Item by TaskId
    [HttpGet("taskItems/{taskItemId}")]
    public async Task<ActionResult<AssignmentResponseDto>> GetAssignmentsByTaskIdAsync(Guid taskItemId)
    {
        try
        {
            var response = await _assignmentService.GetAssignmentsByTaskIdAsync(taskItemId);
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
    
    // Get All asignments of Task Item by TaskId
    [HttpGet("{id}")]
    public async Task<ActionResult<AssignmentResponseDto>> GetAssignmentsByIdAsync(Guid id)
    {
        try
        {
            var response = await _assignmentService.GetAssignmentsByIdAsync(id);
            return Ok(response);
        } 
        catch (NotFoundException ex)
        {
            var errorResponse = new ErrorResponse((int)ErrorStatus.NotFound, "Assignment not found", ex.Message,_logger);
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
    
}