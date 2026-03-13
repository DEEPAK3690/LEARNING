namespace CleanArchitectureExample.Application.Models;

public sealed record TodoItemDto(
    Guid Id,
    string Title,
    bool IsCompleted,
    DateTime CreatedAtUtc,
    DateTime? CompletedAtUtc);
