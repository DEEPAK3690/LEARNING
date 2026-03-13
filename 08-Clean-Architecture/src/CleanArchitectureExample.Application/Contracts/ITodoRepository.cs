using CleanArchitectureExample.Domain.Entities;

namespace CleanArchitectureExample.Application.Contracts;

public interface ITodoRepository
{
    Task<TodoItem> AddAsync(TodoItem todoItem, CancellationToken cancellationToken);
    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken);
}
