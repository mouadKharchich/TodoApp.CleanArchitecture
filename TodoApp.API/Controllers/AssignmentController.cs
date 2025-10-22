using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
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

    public AssignmentController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpGet]
    public async Task<ActionResult<AssignmentResponseDto>> GetAssignmentsAsync()
    {
     var response = await _assignmentService.GetAllAssignmentsAsync();
     return Ok(response);
    }
    
    // Get All asignments of Task Item by TaskId
    [HttpGet("taskItems/{taskItemId}")]
    public async Task<ActionResult<AssignmentResponseDto>> GetAssignmentsByTaskIdAsync(Guid taskItemId)
    {
        var response = await _assignmentService.GetAssignmentsByTaskIdAsync(taskItemId);
        return Ok(response);
    }
    
    // Get All asignments of Task Item by TaskId
    [HttpGet("{id}")]
    public async Task<ActionResult<AssignmentResponseDto>> GetAssignmentsByIdAsync(Guid id)
    {
        var response = await _assignmentService.GetAssignmentsByIdAsync(id);
        return Ok(response);
    }
    
}