using CleanArchitectureExample.Application.Contracts;
using CleanArchitectureExample.Application.UseCases;
using CleanArchitectureExample.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
builder.Services.AddScoped<CreateTodoItemUseCase>();
builder.Services.AddScoped<GetTodoItemsUseCase>();
builder.Services.AddScoped<CompleteTodoItemUseCase>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
