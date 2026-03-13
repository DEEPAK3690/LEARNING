using CleanArchitectureExample.Application.Models;
using CleanArchitectureExample.Domain.Entities;

namespace CleanArchitectureExample.Application.Mappings;

internal static class TodoMappings
{
    public static TodoItemDto ToDto(this TodoItem todoItem)
    {
        return new TodoItemDto(
            todoItem.Id,
            todoItem.Title,
            todoItem.IsCompleted,
            todoItem.CreatedAtUtc,
            todoItem.CompletedAtUtc);
    }
}
