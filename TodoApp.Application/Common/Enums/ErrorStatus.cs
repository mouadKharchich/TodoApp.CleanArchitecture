namespace TodoApp.Application.Common.Enums;

public enum ErrorStatus
{
    None = 0,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    ValidationError = 422,
    InternalServerError = 500
}