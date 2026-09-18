using System.ComponentModel.DataAnnotations;
using FlowForge.Api.Data;
using FlowForge.Api.Models;
using FlowForge.Api.Dtos;
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

app.MapGet("/workflows/{id}", async (Guid id, AppDbContext db) => {
    var workflow = await db.Workflows.FindAsync(id);
    if (workflow is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(workflow);
});

app.MapPost("/workflows", async (CreateWorkflowRequest request, AppDbContext db) => {
    var validationContext = new ValidationContext(request);
    var validationResults = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(
        request,
        validationContext,
        validationResults,
        validateAllProperties: true
    );

    if (!isValid)
    {
        return Results.BadRequest(new
        {
            errors = validationResults.Select(result => result.ErrorMessage)
        });
    }
    
    var workflow = new Workflow
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Description = request.Description,
        CreatedAt = DateTime.UtcNow
    };
    db.Workflows.Add(workflow);
    await db.SaveChangesAsync();
    return Results.Created($"/workflows/{workflow.Id}", workflow);
});

app.MapPut("/workflows/{id}", async (Guid id, Workflow updatedWorkflow, AppDbContext db) => {
    var workflow = await db.Workflows.FindAsync(id);
    if (workflow is null)
    {
        return Results.NotFound();
    }
    workflow.Name = updatedWorkflow.Name;
    workflow.Description = updatedWorkflow.Description;
    workflow.Status = updatedWorkflow.Status;
    await db.SaveChangesAsync();
    return Results.Ok(workflow);
});

app.MapDelete("/workflows/{id}", async (Guid id, AppDbContext db) => {
    var workflow = await db.Workflows.FindAsync(id);
    if (workflow is null)
    {
        return Results.NotFound();
    }
    db.Workflows.Remove(workflow);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();