using CleanArchitectureExample.Application.Contracts;
using CleanArchitectureExample.Application.Mappings;
using CleanArchitectureExample.Application.Models;

namespace CleanArchitectureExample.Application.UseCases;

public sealed class GetTodoItemsUseCase
{
    private readonly ITodoRepository _todoRepository;

    public GetTodoItemsUseCase(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<IReadOnlyList<TodoItemDto>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var todoItems = await _todoRepository.GetAllAsync(cancellationToken);
        return todoItems.Select(todo => todo.ToDto()).ToList();
    }
}
