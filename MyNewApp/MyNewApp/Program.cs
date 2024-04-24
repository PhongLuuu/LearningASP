using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;
using static MyNewApp.Program;

namespace MyNewApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<ITaskService>(new InMemoryTaskService());
        var app = builder.Build();

        app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));
        app.Use(async (context, next) =>
        {
            Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started.");
            await next(context);
            Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Finished.");

            //await context.Response.WriteAsync("<div> before - CustomeMiddleware </div>");
            //await _next(context);
            //await context.Response.WriteAsync("<div> after - CustomeMiddleware </div>");
        });

        var todos = new List<Todo>();
        app.MapGet("/todos", (ITaskService service) => service.GetTodos());
        app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id, ITaskService service) =>
        {
            var targetTodo = service.GetTodoById(id);
            return targetTodo is not null ? TypedResults.Ok(targetTodo) : TypedResults.NotFound();
        });
        app.MapPost("/todos", (Todo task, ITaskService service) =>
        {
            service.AddTodo(task);
            return TypedResults.Created("/todos/{id}", task);
        })
        .AddEndpointFilter(async (context, next) =>
        {
            var taskArgument = context.GetArgument<Todo>(0);
            var errors = new Dictionary<string, string[]>();
            if (taskArgument.DueDate < DateTime.UtcNow)
            {
                errors.Add(nameof(Todo.DueDate), ["Cannot have due date in the past."]);
            }
            if (taskArgument.IsCompleted)
            {
                errors.Add(nameof(Todo.IsCompleted), ["Cannot add completed task."]);
            }
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }
            return await next(context);
        });
        app.MapDelete("/todos/{id}", (int id, ITaskService service) =>
        {
            service.DeleteTodoById(id);
            return TypedResults.NoContent();
        });

        app.Run();
    }
    public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted);
}
interface ITaskService
{
    List<Todo> GetTodos();
    Todo? GetTodoById(int id);
    Todo AddTodo(Todo task);
    void DeleteTodoById(int id);
}
class InMemoryTaskService : ITaskService
{
    private readonly List<Todo> _todos = [];
    public List<Todo> GetTodos() => _todos;
    public Todo? GetTodoById(int id) => _todos.SingleOrDefault(todo => todo.Id == id);
    public Todo AddTodo(Todo task)
    {
        _todos.Add(task);
        return task;
    }
    public void DeleteTodoById(int id) => _todos.RemoveAll(todo => todo.Id == id);
}
