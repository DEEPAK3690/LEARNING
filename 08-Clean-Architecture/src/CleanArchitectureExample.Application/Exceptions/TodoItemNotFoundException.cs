namespace CleanArchitectureExample.Application.Exceptions;

public sealed class TodoItemNotFoundException : Exception
{
    public TodoItemNotFoundException(Guid id)
        : base($"Todo item with id '{id}' was not found.")
    {
    }
}
