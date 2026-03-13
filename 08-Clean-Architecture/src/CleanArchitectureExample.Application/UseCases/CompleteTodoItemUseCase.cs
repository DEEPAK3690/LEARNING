using CleanArchitectureExample.Application.Commands;
using CleanArchitectureExample.Application.Contracts;
using CleanArchitectureExample.Application.Exceptions;
using CleanArchitectureExample.Application.Mappings;
using CleanArchitectureExample.Application.Models;

namespace CleanArchitectureExample.Application.UseCases;

public sealed class CompleteTodoItemUseCase
{
    private readonly ITodoRepository _todoRepository;

    public CompleteTodoItemUseCase(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<TodoItemDto> ExecuteAsync(CompleteTodoItemCommand command, CancellationToken cancellationToken)
    {
        var todoItem = await _todoRepository.GetByIdAsync(command.Id, cancellationToken);

        if (todoItem is null)
        {
            throw new TodoItemNotFoundException(command.Id);
        }

        todoItem.MarkCompleted(DateTime.UtcNow);
        await _todoRepository.UpdateAsync(todoItem, cancellationToken);

        return todoItem.ToDto();
    }
}
