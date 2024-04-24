using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

namespace MyNewApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
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
        app.MapGet("/todos", () => todos);
        app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id) =>
        {
            var targetTodo = todos.SingleOrDefault(todo => todo.Id == id);
            return targetTodo is not null ? TypedResults.Ok(targetTodo) : TypedResults.NotFound();
        });
        app.MapPost("/todos", (Todo task) =>
        {
            todos.Add(task);
            return TypedResults.Created("/todos/{id}", task);
        });
        app.MapDelete("/todos/{id}", (int id) =>
        {
            todos.RemoveAll(todo => todo.Id == id);
            return TypedResults.NoContent();
        });

        app.Run();
    }
    public record Todo(int Id, string Name, DateTime DateTime, bool IsCompleted);
}
