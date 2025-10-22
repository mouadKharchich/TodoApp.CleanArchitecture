# TodoApp.CleanArchitecture

A **.NET 6 Web API** implementing a **Clean Architecture** for a Task Management Application.  
This project uses **SQL Server** as the database provider and **xUnit** for unit testing.

[Chari Test technique backend.pdf](https://github.com/user-attachments/files/22992596/Chari.Test.technique.backend.pdf)

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Domain Model](#domain-model)
- [Technologies](#technologies)
- [Getting Started](#getting-started)

---

## Features

- User management (CRUD operations)
- Task management with:
  - Title, description, status, priority, deadlines
  - Assignment of tasks to users
- Assignments tracking between tasks and users
- Audit fields (CreatedAt, UpdatedAt)

---

## Architecture

The project follows **Clean Architecture principles**:

<img width="223" height="226" alt="download" src="https://github.com/user-attachments/assets/6b8b76ad-0bfd-4d7b-b6a7-1cf9670cfc7e" />

- **Domain Layer**: Contains entities, enums, and domain logic.
- **Application Layer**: Contains DTOs, service interfaces, and business logic , Mapping , Validators,Common.
- **Infrastructure Layer**: Database context and repository implementations.
- **API Layer**: ASP.NET Core Web API exposing endpoints.
- **Tests Layer**: Unit tests using xUnit.

This separation ensures **high maintainability**, **testability**, and **scalability**.

---

## Domain Model
**User 1** ── * **TaskItem**
**User** **1** ── * **Assignment**
**TaskItem** **1** ── * **Assignment**

<img width="500" height="500" alt="3e2e0fe6-df7c-4a1a-a3e6-1c7026749fdb" src="https://github.com/user-attachments/assets/017b5d8d-6e15-4775-91a2-0b762eb3ca24" />

-- **User**

-- **TaskItem**

-- **Assignment**

### Domain Relationships

- **User**  
  Represents an application user. A user can have multiple tasks assigned and multiple task assignments.

- **TaskItem**  
  Represents a task with properties like `Title`, `Description`, `Status`, `Priority`, and `Deadline`. A task may belong to a user and can have multiple assignments.

- **Assignment**  
  Represents the many-to-many relationship between users and tasks. Tracks when a task was assigned to a user.

---

## Application Features

### Validation and Filtering

- **FluentValidation** is used for validating DTOs.
- Supports filtering tasks by:
  - Status
  - Priority
  - User
  - Deadline ranges
- Ensures invalid requests return meaningful validation messages.

### Exception Handling

- Centralized **global exception handler** using middleware.
- Custom exceptions for:
  - `NotFoundException`
  - `ValidationException`
  - `UnauthorizedAccessException`
- Returns consistent JSON error responses with status codes and messages.

### AutoMapper

- Maps between **Domain Entities** and **DTOs**.
- Ensures clean separation between domain layer and API layer.
- Example mappings:
  - `User` → `UserResponseDto`
  - `TaskItem` → `TaskItemResponseDto`
  - `Assignment` → `AssignmentResponseDto`

### JWT Authentication

- Users register/login to receive a **JWT Bearer token**.
- Token contains user claims for secure access to protected endpoints.
- Protected API routes require `Authorization: Bearer {token}` header.
- JWT settings configurable in `appsettings.json`.

### Unit Testing with xUnit

- **xUnit** is used for unit tests.
- **Moq** is used for mocking dependencies in services and repositories.
- Typical tests cover:
  - User registration and authentication
  - Task creation, update, deletion
  - Assignment management
  - Exception scenarios and validation failures
- Tests ensure the application adheres to business rules without relying on a real database.

---

## Technologies

- **.NET 6** (C#)
- **ASP.NET Core Web API**
- **Entity Framework Core** (SQL Server)
- **xUnit** (unit testing)
- **Moq** (mocking)
- **JWT** (authentication)
- **Mapper** (Manual mapping)
- **FluentValidation** (DTO validation)
- **Swagger/OpenAPI** (API documentation)

---

## Getting Started

1. Clone the repository:

```bash
git clone https://github.com/yourusername/TodoApp.CleanArchitecture.git
cd TodoApp
```

2. Update the connection string in appsettings.Development.json:
3. Ensure your SQL Server database is running.
4. Create the initial migration and update the database:

```bash
# Create migration
dotnet ef migrations add [Migration-name("Initilisation")] --project TodoApp.Infrastructure --startup-project TodoApp.API
```

```bash
# Apply migration to database
dotnet ef database update --project TodoApp.Infrastructure --startup-project TodoApp.API
```

5. Run the application:

```bash
dotnet run --project TodoApp.API
```

6. To run unit tests, go to the `Tests` folder → `TodoApp.ApplicationTests` project and run all the tests you need.

7. All API endpoints require authentication except the public APIs like **Get Tasks** and **Get Task By User**.

8. The Task API supports parameters for:

- **Search**: Filter tasks by title, description, or assigned user.
- **Pagination**: Specify page number and page size.
- **Filter**: Filter tasks by status, priority, or due date.

Example API request:

```bash
GET /api/tasks?search=meeting&pageNumber=1&pageSize=10&status=Pending
```

<img width="1287" height="881" alt="image" src="https://github.com/user-attachments/assets/6695088f-b30c-4391-a51b-08fe63107faa" />
