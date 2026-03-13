using CleanArchitectureExample.Application.Commands;
using CleanArchitectureExample.Application.Contracts;
using CleanArchitectureExample.Application.Exceptions;
using CleanArchitectureExample.Application.UseCases;
using CleanArchitectureExample.Domain.Entities;

namespace CleanArchitectureExample.Application.Tests;

public class TodoUseCasesTests
{
    [Fact]
    public async Task CreateTodoItem_SavesAndReturnsTodo()
    {
        var repository = new FakeTodoRepository();
        var useCase = new CreateTodoItemUseCase(repository);

        var result = await useCase.ExecuteAsync(new CreateTodoItemCommand("Learn Clean Architecture"), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Learn Clean Architecture", result.Title);
        Assert.False(result.IsCompleted);
    }

    [Fact]
    public async Task CompleteTodoItem_Throws_WhenTodoMissing()
    {
        var repository = new FakeTodoRepository();
        var useCase = new CompleteTodoItemUseCase(repository);

        await Assert.ThrowsAsync<TodoItemNotFoundException>(() =>
            useCase.ExecuteAsync(new CompleteTodoItemCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task CompleteTodoItem_MarksTodoAsCompleted()
    {
        var repository = new FakeTodoRepository();
        var createUseCase = new CreateTodoItemUseCase(repository);
        var completeUseCase = new CompleteTodoItemUseCase(repository);

        var created = await createUseCase.ExecuteAsync(new CreateTodoItemCommand("Practice use cases"), CancellationToken.None);
        var completed = await completeUseCase.ExecuteAsync(new CompleteTodoItemCommand(created.Id), CancellationToken.None);

        Assert.True(completed.IsCompleted);
        Assert.NotNull(completed.CompletedAtUtc);
    }

    private sealed class FakeTodoRepository : ITodoRepository
    {
        private readonly Dictionary<Guid, TodoItem> _items = new();

        public Task<TodoItem> AddAsync(TodoItem todoItem, CancellationToken cancellationToken)
        {
            _items[todoItem.Id] = todoItem;
            return Task.FromResult(todoItem);
        }

        public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _items.TryGetValue(id, out var todoItem);
            return Task.FromResult(todoItem);
        }

        public Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken)
        {
            IReadOnlyList<TodoItem> items = _items.Values.ToList();
            return Task.FromResult(items);
        }

        public Task UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken)
        {
            _items[todoItem.Id] = todoItem;
            return Task.CompletedTask;
        }
    }
}
