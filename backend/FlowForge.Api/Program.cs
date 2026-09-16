using FlowForge.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => {
    return "FlowForge API is running!";
});

app.MapGet("/workflows", async (AppDbContext db) => {
    var workflows = await db.Workflows.ToListAsync();
    return Results.Ok(workflows);
});

app.Run();