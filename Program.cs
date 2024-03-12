using TodoApi2;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Todo items for the test
app.MapGet("/Todo", async (DataContext db) =>
    await db.Todo.ToListAsync());

app.MapGet("/Todo/{id}", async (int id, DataContext db) =>
    await db.Todo.FindAsync(id)
        is Todo toDo
        ? Results.Ok(toDo) : Results.NotFound());

app.MapPost("Add/Todo", async (Todo todo, DataContext db) =>
{
    db.Todo.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/Todo/{todo.ID}", todo);
});

app.MapPut("/Todo/{id}", async (int id, Todo inputTodo, DataContext db) =>
{
    var todo = await db.Todo.FindAsync(id);
    if (todo == null) return Results.NotFound();

    todo.ItemName = inputTodo.ItemName;
    todo.Description = inputTodo.Description;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/Delete/{id}", async (int id, DataContext db) =>
{
    if (await db.Todo.FindAsync(id) is Todo todo)
    {
        db.Todo.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }
    return Results.NotFound();
});

app.Run();
