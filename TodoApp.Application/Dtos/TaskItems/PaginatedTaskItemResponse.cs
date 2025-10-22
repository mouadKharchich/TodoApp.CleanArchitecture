namespace TodoApp.Application.Dtos.TaskItem;

public class PaginatedTaskItemResponse
{
    public int TotalCount { get; set; }
    public int TotalPage { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public List<TaskItemResponseDto> Items { get; set; } = new List<TaskItemResponseDto>();
}