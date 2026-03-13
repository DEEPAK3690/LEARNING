using CleanArchitectureExample.Application.Commands;
using CleanArchitectureExample.Application.Contracts;
using CleanArchitectureExample.Application.Mappings;
using CleanArchitectureExample.Application.Models;
using CleanArchitectureExample.Domain.Entities;

namespace CleanArchitectureExample.Application.UseCases;

public sealed class CreateTodoItemUseCase
{
    private readonly ITodoRepository _todoRepository;

    public CreateTodoItemUseCase(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<TodoItemDto> ExecuteAsync(CreateTodoItemCommand command, CancellationToken cancellationToken)
    {
        var todoItem = new TodoItem(Guid.NewGuid(), command.Title, DateTime.UtcNow);
        var saved = await _todoRepository.AddAsync(todoItem, cancellationToken);
        return saved.ToDto();
    }
}
