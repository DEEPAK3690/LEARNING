using System.Collections.Concurrent;
using CleanArchitectureExample.Application.Contracts;
using CleanArchitectureExample.Domain.Entities;

namespace CleanArchitectureExample.Infrastructure.Persistence;

public sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _store = new();

    public Task<TodoItem> AddAsync(TodoItem todoItem, CancellationToken cancellationToken)
    {
        _store[todoItem.Id] = todoItem;
        return Task.FromResult(todoItem);
    }

    public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _store.TryGetValue(id, out var todoItem);
        return Task.FromResult(todoItem);
    }

    public Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        var items = _store.Values
            .OrderBy(item => item.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<TodoItem>>(items);
    }

    public Task UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken)
    {
        _store[todoItem.Id] = todoItem;
        return Task.CompletedTask;
    }
}
