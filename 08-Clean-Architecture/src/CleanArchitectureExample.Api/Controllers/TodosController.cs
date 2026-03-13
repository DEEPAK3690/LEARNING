using CleanArchitectureExample.Application.Commands;
using CleanArchitectureExample.Application.Exceptions;
using CleanArchitectureExample.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureExample.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] GetTodoItemsUseCase getTodoItemsUseCase,
        CancellationToken cancellationToken)
    {
        var result = await getTodoItemsUseCase.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTodoRequest request,
        [FromServices] CreateTodoItemUseCase createTodoItemUseCase,
        CancellationToken cancellationToken)
    {
        var result = await createTodoItemUseCase.ExecuteAsync(
            new CreateTodoItemCommand(request.Title),
            cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(
        Guid id,
        [FromServices] CompleteTodoItemUseCase completeTodoItemUseCase,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await completeTodoItemUseCase.ExecuteAsync(
                new CompleteTodoItemCommand(id),
                cancellationToken);

            return Ok(result);
        }
        catch (TodoItemNotFoundException)
        {
            return NotFound();
        }
    }
}

public sealed record CreateTodoRequest(string Title);
