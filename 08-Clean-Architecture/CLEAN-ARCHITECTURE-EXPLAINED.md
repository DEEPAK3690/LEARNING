# Clean Architecture Explained (Learning Notes)

This guide explains how the sample in this folder is structured and why each layer exists.

## 1) Big Idea

Clean Architecture keeps business logic independent from frameworks, databases, and UI concerns.

Rule: dependencies point inward.

- Outer layers can depend on inner layers.
- Inner layers should not depend on outer layers.

In this project:

- API depends on Application + Infrastructure.
- Infrastructure depends on Application + Domain.
- Application depends on Domain.
- Domain depends on nothing.

## 2) Layer-by-Layer Explanation

## Domain Layer

Path: src/CleanArchitectureExample.Domain

Purpose:

- Holds core business concepts and rules.
- Should be framework-free.

Example:

- Entities/TodoItem.cs
- Behavior like MarkCompleted and Rename lives here because it is domain behavior.

Why this matters:

- Domain stays stable even if web framework or database changes.

## Application Layer

Path: src/CleanArchitectureExample.Application

Purpose:

- Coordinates use cases.
- Defines abstractions (contracts/interfaces) needed by use cases.
- Maps domain data to DTOs for external layers.

Key parts in this project:

- Contracts/ITodoRepository.cs: repository abstraction used by use cases.
- UseCases/CreateTodoItemUseCase.cs: creates a TodoItem.
- UseCases/GetTodoItemsUseCase.cs: reads todo list.
- UseCases/CompleteTodoItemUseCase.cs: marks todo completed.
- Models/TodoItemDto.cs: output model for API layer.

Why this matters:

- Business workflows are testable without HTTP, EF Core, or ASP.NET runtime.

## Infrastructure Layer

Path: src/CleanArchitectureExample.Infrastructure

Purpose:

- Implements interfaces declared by the Application layer.
- Handles technical concerns (database, messaging, files, external services).

Example in this sample:

- Persistence/InMemoryTodoRepository.cs implements ITodoRepository.

Why this matters:

- You can replace in-memory storage with EF Core later, with minimal change to application use cases.

## API Layer

Path: src/CleanArchitectureExample.Api

Purpose:

- Handles HTTP concerns: routing, status codes, request/response serialization, DI wiring.
- Delegates real work to application use cases.

Example in this sample:

- Controllers/TodosController.cs for GET/POST/PUT endpoints.
- Program.cs registers repository and use cases in DI.

Why this matters:

- Controller stays thin and focused on transport concerns.

## 3) Request Flow (End-to-End)

Example: complete todo

1. HTTP PUT /api/todos/{id}/complete hits TodosController.
2. Controller creates CompleteTodoItemCommand.
3. Controller calls CompleteTodoItemUseCase.
4. Use case loads entity through ITodoRepository.
5. Domain entity method MarkCompleted is executed.
6. Use case persists via repository and returns DTO.
7. Controller returns HTTP 200 (or 404 if not found).

## 4) Why This Design Helps Learning

- Clear separation of concerns.
- Easier unit testing (Application tests do not need web host).
- Easier replacement of infrastructure (in-memory to EF Core).
- Better long-term maintainability as project grows.

## 5) How to Run

From this folder:

```bash
dotnet build CleanArchitectureExample.slnx
dotnet test CleanArchitectureExample.slnx
dotnet run --project src/CleanArchitectureExample.Api
```

## 6) Quick API Try

Create a todo:

```http
POST /api/todos
Content-Type: application/json

{ "title": "Learn Clean Architecture deeply" }
```

List todos:

```http
GET /api/todos
```

Complete todo:

```http
PUT /api/todos/{id}/complete
```

## 7) Suggested Next Upgrades

1. Replace InMemoryTodoRepository with EF Core repository.
2. Add validation pipeline (for example FluentValidation).
3. Add integration tests for API endpoints.
4. Introduce CQRS pattern folders as features grow.


LINK :https://dotnettutorials.net/lesson/clean-architecture-in-asp-net-core-web-api/
