# Clean Architecture Example (C#)

This folder contains a minimal, runnable Clean Architecture sample for learning.

Detailed walkthrough: [CLEAN-ARCHITECTURE-EXPLAINED.md](CLEAN-ARCHITECTURE-EXPLAINED.md)

## Architecture

- `src/CleanArchitectureExample.Domain`
  - Enterprise business rules (entities only).
- `src/CleanArchitectureExample.Application`
  - Use cases, contracts, DTOs, and application exceptions.
- `src/CleanArchitectureExample.Infrastructure`
  - Repository implementation (in-memory persistence for learning).
- `src/CleanArchitectureExample.Api`
  - ASP.NET Core Web API entry point and controllers.
- `tests/CleanArchitectureExample.Application.Tests`
  - Unit tests for application use cases.

Dependency direction is inward only:

- API -> Application + Infrastructure
- Infrastructure -> Application + Domain
- Application -> Domain
- Domain -> (no project dependencies)

## Run

```bash
dotnet build CleanArchitectureExample.slnx
dotnet test CleanArchitectureExample.slnx
dotnet run --project src/CleanArchitectureExample.Api
```

## API Endpoints

- `GET /api/todos` - get all todo items
- `POST /api/todos` - create a todo item
  - body: `{ "title": "Learn clean architecture" }`
- `PUT /api/todos/{id}/complete` - mark a todo item as complete

## What to study

- How application use cases avoid framework dependencies.
- How repository abstractions are owned by the application layer.
- How infrastructure can be swapped (in-memory now, EF Core later) without changing use cases.
